using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels
{
    public class HotelListViewModel
    {
        public List<HotelCard>? HotelCards { get; set; }

        public HotelFilterModel? HotelFilter { get; set; }
    }
}
