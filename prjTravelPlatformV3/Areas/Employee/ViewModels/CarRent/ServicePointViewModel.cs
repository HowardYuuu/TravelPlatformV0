using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent
{
    public class ServicePointViewModel
    {
        public int fServicePointId { get; set; }
        [DisplayName("據點名稱")]
        [Required(ErrorMessage = "*請填寫據點名稱")]
        public string? fServicePoint { get; set; }
        [DisplayName("地址")]
        [Required(ErrorMessage = "*請填寫地址")]
        public string? fAddress { get; set; }
        [DisplayName("聯絡電話")]
        [Required(ErrorMessage = "*請填寫聯絡電話")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "*聯絡電話格式不正確，必須是10碼數字，市內電話請加區域碼")]
        public string? fPhone { get; set;}
        [DisplayName("啟用狀態")]
        [Required(ErrorMessage = "*請選擇啟用狀態")]
        public Boolean? fServicePointInUse { get; set; }
    }
}
