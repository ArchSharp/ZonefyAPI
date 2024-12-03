using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IS3Service : IAutoDependencyService
    {
        Task<List<string>> UploadFileAsync(List<IFormFile> files);
        Task<Stream> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}
