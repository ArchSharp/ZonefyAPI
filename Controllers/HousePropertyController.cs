using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    [Authorize]
    public class HousePropertyController : ControllerBase
    {
        private readonly IHousePropertyService _propertyService;
        private readonly IGoogleDriveService _googleDriveService;
        /// <summary>
        /// This class represents a controller for user-related actions.
        /// </summary>
        public HousePropertyController(
            IHousePropertyService propertyService,
            IGoogleDriveService googleDriveService)
        {
            _propertyService = propertyService;
            _googleDriveService = googleDriveService;
        }

        /// <summary>
        /// Endpoint to create a new property
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("Create")]
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
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>), 200)]
        public async Task<IActionResult> GetAllHousePropertiesByEmail(string email, int pageNumber)
        {

            var response = await _propertyService.GetAllHousePropertiesByEmail(email, pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update house properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut()]
        [Route("Update")]
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
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, Guid propertyId)
        {

            var response = await _googleDriveService.UploadFileAsync(files, propertyId);

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
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteFileAsync(string fileId, string userEmail, Guid propertyId)
        {

            var response = await _googleDriveService.DeleteFileAsync(fileId, userEmail, propertyId);

            return Ok(response);
        }
    }
}

