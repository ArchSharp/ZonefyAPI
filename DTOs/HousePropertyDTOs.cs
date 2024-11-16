using System.ComponentModel.DataAnnotations;

namespace ZonefyDotnet.DTOs
{
    public class CreateHousePropertyDTO
    {
        [Required]
        [EmailAddress]
        public string CreatorEmail { get; set; }
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string OwnerPhoneNumber { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public double PropertyPrice { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        [Required]
        public string PropertyType { get; set; }
        [Required]
        //public List<string>?  PropertyImageUrl { get; set; }
        public string PropertyLocation { get; set; }
        [Required]
        public int ToiletNumber { get; set; }
        [Required]
        public int ParkingLot { get; set; }
    }

    public class GetHousePropertyDTO
    {
        public Guid Id { get; set; }
        public string CreatorEmail { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string PropertyName { get; set; }
        public double PropertyPrice { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyType { get; set; }
        public List<string> PropertyImageUrl { get; set; }
        public string PropertyLocation { get; set; }
        public int ToiletNumber { get; set; }
        public int ParkingLot { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateHousePropertyDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string CreatorEmail { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string PropertyName { get; set; }
        public double PropertyPrice { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyType { get; set; }
        public List<string> PropertyImageUrl { get; set; }
        public string PropertyLocation { get; set; }
        public int ToiletNumber { get; set; }
        public int ParkingLot { get; set; }
    }
}
