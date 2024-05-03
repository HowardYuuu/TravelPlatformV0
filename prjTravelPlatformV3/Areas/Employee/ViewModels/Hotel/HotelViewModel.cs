using prjTravelPlatformV3.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel
{
    public class HotelViewModel
    {
        public int HotelId { get; set; }

        [Required(ErrorMessage = "飯店為必填")]
        [DisplayName("飯店中文名稱")]
        public string? HotelName { get; set; }

        [DisplayName("飯店英文名稱")]
        public string? HotelEngName { get; set; }

        [DisplayName("地址")]
        public string? HotelAddress { get; set; }

        [Required(ErrorMessage = "電話為必填")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "長度不符")]
        [DisplayName("電話")]
        public string? Phone { get; set; }

        [DisplayName("區域")]
        public string? Region { get; set; }

        [DisplayName("統編")]
        public string? TexId { get; set; }
        public List<THimage>? HotelImages { get; set; }
        public List<THfacility>? HotelFacilities { get; set; }
        public List<THfacility>? ExistHotelFacilities { get; set; }
        public List<int>? AddFacilities {  get; set; }
        public List<int>? RemoveFacilities { get; set; }


    }
}
