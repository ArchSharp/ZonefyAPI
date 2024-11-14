using AutoMapper;
using System.Net;
using ZonefyDotnet.Common;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Repositories.Interfaces;
using ZonefyDotnet.Services.Interfaces;
namespace ZonefyDotnet.Services.Implementations
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public ChatMessageService(
            IRepository<User> userRepository,
            IMapper mapper,
            INotificationService notificationService,
            IRepository<ChatMessage> chatMessageRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task<SuccessResponse<string>> DeleteChatMessages(string chatIdentifier)
        {
            // Find all matches for email patterns
            var matches = chatIdentifier.Split(" ");

            string email1, email2;
            if (matches.Length == 2)
            {
                email1 = matches[0];
                email2 = matches[1];
                //Console.WriteLine($"First email: {email1}");
                //Console.WriteLine($"Second email: {email2}");
            }
            else
            {
                //Console.WriteLine($"Could not split into exactly two emails. {matches[0]}");
                throw new RestException(HttpStatusCode.BadRequest, "invalid chat identifier");
            }

            var findAllChats = (await _chatMessageRepository.FindAsync(x => x.ChatIdentifier == email1 + email2 || x.ChatIdentifier == email2 + email1)).ToList();

            if (findAllChats == null || !findAllChats.Any())
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ChatMessageNotFound);

            // Remove each chat message individually
            foreach (var chat in findAllChats)
            {
                _chatMessageRepository.Remove(chat);
            }

            // Save changes once after all removals
            await _chatMessageRepository.SaveChangesAsync();


            return new SuccessResponse<string>
            {
                Data = "Chat messages deleted",
                Code = 200,
                Message = ResponseMessages.ChatMessageDeleted,
                ExtraInfo = ""
            };
        }

        public async Task<SuccessResponse<string>> DeleteSingleChatMessage(Guid messageId)
        {
            var findMessage = await _chatMessageRepository.FirstOrDefault(x => x.Id == messageId);

            if (findMessage == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ChatMessageNotFound);

            _chatMessageRepository.Remove(findMessage);
            await _chatMessageRepository.SaveChangesAsync();


            return new SuccessResponse<string>
            {
                Data = "Chat message deleted",
                Code = 200,
                Message = ResponseMessages.ChatMessageDeleted,
                ExtraInfo = ""
            };
        }

        public async Task<SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>> GetAllChatMessages(int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;
            var allProperties = await _chatMessageRepository.GetAllPaginatedAsync(skip, pageSize, p => p.CreatedAt);
            var totalCount = await _chatMessageRepository.CountAsync(_ => true);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            var propertiesResponse = _mapper.Map<IEnumerable<GetChatMessagesDTO>>(allProperties);

            var paginatedResponse = new PaginatedResponse<GetChatMessagesDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>> GetPaginatedChatMessages(string sender, string receiver, int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;

            // Retrieve the total count of properties for the given email for pagination metadata
            int totalCount = await _chatMessageRepository.CountAsync(x => x.ChatIdentifier == sender + receiver || x.ChatIdentifier == receiver + sender);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            // Fetch the paginated data
            var allChatMessages = await _chatMessageRepository.FindPaginatedAsync(x => x.ChatIdentifier == sender + receiver || x.ChatIdentifier == sender + receiver, skip, pageSize, p => p.CreatedAt);

            //if(allChatMessages == null) 
            //    throw new RestException(HttpStatusCode.NotFound, ResponseMessages)

            // Map the properties to DTOs
            var propertiesResponse = _mapper.Map<IEnumerable<GetChatMessagesDTO>>(allChatMessages);

            // Wrap in paginated response
            var paginatedResponse = new PaginatedResponse<GetChatMessagesDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetChatMessagesDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = ""
            };
        }

        public async Task<SuccessResponse<string>> SendMessage(SendMessageRequestDTO request)
        {
            var findUSender = await _userRepository.FirstOrDefault(x => x.Email == request.SenderEmail);

            if (findUSender == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.SenderNotFound);

            var findReceiver = await _userRepository.FirstOrDefault(x => x.Email == request.ReceiverEmail);

            if (findReceiver == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ReceiverNotFound);

            if (findUSender.Email == findReceiver.Email)
                throw new RestException(HttpStatusCode.BadRequest, "You cannot send message to yourself");

            var newChat = _mapper.Map<ChatMessage>(request);
            newChat.ChatIdentifier = request.SenderEmail.Trim() + request.ReceiverEmail.Trim();
            newChat.SenderId = findUSender.Id;
            newChat.ReceiverId = findReceiver.Id;

            await _chatMessageRepository.AddAsync(newChat);
            await _chatMessageRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = "Message sent",
                Code = 201,
                Message = ResponseMessages.MessageSent,
                ExtraInfo = "",
            };
        }
    }
}
