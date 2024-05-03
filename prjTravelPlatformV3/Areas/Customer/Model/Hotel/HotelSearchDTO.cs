namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class HotelSearchDTO
    {
        public string? Destination { get; set; }
        public string? Category { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNum { get; set; }
        public string? RoomNum { get; set; }
        public string? SortBy { get; set; }
        public string? SortType { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public List<string>? FilterList { get; set; }
        public HotelFilterDTO? FilterObj { get; set; }
    }
}
