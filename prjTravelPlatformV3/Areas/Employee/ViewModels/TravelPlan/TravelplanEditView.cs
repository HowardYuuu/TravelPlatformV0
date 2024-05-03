using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class TravelplanEditView
	{
		public string? FTravelId { get; set; }


		public bool? FState { get; set; }

		[Required(ErrorMessage = "方案名稱為必填")]
		[DisplayName("方案名稱")]
		public string? FTravelName { get; set; }

		[Required(ErrorMessage = "金額尚未填寫")]
		[RegularExpression(@"^\d+$", ErrorMessage = "價位僅能包含數字")]
		[DisplayName("價位(NT)")]
		public int? FPrice { get; set; }

		[Required(ErrorMessage = "數量尚未填寫")]
		[RegularExpression(@"^\d+$", ErrorMessage = "價位僅能包含數字")]
		[DisplayName("門票數量")]
		public int? FStock { get; set; }

		[Required(ErrorMessage = "景點區域為必選")]
		[DisplayName("區域")]
		public int? FAreaId { get; set; }

		[Required(ErrorMessage = "景點類型為必選")]
		[DisplayName("類型")]
		public int? FTypeId { get; set; }

		[DisplayName("方案敘述")]
		public string? FTravelContent { get; set; }

		[Required(ErrorMessage = "請輸入天數")]
		[DisplayName("天數")]
		public int? FDay { get; set; }

		public string? FTransport { get; set; }
	}
}
