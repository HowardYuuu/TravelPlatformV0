﻿using System.ComponentModel;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class TravelPlanOrderEditView
	{
		public string? FOrderId { get; set; }

		[DisplayName("用戶名稱")]
		public string? FName { get; set; }
		public int? FMemeberId { get; set; }

		[DisplayName("購買方案")]
		public string? FPlanId { get; set; }

		[DisplayName("用戶信箱")]
		public string? FEmail { get; set; }

		[DisplayName("用戶電話")]
		public string? FPhone { get; set; }

		[DisplayName("購買數量")]
		public int? FQty { get; set; }

		[DisplayName("行程日期")]
		public string? FOrderDate { get; set; }

		[DisplayName("付款方式")]
		public string? FPayMethod { get; set; }

		[DisplayName("優惠券")]
		public string? FCoupomName { get; set; }

		[DisplayName("訂單金額")]
		public int? FPrice { get; set; }
		//[DisplayName("訂單金額")]
		//public int? FAfterPrice { get; set; }
	}
}
