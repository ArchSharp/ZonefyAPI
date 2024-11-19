using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IGoogleDriveService : IAutoDependencyService
    {
        Task<SuccessResponse<List<string>>> UploadFileAsync(List<IFormFile> files, Guid propertyId);
        Task<SuccessResponse<string>> DeleteFileAsync(string fileId, string userEmail, Guid propertyId);
        Task<GoogleDriveFile> GetFileAsync(string fileId);
    }
}
