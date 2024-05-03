namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class HotelOrderDTO
    {
        public int CustomerId { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNum { get; set; }
        public string? RoomNum { get; set; }
        public string? Price { get; set; }
        public string? TotalPriceAddTax { get; set; }
        public int? PaymentId { get; set; } = 6;
        public int? PaymentStatus { get; set; } = 3;
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public DateTime? PayDate { get; set; } = null;


        public List<string> RoomIds { get; set; }
        public List<string> RoomNumbers { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
