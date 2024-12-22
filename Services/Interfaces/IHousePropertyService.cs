using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IHousePropertyService : IAutoDependencyService
    {
        Task<SuccessResponse<GetHousePropertyDTO>> CreateHouseProperty(CreateHousePropertyDTO model);
        Task<SuccessResponse<GetHousePropertyDTO>> GetHousePropertyByName(string name);
        Task<SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>> GetAllHouseProperties(int pageNumber);
        Task<SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>> GetAllHousePropertiesByEmailOrPhone(string emailOrPhone, int pageNumber);
        Task<SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>> SearchAllHouseProperties(string locationOrPostCode, DateTime checkIn, DateTime checkOut, string propertyType, int pageNumber);
        Task<SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>> GetPropertyStatisticsById(Guid id, int pageNumber);
        Task<SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>> GetAllUserPropertyStatisticsByEmail(string email, int pageNumber);
        Task<SuccessResponse<GetHousePropertyDTO>> UpdateHouseProperty(UpdateHousePropertyDTO model);
        Task<SuccessResponse<string>> DeleteHouseProperty(Guid id);
        Task<SuccessResponse<List<string>>> UploadPropertyImages(List<IFormFile> files, Guid propertyId);
        Task<SuccessResponse<string>> DeletePropertyImageAsync(string fileId, string userEmail, Guid propertyId);
        Task<Stream> DownloadPropertyImageAsync(string fileId, string userEmail, Guid propertyId);
        Task<SuccessResponse<GetHousePropertyDTO>> BlockingProperty(string email, Guid propId, bool blockState);
    }
}
