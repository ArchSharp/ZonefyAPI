namespace ZonefyDotnet.Entities
{
    public class PropertyStatistics
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string CreatorEmail { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
