namespace ZonefyDotnet.DTOs
{
    public class GetPropertyStatisticDTO
    {
        public Guid PropertyId { get; set; }
        public string CreatorEmail { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
