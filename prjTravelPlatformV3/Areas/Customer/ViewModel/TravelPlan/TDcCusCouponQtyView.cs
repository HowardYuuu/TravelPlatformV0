namespace prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan
{
	public class TDcCusCouponQtyView
	{
		public int Fid { get; set; }

		public int FCustomerId { get; set; }

		public int FCouponId { get; set; }
		public string FCouponName {  get; set; }

		public DateTime? FStartDate { get; set; }

		public DateTime? FEndDate { get; set; }

		public bool? FUsed { get; set; }
	}
}
