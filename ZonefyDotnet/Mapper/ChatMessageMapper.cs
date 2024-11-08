using AutoMapper;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;

namespace ZonefyDotnet.Mapper
{
    public class ChatMessageMapper : Profile
    {
        public ChatMessageMapper()
        {
            CreateMap<SendMessageRequestDTO, ChatMessage>();
            CreateMap<ChatMessage, GetChatMessagesDTO>();
        }
    }
}
