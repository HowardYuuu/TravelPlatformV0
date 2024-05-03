namespace prjTravelPlatform_release.Areas.Customer.Model.Hotel
{
    public class HotelFilterDTO
    {
        public List<string>? Facilities { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set;}
       
        public List<decimal>? Score { get; set;}
        public List<int>? Rank { get; set; }
    }
}
