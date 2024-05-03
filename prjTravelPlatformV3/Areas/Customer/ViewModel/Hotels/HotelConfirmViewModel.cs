namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelConfirmViewModel
    {
        public string? HotelOrderId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? HotelId { get; set; }
        public int? RoomTypeId { get; set; }
        public string? HotelName { get; set; }
        public string? HotelAddress { get; set; }
        public string? HotelPhone { get; set; }
        public string? RoomTypeName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PersonNum { get; set; }
        public int? RoomNum { get; set; }
        public string? Price { get; set; }
        public string? TotalPrice { get; set; }
        public string? PaymentWay { get; set; } = "Line Pay";
        public string? PaymentStatus { get; set; } = "未付款";
        public string? OrderDate { get; set; }
        public string? PayDate { get; set; }
        public List<string>? RoomIds { get; set; }
        public List<string>? RoomsNumbers { get; set; }
        public string? RoomNumberStr { get; set; }
        public string? RoomImg { get; set; }
        public int? DayCount { get; set; }
        public string? Tax { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? BedNum { get; set; }
        public string? BedType { get; set; }
    }
}
