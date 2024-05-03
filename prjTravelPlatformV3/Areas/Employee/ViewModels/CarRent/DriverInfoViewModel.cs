using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent
{
    public class DriverInfoViewModel
    {
        public int fDriverId { get; set; }
        [DisplayName("身分證字號")]
        [Required(ErrorMessage = "*請填寫身分證字號")]
        [RegularExpression(@"^[A-Z]\d{9}$", ErrorMessage = "*身分證字號格式不正確，首字為英文大寫，後面9個為數字")]
        public string? fId { get; set; }
        [DisplayName("駕駛姓名")]
        [Required(ErrorMessage = "*請填寫駕駛姓名")]
        public string? fName { get; set;}
        [DisplayName("聯絡電話")]
        [Required(ErrorMessage = "*請填寫聯絡電話")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "*聯絡電話格式不正確，必須是10碼數字，市內電話請加區域碼")]
        public string? fPhone { get; set;}
        [DisplayName("駕照圖檔")]
        public string? fLicenseImagePath { get; set;}
        public IFormFile? ImageFile { get; set; } // 添加此屬性用於接收上傳的圖片文件
    }
}
