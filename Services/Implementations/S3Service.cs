using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using ZonefyDotnet.Common;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            var awsOptions = configuration.GetSection("AWS");
            _bucketName = awsOptions["S3BucketName"];

            _s3Client = new AmazonS3Client(
                awsOptions["AccessKey"],
                awsOptions["SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(awsOptions["Region"])
            );
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
                        using var fileStream = file.OpenReadStream();
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = fileStream,
                            Key = file.FileName,
                            BucketName = _bucketName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        using var transferUtility = new TransferUtility(_s3Client);
                        await transferUtility.UploadAsync(request);

                        string img_url = $"https://{_bucketName}.s3.amazonaws.com/{file.FileName}";
                        uploadedFileIds.Add(img_url);
                        //return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
                    }
                }

                return uploadedFileIds;
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, fileName);
            return response.ResponseStream;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            });
        }
    }
}
