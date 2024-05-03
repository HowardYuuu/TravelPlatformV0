using prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels;

namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class HotelSearchResDTO
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<HotelCard>? HotelCards { get; set; }
    }
}
