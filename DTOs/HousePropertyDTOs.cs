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
        public int PostCode { get; set; }
        [Required]
        public int ToiletNumber { get; set; }
        [Required]
        public int ParkingLot { get; set; }
        [Required]
        public int Guests { get; set; }
        [Required]
        public double Dimension { get; set; }
        [Required]
        public DateTime CheckInTime { get; set; }
        [Required]
        public DateTime CheckOutTime { get; set; }
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
        public int PostCode { get; set; }
        public int ToiletNumber { get; set; }
        public int ParkingLot { get; set; }
        public int Guests { get; set; }
        public double Dimension { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
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
        public int PostCode { get; set; }
        public bool IsBlocked { get; set; }
        public int ToiletNumber { get; set; }
        public int ParkingLot { get; set; }
        public int Guests { get; set; }
        public double Dimension { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
    }

    public class GoogleDriveFile
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public MemoryStream Stream { get; set; }
    }

    public class FileCacheData
    {
        public string FileContent { get; set; }  // Base64 encoded file content
        public string FileName { get; set; }     // The name of the file
    }
}
