using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan
{
	public class DestionationEditView
	{
		public string? FDestinationId { get; set; }

		public bool? FState { get; set; }

		[Required(ErrorMessage = "景點名稱為必填")]
		[DisplayName("景點名稱")]
		public string? FDestinationName { get; set; }

		[Required(ErrorMessage = "金額尚未填寫")]
		[RegularExpression(@"^\d+$", ErrorMessage = "價位僅能包含數字")]
		[DisplayName("價位(NT)")]
		public int? FPrice { get; set; }

		[Required(ErrorMessage = "數量尚未填寫")]
		[RegularExpression(@"^\d+$", ErrorMessage = "價位僅能包含數字")]
		[DisplayName("門票數量")]
		public int? FStock { get; set; }

		[Required(ErrorMessage = "地址為必填")]
		[DisplayName("地址")]
		public string? FAddress { get; set; }

		[Required(ErrorMessage = "景點區域為必選")]
		[DisplayName("區域")]
		public int? FAreaId { get; set; }

		[Required(ErrorMessage = "景點類型為必選")]
		[DisplayName("類型")]
		public int? FDestinationTypeId { get; set; }

		[DisplayName("景點敘述")]
		public string? FDestinationContent { get; set; }

		[DisplayName("架上狀態")]
		public int? FPriority {  get; set; }

		[DisplayName("景點圖片")]
		public List<byte[]>? TdestinationPhotos { get; set; }
	}
}
