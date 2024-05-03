namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class RoomSearchDTO
    {
        public int HotelId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNum { get; set; }
        public string? RoomNum { get; set; }
    }
}
