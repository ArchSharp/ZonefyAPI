using AutoMapper;
using Domain.Entities.Token;
using System.Net;
using System.Security.Cryptography;
using ZonefyDotnet.Common;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Repositories.Interfaces;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ForgotPassword> _forgotRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IRepository<HouseProperty> _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly INotificationService _notificationService;

        public UserService(
            IRepository<User> userRepository,
            IRepository<ForgotPassword> forgotRepository,
            IRepository<HouseProperty> propertyRepository,
            IMapper mapper,
            IJwtService jwtService,
            IRepository<RefreshToken> refreshTokenRepository,
            ITwoFactorAuthService twoFactorAuthService,
            INotificationService notificationService)
        {
            _userRepository = userRepository;
            _forgotRepository = forgotRepository;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _twoFactorAuthService = twoFactorAuthService;
            _notificationService = notificationService;
        }
        public async Task<SuccessResponse<GetUserDto>> CreateUser(CreateUserDTO model)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == model.Email);

            if (findUser != null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.UserAlreadyExist);

            var findUserPhone = await _userRepository.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);

            if(findUserPhone != null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.UserPhoneAlreadyExist);

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.Password = hashPassword;

            var emailVerifyToken = CreateRandomToken();

            var newUser = _mapper.Map<User>(model);
            newUser.VerificationCode = emailVerifyToken;
            newUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            _notificationService.SendVerificationEmail(newUser.Email, "User", emailVerifyToken);
            var newUserResponse = _mapper.Map<GetUserDto>(newUser);

            return new SuccessResponse<GetUserDto>
            {
                Data = newUserResponse,
                Code = 201,
                Message = ResponseMessages.NewUserCreated,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<string>> DeleteUser(string email)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == email) ?? throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);
            _userRepository.Remove(findUser);
            await _userRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = $"User with {email} have been deleted",
                Code = 200,
                Message = ResponseMessages.UserDeleted,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<GetUserDto>> Login(LoginUserDto model)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == model.Email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password))
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InCorrectPassword);

            var responseMessage = ResponseMessages.LoginSuccessful;

            // Create user Token
            string accessToken = _jwtService.CreateJwtToken(findUser);
            var isUserRefreshTokenInDb = await _refreshTokenRepository.FirstOrDefault(x => x.UserId == findUser.Id && x.ExpirationDate >= DateTime.UtcNow);
            var refreshToken = isUserRefreshTokenInDb == null ? await _jwtService.CreateRefreshToken() : isUserRefreshTokenInDb.Token;
            if (isUserRefreshTokenInDb == null)
                await InsertRefreshToken(findUser.Id, refreshToken);

            var extraIfo = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            //----------------------

            if (!findUser.IsEmailVerified)
            {
                TimeSpan timeSinceStartDate = DateTime.UtcNow - findUser.ExpiresAt;
                int daysSinceStartDate = timeSinceStartDate.Days;
                if (daysSinceStartDate >= 3)
                {
                    var emailVerifyToken = CreateRandomToken();
                    findUser.VerificationCode = emailVerifyToken;
                    findUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
                    await _userRepository.SaveChangesAsync();
                    _notificationService.SendVerificationEmail(findUser.Email, "User", emailVerifyToken);

                    Console.WriteLine("more than 3 days");
                    throw new RestException(HttpStatusCode.Forbidden, ResponseMessages.UserEmailNotVerified);
                }
                else
                {
                    Console.WriteLine("less than 3 days");
                    throw new RestException(HttpStatusCode.Forbidden, ResponseMessages.UserEmailNotVerified);
                }
            }

            var userResponse = _mapper.Map<GetUserDto>(findUser);

            return new SuccessResponse<GetUserDto>
            {
                Data = userResponse,
                Code = 200,
                Message = responseMessage,
                ExtraInfo = extraIfo,
            };
        }

        //public async Task<SuccessResponse<UpdateUserDto>> UpdateUser(string email, UpdateUserDto model)
        //{
        //    var findUser = await _userRepository.FirstOrDefault(x => x.Email == email);

        //    if (findUser == null)
        //        throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

        //    Functions.UpdateProperties(findUser, model);

        //    await _userRepository.SaveChangesAsync();

        //    return new SuccessResponse<UpdateUserDto>
        //    {
        //        Data = null,
        //        code = 201,
        //        Message = ResponseMessages.CourseUpdated,
        //        ExtraInfo = "",

        //    };
        //}
        public async Task<SuccessResponse<string>> ForgotPassword(string email)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var resetToken = CreateRandomToken();

            var forgotModel = await _forgotRepository.FirstOrDefault(x => x.Email == email);

            var newForgot = new ForgotPassword
            {
                Email = email,
                UserId = findUser.Id,
                ForgotToken = resetToken,
                ExpiresAt = DateTime.UtcNow.ToUniversalTime().AddMinutes(5)
            };

            // Create forgot password for this user
            if (forgotModel == null)
            {
                await _forgotRepository.AddAsync(newForgot);
                await _forgotRepository.SaveChangesAsync();
            }
            else if (forgotModel is not null)
            {
                forgotModel.ForgotToken = resetToken;
                forgotModel.ExpiresAt = DateTime.UtcNow.ToUniversalTime().AddMinutes(5);
                _forgotRepository.Update(forgotModel);
                await _forgotRepository.SaveChangesAsync();
            }

            _notificationService.SendPasswordResetEmail(findUser.Email, "User", resetToken);

            return new SuccessResponse<string>
            {
                Code = 204,
                Message = ResponseMessages.ForgotPasswordLinkSent,
                ExtraInfo = "",
            };
        }
        public async Task<SuccessResponse<string>> ResetPassword(string email, string token, ResetPasswordDto model)
        {
            var findToken = await _forgotRepository.FirstOrDefault(x => x.Email == email && x.ForgotToken == token);

            if (findToken == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.TokenNotFound);

            if (findToken.ExpiresAt < DateTime.Now.ToUniversalTime())
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.ExpiredToken);

            var findUser = await _userRepository.FirstOrDefault(x => x.Email == email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            findUser.Password = hashPassword;
            findUser.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(findUser);
            await _userRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = ResponseMessages.ResetSuccessful,
                Code = 200,
                Message = ResponseMessages.ResetSuccessful,
                ExtraInfo = ""
            };
        }
        public async Task<SuccessResponse<string>> ChangePassword(ChangePasswordDto model)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == model.Email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, findUser.Password))
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InCorrectPassword);

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            model.NewPassword = hashPassword;

            //var newUser = _mapper.Map<User>(model);
            //_mapper.Map(model, findUser);
            findUser.Password = model.NewPassword;
            await _userRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = ResponseMessages.PasswordChangedSuccessful,
                Code = 200,
                Message = ResponseMessages.PasswordChangedSuccessful,
                ExtraInfo = ""
            };
        }
        public async Task<SuccessResponse<string>> VerifyEmail(string email, string token)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.VerificationCode == token && x.Email == email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            TimeSpan timeSinceStartDate = DateTime.UtcNow - findUser.ExpiresAt;
            int minsSinceStartDate = timeSinceStartDate.Minutes;

            Console.WriteLine($"expire in: {findUser.ExpiresAt} now: {DateTime.UtcNow} diff: {minsSinceStartDate.ToString()}");

            if(minsSinceStartDate > 10)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.ExpiredToken);

            //var userResponse = _mapper.Map<CreateUserDto>(findUser);
            findUser.IsEmailVerified = true;
            await _userRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = "Email is verified",
                Code = 200,
                Message = ResponseMessages.VerifiedEmail,
                ExtraInfo = "",
            };
        }
        public async Task<SuccessResponse<TokenDto>> RenewTokens(RefreshTokenDto model)
        {
            var tokens = await _jwtService.RenewTokens(model);
            if (tokens == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidRefreshToken);
            }

            return new SuccessResponse<TokenDto>
            {
                Data = tokens,
                Code = 200,
                Message = ResponseMessages.RenewedToken,
                ExtraInfo = "",
            };
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        private async Task InsertRefreshToken(Guid userId, string refreshtoken)
        {
            var newRefreshToken = new RefreshToken
            {
                UserId = userId,
                Token = refreshtoken,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task<SuccessResponse<PaginatedResponse<GetUserDto>>> GetAllUsers(int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;
            var allProperties = await _userRepository.GetAllPaginatedAsync(skip, pageSize, p => p.CreatedAt);
            var totalCount = await _userRepository.CountAsync(_ => true);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            var propertiesResponse = _mapper.Map<IEnumerable<GetUserDto>>(allProperties);

            var paginatedResponse = new PaginatedResponse<GetUserDto>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetUserDto>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<string>> ResendVerifyEmail(string email)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            if(findUser.IsEmailVerified)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.VerifiedEmail);

            var emailVerifyToken = CreateRandomToken();
            findUser.VerificationCode = emailVerifyToken;
            findUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
            await _userRepository.SaveChangesAsync();
            _notificationService.SendVerificationEmail(findUser.Email, "User", emailVerifyToken);


            return new SuccessResponse<string>
            {
                Data = "New email verification mail has been sent to you",
                Code = 200,
                Message = ResponseMessages.MessageSent,
                ExtraInfo = "",
            };

        }

        public async Task<SuccessResponse<GetUserDto>> GetUserByPhoneOrEmail(string phoneOrEmail)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.PhoneNumber == phoneOrEmail || x.Email == phoneOrEmail) ?? throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var newUserResponse = _mapper.Map<GetUserDto>(findUser);

            return new SuccessResponse<GetUserDto>
            {
                Data = newUserResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<GetUserDto>> BlockingUser(string email, bool blockState)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == email);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var allProperties = await _propertyRepository.FindAsync(X => X.CreatorEmail == email);
            if (allProperties != null){
                for (int i = 0; i < allProperties.Count(); i++)
                {
                    var property = allProperties.ElementAt(i);
                    property.IsBlocked = blockState;
                    await _propertyRepository.SaveChangesAsync();
                }
            }

            findUser.IsBlocked = blockState;
            await _userRepository.SaveChangesAsync();

            var newUserResponse = _mapper.Map<GetUserDto>(findUser);

            return new SuccessResponse<GetUserDto>
            {
                Data = newUserResponse,
                Code = 201,
                Message = ResponseMessages.BlockStatusChanged,
                ExtraInfo = "",
            };
        }
    }
}
