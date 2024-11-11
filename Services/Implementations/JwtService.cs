using Domain.Entities.Token;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Repositories.Interfaces;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    internal sealed class JwtService : IJwtService
    {
        private readonly JwtParameters _jwtParameters;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IRepository<User> _userRepository;

        public JwtService(IOptions<JwtParameters> jwtParameters, IRepository<RefreshToken> refreshToken, IRepository<User> userRepository)
        {
            _jwtParameters = jwtParameters.Value;
            _refreshTokenRepository = refreshToken;
            _userRepository = userRepository;
        }

        public string CreateJwtToken(User user)
        {
            var claims = new Claim[]
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email)
            };
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtParameters.SecretKey)),
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: _jwtParameters.Issuer,
                audience: _jwtParameters.Audience,
                claims: claims,
                null,
                expires: DateTime.UtcNow.AddMinutes(10),//.AddDays(2),//.AddHours(2),
                signingCredentials: signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
        public async Task<string> CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshtoken = Convert.ToBase64String(tokenBytes);

            var isRefreshTokenInDb = await _refreshTokenRepository.FirstOrDefault(x => x.Token == refreshtoken);
            if (isRefreshTokenInDb != null)
            {
                return await CreateRefreshToken();
            }

            return refreshtoken;
        }
        public async Task<TokenDto> RenewTokens(RefreshTokenDto refreshToken)
        {
            var userRefreshToken = await _refreshTokenRepository.FirstOrDefault(x => x.Token == refreshToken.Token &&
                                                                              x.ExpirationDate >= DateTime.UtcNow);
            if (userRefreshToken == null)
            {
                return null;
            }

            var findUser = await _userRepository.FirstOrDefault(x => x.Id == userRefreshToken.UserId);

            var newJwtToken = CreateJwtToken(findUser);
            var newRefreshToken = await CreateRefreshToken();

            userRefreshToken.Token = newRefreshToken;
            userRefreshToken.ExpirationDate = DateTime.UtcNow.AddDays(7);
            await _refreshTokenRepository.SaveChangesAsync();

            return new TokenDto
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
