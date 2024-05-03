namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class HotelOrderInitDTO
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNum { get; set; }
        public string? RoomNum { get; set; }
        public int RoomNumSuggest { get; set; }
    }
}
