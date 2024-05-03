namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class TFreePlanDetailView
	{
		public int FdestinationDetailId { get; set; }

		public int? FcustomerId { get; set; }

		public string FdestinationId { get; set; }
		public string FdestionationName {  get; set; }

		public int? FtravelDay { get; set; }

		public TimeSpan? FdestionationTime { get; set; }

		public string FfreeId { get; set; }

		public bool? FdestionationState { get; set; }
	}
}
