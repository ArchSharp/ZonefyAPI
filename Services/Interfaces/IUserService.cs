using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IUserService : IAutoDependencyService
    {
        Task<SuccessResponse<GetUserDto>> CreateUser(CreateUserDTO model);
        Task<SuccessResponse<string>> DeleteUser(string email);
        Task<SuccessResponse<GetUserDto>> Login(LoginUserDto model);
        Task<SuccessResponse<string>> ForgotPassword(string email);
        Task<SuccessResponse<string>> ResetPassword(string email, string token, ResetPasswordDto model);
        Task<SuccessResponse<string>> ChangePassword(ChangePasswordDto model);
        Task<SuccessResponse<string>> VerifyEmail(string email, string token);
        //Task<SuccessResponse<UpdateUserDto>> UpdateUser(string email, UpdateUserDto model);
        Task<SuccessResponse<TokenDto>> RenewTokens(RefreshTokenDto model);
        Task<SuccessResponse<PaginatedResponse<GetUserDto>>> GetAllUsers(int pageNumber);
        Task<SuccessResponse<string>> ResendVerifyEmail(string email);
    }
}
