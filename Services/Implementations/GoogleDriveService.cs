﻿using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using System.Net;
using ZonefyDotnet.Common;
using ZonefyDotnet.DTOs;
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

        public async Task<string> DeleteFileAsync(string fileId)
        {
           try
            {
                await _driveService.Files.Delete(fileId).ExecuteAsync();

                return $"File with ID {fileId} has been deleted.";
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, $"Error deleting file with ID {fileId}: {ex.Message}");
            }
        }

        public async Task<GoogleDriveFile> GetFileAsync(string fileId)
        {
            try
            {
                // Fetch metadata for MIME type and name
                var fileRequest = _driveService.Files.Get(fileId);
                fileRequest.Fields = "mimeType, name";

                var fileMetadata = await fileRequest.ExecuteAsync();
                if (fileMetadata == null)
                {
                    throw new FileNotFoundException($"File with ID {fileId} not found.");
                }

                // Fetch file content
                var stream = new MemoryStream();
                await fileRequest.DownloadAsync(stream);
                stream.Position = 0;

                return new GoogleDriveFile
                {
                    FileName = fileMetadata.Name,
                    MimeType = fileMetadata.MimeType,
                    Stream = stream
                };
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"File with ID {fileId} not found on Google Drive.");
            }
        }

        public async Task<List<string>> UploadFileAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                throw new RestException(HttpStatusCode.BadRequest, "No files uploaded.");
            }

            var uploadedFileIds = new List<string>();

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
                            Parents = new List<string> { "1eNBmN01prburcB9S2wtVVkJHC5W3jJcw" }  // Specify the Google Drive folder ID if needed
                        };

                        var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                        request.Fields = "id";

                        var result = await request.UploadAsync();

                        if (result.Status == UploadStatus.Failed)
                        {
                            throw new Exception($"Error uploading file {file.FileName}: {result.Exception.Message}");
                        }

                        // Add the file ID to the list
                        uploadedFileIds.Add(request.ResponseBody.Id);

                        // Set file permissions to make it publicly accessible
                        var permission = new Google.Apis.Drive.v3.Data.Permission
                        {
                            Role = "reader",  // Read-only permission
                            Type = "anyone"   // Accessible by anyone with the link
                        };

                        await _driveService.Permissions.Create(permission, request.ResponseBody.Id).ExecuteAsync();
                    }
                }

                return  uploadedFileIds;
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
