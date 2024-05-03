using prjTravelPlatform_release.Areas.Employee.ViewModels.Product;
using prjTravelPlatformV3.Models;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Products
{
    public class TIorderViewModel
    {
        public int FId { get; set; }

        public string? FOrderId { get; set; }
        public int? FMemberId { get; set; }
        public string? FNotes { get; set; }

        [Required(ErrorMessage = "姓名是必填欄位")]
        public string? FShipName { get; set; }

        [Required(ErrorMessage = "電話是必填欄位")]
        public string? FShipPhone { get; set; }

        [Required(ErrorMessage = "收件地址是必填欄位")]
        public string? FShipAddress { get; set; }
        public int? FLogisticsId { get; set; }
        public int? FCoupponId { get; set; }
        public int? FPayId { get; set; }

        // 添加一个属性来接收 TIorderDetail 模型的列表
        public List<ItemOrdetailViewModel> OrderDetails { get; set; }

    }
}
