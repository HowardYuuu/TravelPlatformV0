namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelFilterModel
    {
        public string? Destination { get; set; }
        public string? Category { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set;}
        public int? PersonNum { get; set; }
        public int? RoomNum { get; set; }
        public List<HotelSuggestViewModel>? HotelSuggests { get; set; }
    }
    public class HotelSuggestViewModel
    {
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public string? HotelImg { get; set; }
        public string? Address { get; set; }
        public decimal? Score { get; set; }
        public string? ScoreStr { get; set;}
        public decimal? Price { get; set; }

    }
}
