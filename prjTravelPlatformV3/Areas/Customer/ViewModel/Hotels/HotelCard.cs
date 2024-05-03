using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelCard
    {
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public string? HotelEngName { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Price { get; set; }
        public List<string>? HotelsFacilitiesName { get; set; }
        public List<string>? HotelsImagies { get; set; }
        public string? Score { get; set; }
        public string? ScoreStr { get; set; }
        public int? CommentCount { get; set; }
        public int? Rank { get; set; }
        public bool? isCollected { get; set; } = false;

    }
}
