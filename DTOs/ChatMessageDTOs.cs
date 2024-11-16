using System.ComponentModel.DataAnnotations;

namespace ZonefyDotnet.DTOs
{
    public class SendMessageRequestDTO
    {
        [Required]
        public Guid PropertyId { get; set; }
        [Required] 
        public string SenderEmail { get; set; }
        [Required] 
        public string ReceiverEmail { get; set; }
        [Required] 
        public string Content { get; set; }
    }

    public class GetChatMessagesDTO
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string ChatIdentifier { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
