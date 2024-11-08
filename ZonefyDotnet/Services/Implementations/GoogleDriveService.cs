using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using ZonefyDotnet.Common;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Repositories.Interfaces;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<HouseProperty> _propertyRepository;

        public GoogleDriveService(DriveService driveService,
            IRepository<User> userRepository,
            IRepository<HouseProperty> propertyRepository)
        {
            _driveService = driveService;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<SuccessResponse<string>> DeleteFileAsync(string fileId, string userEmail, Guid propertyId)
        {
            var findUser = await _userRepository.FirstOrDefault(x=> x.Email == userEmail);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var findProperty = await _propertyRepository.FirstOrDefault(x => x.Id == propertyId);

            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            try
            {
                await _driveService.Files.Delete(fileId).ExecuteAsync();
                
                findProperty.PropertyImageUrl.Remove(fileId);
                await _propertyRepository.SaveChangesAsync();
                

                return new SuccessResponse<string>
                {
                    Data = $"File with ID {fileId} has been deleted.",
                    Code = 200,
                    Message = ResponseMessages.ImageDeleted,
                    ExtraInfo = "File deleted successfully."
                };
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, $"Error deleting file with ID {fileId}: {ex.Message}");
            }
        }

        public async Task<SuccessResponse<List<string>>> UploadFileAsync(List<IFormFile> files, Guid propertyId)
        {
            if (files == null || files.Count == 0)
            {
                throw new RestException(HttpStatusCode.BadRequest, "No files uploaded.");
            }

            var uploadedFileIds = new List<string>();

            var findProperty = await _propertyRepository.FirstOrDefault(x=> x.Id == propertyId);

            if(findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            try
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using var stream = file.OpenReadStream();
                        var fileMetadata = new Google.Apis.Drive.v3.Data.File
                        {
                            Name = file.FileName,  // Use the file's original name or customize as needed
                            Parents = new List<string> { "1fVdMQDJdCOsYtINW13rG0sRVudkjmRTV" }  // Specify the Google Drive folder ID if needed
                        };

                        var request = _driveService.Files.Create(fileMetadata, stream, "image/jpeg");
                        request.Fields = "id";

                        var result = await request.UploadAsync();

                        if (result.Status == UploadStatus.Failed)
                        {
                            throw new Exception($"Error uploading file {file.FileName}: {result.Exception.Message}");
                        }

                        // Add the file ID to the list
                        uploadedFileIds.Add(request.ResponseBody.Id);
                    }
                }

                if (uploadedFileIds != null && uploadedFileIds.Count > 0)
                {
                    findProperty.PropertyImageUrl ??= new List<string>();
                    findProperty.PropertyImageUrl.AddRange(uploadedFileIds);
                    await _propertyRepository.SaveChangesAsync();
                }

                return new SuccessResponse<List<string>>
                {
                    Data = uploadedFileIds,
                    Code = 200,
                    Message = ResponseMessages.ImageUploaded,
                    ExtraInfo = "All files uploaded successfully."
                };
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


    }
}
