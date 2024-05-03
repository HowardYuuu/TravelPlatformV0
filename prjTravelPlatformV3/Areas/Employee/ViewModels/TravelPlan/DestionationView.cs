using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class DestionationView
	{
		public string? FDestinationId { get; set; }
		public string? FDestinationName { get; set; }
		public string? FDestinationContent { get; set; }
		public string? Ftype { get; set; }//資料庫是int
		public string? FAreaName { get; set; }//資料庫是int
		public int? FPrice { get; set; }
		public int? FStock { get; set; }
		public string? FState { get; set; }
		public string? FAddress { get; set; }
		public string? FPriority { get; set; }
	}
}
