namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelOrderViewModel
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string? HotelName { get; set; }
        public string? RoomTypeName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNum { get; set; }
        public string? RoomNum { get; set; }
        public string? Price { get; set; }
        public string? TotalPrice { get; set; }
        public string? TotalPriceAddTax { get; set; }
        public string? RoomImg { get; set; }
        public string? BedType { get; set; }
        public string? BedNum { get; set; }
        public string? Tax { get; set; }
        public List<int>? RoomIds { get; set; }
        public List<string>? RoomNumbers { get; set; }
        public int DayCount { get; set; }
    }
}
