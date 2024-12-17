using Application.Helpers;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IChatMessageService : IAutoDependencyService
    {
        Task<SuccessResponse<string>> SendMessage(SendMessageRequestDTO request);
        Task<SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>> GetAllChatMessages(int pageNumber);
        Task<SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>> GetPropertyUserMessages(string sender, string receiver, Guid propertyId, int pageNumber);
        Task<SuccessResponse<string>> DeleteChatMessages(string chatIdentifier);
        Task<SuccessResponse<string>> DeleteSingleChatMessage(Guid messageId);
        Task<SuccessResponse<string>> UpdateChatMessageReadStatus(Guid messageId, Guid userId);
    }
}
