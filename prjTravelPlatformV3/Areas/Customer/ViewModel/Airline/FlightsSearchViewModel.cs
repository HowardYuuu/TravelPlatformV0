using prjTravelPlatformV3.Models;
using System.Globalization;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Airline
{
    public class FlightsSearchViewModel
    {
        public string Departure {  get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? Class {  get; set; }
        public int? ticketQty { get; set; }

    }
}
