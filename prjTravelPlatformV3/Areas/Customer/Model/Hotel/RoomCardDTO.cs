using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class RoomCardDTO
    {
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public List<string>? RooomFacilities { get; set; }
        public string? RoomImg { get; set; }
        public string? RoomPrice { get; set; }
        public string? Message { get; set; }
        public int? MessageValue { get; set; }
        public int? RoomCount { get; set; }
        public int? BedNum { get; set; }
        public string? BedType { get; set; }
        public int? MaxCapacity { get; set; }
        public string? RoomCountMsg { get; set; }
        
    }
}
