using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IGoogleDriveService : IAutoDependencyService
    {
        Task<List<string>> UploadFileAsync(List<IFormFile> files);
        Task<string> DeleteFileAsync(string fileId);
        Task<GoogleDriveFile> GetFileAsync(string fileId);
    }
}
