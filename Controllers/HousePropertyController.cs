using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Implementations;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/HouseProperty")]    
    public class HousePropertyController : ControllerBase
    {
        private readonly IHousePropertyService _propertyService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly RedisService _redisService;
        /// <summary>
        /// This class represents a controller for user-related actions.
        /// </summary>
        public HousePropertyController(
            IHousePropertyService propertyService,
            IGoogleDriveService googleDriveService,
             RedisService redisService)
        {
            _propertyService = propertyService;
            _googleDriveService = googleDriveService;
            _redisService = redisService;
        }

        /// <summary>
        /// Endpoint to create a new property
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("Create")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<GetHousePropertyDTO>), 201)]
        public async Task<IActionResult> CreateUser(CreateHousePropertyDTO model)
        {

            var response = await _propertyService.CreateHouseProperty(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all properties
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetAll")]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>), 200)]
        public async Task<IActionResult> GetAllHouseProperties(int pageNumber)
        {

            var response = await _propertyService.GetAllHouseProperties(pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all properties by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetAllByEmail")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>), 200)]
        public async Task<IActionResult> GetAllHousePropertiesByEmail(string email, int pageNumber)
        {

            var response = await _propertyService.GetAllHousePropertiesByEmail(email, pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all property statistics by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetPropertyStatisticsById")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>), 200)]
        public async Task<IActionResult> GetPropertyStatisticsById(Guid id, int pageNumber)
        {

            var response = await _propertyService.GetPropertyStatisticsById(id, pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all user property statistics by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetAllUserPropertyStatisticsByEmail")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>), 200)]
        public async Task<IActionResult> GetAllUserPropertyStatisticsByEmail(string email, int pageNumber)
        {

            var response = await _propertyService.GetAllUserPropertyStatisticsByEmail(email, pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Fetches a file from Google Drive by file ID.
        /// </summary>
        /// <param name="fileId">The ID of the file to fetch.</param>
        /// <returns>The file as a downloadable response.</returns>
        [HttpGet("Files/{fileId}")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFile(string fileId)
        {
            try
            {
                // Check if the file is cached in Redis first
                var cachedFile = await _redisService.GetCacheAsync(fileId);
                if (cachedFile != null)
                {
                    Console.WriteLine("Found image in redis cache");
                    // If file is found in cache, return it as an image
                    var fileCacheData = JsonConvert.DeserializeObject<FileCacheData>(cachedFile);
                    byte[] fileBytes = Convert.FromBase64String(fileCacheData.FileContent);
                    var mimeType = GetMimeType(fileBytes); // Dynamically determine MIME type
                    return File(fileBytes, mimeType, fileCacheData.FileName); // Include the file name
                }

                // If the file is not in the cache, fetch it from Google Drive
                var file = await _googleDriveService.GetFileAsync(fileId);

                // Cache the file in Redis (store the file as a base64 string for easy caching)
                using (var memoryStream = new MemoryStream())
                {
                    await file.Stream.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    string base64File = Convert.ToBase64String(fileBytes);

                    // Create an object with both file content and filename
                    var fileCacheData = new FileCacheData
                    {
                        FileContent = base64File,
                        FileName = file.FileName
                    };

                    // Store the file in Redis with filename (TTL can be adjusted)
                    var cacheData = JsonConvert.SerializeObject(fileCacheData);
                    await _redisService.SetCacheAsync(fileId, cacheData, TimeSpan.FromHours(1));


                    // Return the file as response
                    var mimeType = GetMimeType(fileBytes);
                    return File(fileBytes, mimeType, file.FileName);
                }
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { Message = "File not found on Google Drive." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }


        /// <summary>
        /// Endpoint to update house properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut()]
        [Route("Update")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<GetHousePropertyDTO>), 200)]
        public async Task<IActionResult> UpdateHouseProperty(UpdateHousePropertyDTO model)
        {

            var response = await _propertyService.UpdateHouseProperty(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("Delete")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteHouseProperty(Guid id)
        {

            var response = await _propertyService.DeleteHouseProperty(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to upload property images
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("UploadImage")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, Guid propertyId)
        {

            var response = await _propertyService.UploadPropertyImages(files, propertyId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete property images
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="fileId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("DeleteImage")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteFileAsync(string fileId, string userEmail, Guid propertyId)
        {

            var response = await _propertyService.DeletePropertyImageAsync(fileId, userEmail, propertyId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to upload property images
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="fileId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("DownloadImage")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DownloadImage(string fileId, string userEmail, Guid propertyId)
        {

            var fileStream = await _propertyService.DownloadPropertyImageAsync(fileId, userEmail, propertyId);

            return File(fileStream, "application/octet-stream", fileId);
        }

        private string GetMimeType(byte[] fileBytes)
        {
            // Check the file's signature (magic bytes) to determine the MIME type
            var header = fileBytes.Take(4).ToArray();

            if (header.SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }))
            {
                return "image/jpeg";  // JPEG
            }
            if (header.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
            {
                return "image/png";  // PNG
            }
            if (header.SequenceEqual(new byte[] { 0x47, 0x49, 0x46, 0x38 }))
            {
                return "image/gif";  // GIF
            }
            if (header.SequenceEqual(new byte[] { 0x42, 0x4D }))
            {
                return "image/bmp";  // BMP
            }
            if (header.SequenceEqual(new byte[] { 0x49, 0x20, 0x49, 0x49 }))
            {
                return "image/tiff";  // TIFF
            }

            return "application/octet-stream";  // Default for unknown file types
        }

    }
}

