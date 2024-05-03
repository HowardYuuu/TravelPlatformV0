namespace prjTravelPlatform_release.Models
{
    public class FlightSearchDTO
    {
        public int fId {  get; set; }
        public int AirlineId { get; set; }
        public string? Departure { get; set; }
        public string? Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Duration { get; set; }
        public decimal? TicketPrice { get; set; }

    }
}
