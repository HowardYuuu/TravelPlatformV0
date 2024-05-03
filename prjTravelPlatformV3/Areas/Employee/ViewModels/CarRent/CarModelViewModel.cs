using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent
{
    public class CarModelViewModel
    {
        public int FModelId { get; set; }

        [Required(ErrorMessage = "*車輛型號為必填")]
        [DisplayName("車輛型號")]
        public string? FModelName { get; set; }
        [Required(ErrorMessage = "*請填寫容納行李數")]
        [DisplayName("容納行李數")]
        [Range(0, 50, ErrorMessage = "*容納行李數必須是有效的數字")]
        public int? FNumOfLuggage { get; set; }
        [Required(ErrorMessage = "*請填寫車載人數")]
        [DisplayName("車載人數")]
        [Range(0, 50, ErrorMessage = "*車載人數必須是有效的數字")]
        public int? FNumOfPsgr { get; set; }
        [Required(ErrorMessage = "*請填寫日租費用")]
        [DisplayName("日租費用")]
        [Range(0, int.MaxValue, ErrorMessage = "*日租費用必須是有效的數字")]
        public decimal? FRentalFee { get; set; }
        [DisplayName("車輛圖檔")]
        public string? FImagePath { get; set; }
        [Required(ErrorMessage = "*啟用狀態為必選")]
        [DisplayName("啟用狀態")]
        public bool? FModelInUse { get; set; }

        public IFormFile? ImageFile { get; set; } // 添加此屬性用於接收上傳的圖片文件
    }
}
