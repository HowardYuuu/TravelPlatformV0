using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelDetailViewModel
    {
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public string? HotelEngName { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Intro { get; set; }
        public int? Rank { get; set; }
        public decimal? Score { get; set; }
        public List<HotelCommentView>? HotelComments { get; set; }
        public List<string>? HotelFacilities { get; set; }
        public List<string>? HotelsImgs { get; set; }
        public List<string>? RoomFacilities { get; set; }
        public List<string>? RoomImgs { get; set; }
    }
}
