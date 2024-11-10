using Autofac.Features.OwnedInstances;
using System.ComponentModel.DataAnnotations;

namespace ZonefyDotnet.Entities
{
    public class HouseProperty
    {
        [Key]
        public Guid Id { get; set; }
        public string CreatorEmail { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string PropertyName { get; set; }
        public double PropertyPrice { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyType { get; set; }
        public List<string>? PropertyImageUrl { get; set; }
        public string PropertyLocation { get; set; }
        public int ToiletNumber { get; set; }
        public int ParkingLot { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
