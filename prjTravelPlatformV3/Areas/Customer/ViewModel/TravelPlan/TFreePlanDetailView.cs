namespace prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan
{
	public class TFreePlanDetailView
	{
		public int FdestinationDetailId { get; set; }

		public int? FcustomerId { get; set; }

		public string FdestinationId { get; set; }
		public string FdestionationName { get; set; }

		public int? FtravelDay { get; set; }
		public string FreeId {  get; set; }

		public int? FPrice { get; set; }
		public TimeSpan? FdestionationTime { get; set; }

		public bool? FdestionationState { get; set; }
	}
}
