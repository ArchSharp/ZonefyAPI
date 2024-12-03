using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiVersion("1.0")]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// This class represents a controller for user-related actions.
        /// </summary>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Endpoint to verify new user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("VerifyEmail")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {

            var response = await _userService.VerifyEmail(email, token);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("Register")]
        [ProducesResponseType(typeof(SuccessResponse<GetUserDto>), 201)]
        public async Task<IActionResult> CreateUser(CreateUserDTO model)
        {

            var response = await _userService.CreateUser(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to login a returning user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("Login")]
        [ProducesResponseType(typeof(SuccessResponse<GetUserDto>), 200)]
        public async Task<IActionResult> LoginUser(LoginUserDto model)
        {

            var response = await _userService.Login(model);

            return Ok(response);
        }

        /// <summary>
        /// Forgot password enpoint
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("ForgotPassword")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 204)]
        public async Task<IActionResult> ForgotPassword(string email)
        {

            var response = await _userService.ForgotPassword(email);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to reset user password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("ResetPassword")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> ResetPassword(string email, string token, ResetPasswordDto model)
        {
            var response = await _userService.ResetPassword(email, token, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to change user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost()]
        [Route("ChangePassword")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var response = await _userService.ChangePassword(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a new user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("Delete")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteUser(string email)
        {

            var response = await _userService.DeleteUser(email);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to renew user tokens
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("RenewTokens")]
        [ProducesResponseType(typeof(SuccessResponse<TokenDto>), 200)]
        public async Task<IActionResult> RenewTokens(RefreshTokenDto model)
        {
            var response = await _userService.RenewTokens(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all users
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetAll")]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetUserDto>>), 200)]
        public async Task<IActionResult> GetAllUsers(int pageNumber)
        {

            var response = await _userService.GetAllUsers(pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to send new verify email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("ResendVerifyEmail")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> ResendVerifyEmail(string email)
        {

            var response = await _userService.ResendVerifyEmail(email);

            return Ok(response);
        }
    }
}
