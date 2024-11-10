using System.ComponentModel.DataAnnotations;

namespace ZonefyDotnet.Entities
{
    public class ForgotPassword
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string ForgotToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
