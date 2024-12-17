using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Implementations;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/ChatMessage")]
    [Authorize]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageService _chatMessageService;

        /// <summary>
        /// This class represents a controller for chat-related actions.
        /// </summary>
        public ChatMessageController(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        /// <summary>
        /// Endpoint to create a send chat message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("Send")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> CreateUser(SendMessageRequestDTO model)
        {

            var response = await _chatMessageService.SendMessage(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get chat messages between property owner and interested renter/purchaser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="propertyId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetByUserIdsPropId")]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>), 200)]
        public async Task<IActionResult> GetChatMessages(string sender, string receiver, Guid propertyId, int pageNumber)
        {

            var response = await _chatMessageService.GetPropertyUserMessages(sender, receiver, propertyId, pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update chat message read status
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("UpdateChatMessageReadStatus")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 201)]
        public async Task<IActionResult> UpdateChatMessageReadStatus(Guid messageId, Guid userId)
        {

            var response = await _chatMessageService.UpdateChatMessageReadStatus(messageId, userId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all chat messages
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetAll")]
        [ProducesResponseType(typeof(SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>), 200)]
        public async Task<IActionResult> GetAllChatMessages(int pageNumber)
        {
            var response = await _chatMessageService.GetAllChatMessages(pageNumber);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete single chat message
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("DeleteSingle")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteSingleChatMessage(Guid messageId)
        {

            var response = await _chatMessageService.DeleteSingleChatMessage(messageId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete all chat messages between two users
        /// </summary>
        /// <param name="chatIdentifier"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Route("DeleteChatMessages")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> DeleteChatMessages(string chatIdentifier)
        {

            var response = await _chatMessageService.DeleteChatMessages(chatIdentifier);

            return Ok(response);
        }
    }
}
