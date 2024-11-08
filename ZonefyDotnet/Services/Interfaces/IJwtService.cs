using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IJwtService : IAutoDependencyService
    {
        string CreateJwtToken(User user);
        Task<string> CreateRefreshToken();
        Task<TokenDto> RenewTokens(RefreshTokenDto refreshToken);
    }
}
