namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class ScheduleView
	{
		public string FTravelId { get; set; }
		public int FTravelDay { get; set; }
		public string? FDestinationMorningName { get; set; }
		public string? FDestinationAfter { get; set; }
		public DateTime? FDestinationMorningTime { get; set; }
		public DateTime? FDestinationAfterTime { get; set; }
	}
}
