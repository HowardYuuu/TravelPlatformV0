using prjTravelPlatformV3.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent
{
    public class CarInfoViewModel
    {
        public int fCarId { get; set; }

        [DisplayName("車牌號碼")]
        [Required(ErrorMessage = "*請填寫車牌號碼")]
        [RegularExpression(@"^[A-Z]{2,3}-\d{3,4}$|^\d{3,4}-[A-Z]{2,3}$", ErrorMessage = "*車牌號碼格式不正確，請輸入符合台灣汽車車牌的格式")]
        public string? fLicensePlateNum { get; set; }
        [Required(ErrorMessage = "*車輛型號為必選")]
        
        public int? fModelId { get; set; }
        [DisplayName("車輛型號")]
        public string? fModelName { get; set; }
        [DisplayName("車輛圖檔")]
        public string? fImagePath { get; set; }
        [Required(ErrorMessage = "*出租狀態為必選")]
        [DisplayName("出租狀態")]
        public Boolean? fRentStatus { get; set; } 
        [Required(ErrorMessage = "*所屬公司為必選")]
        [DisplayName("所屬公司")]
        public int? fCompanyId { get; set; }
        [Required(ErrorMessage = "*車所在地為必選")]
        [DisplayName("車所在地")]
        public int? fLocationId { get; set; }
        public List<TRcarModel>? CarModels { get; set; }
        public List<TRservicePoint>? ServicePoints { get; set; }
        public List<TCcompanyInfo>? Companys { get; set; }
    }
}
