using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using prjTravelPlatformV3.Models;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel
{
    public class RoomTypeViewModel
    {
        public int RoomTypeId { get; set; }

        public int? HotelId { get; set; }


        [Display(Name = "選擇飯店")]
        public List<THotel>? Hotels { get; set; }

        public List<SelectListItem>? HotelSelectList { get; set; } = new List<SelectListItem>();

        public string? HotelName { get; set; }

        [Display(Name = "房型名稱")]
        public string? RoomTypeName { get; set; }

        [Display(Name = "床型")]
        public string? BedType { get; set; }

        [Display(Name = "床數")]
        public int? BedNum { get; set; }

        [Display(Name = "可容納人數")]
        public int? MaxCapacity { get; set; }

        [Display(Name = "定價")]
        public decimal Price { get; set; }
        public List<THroomTypeImage>? RoomTypeImages { get; set; }
        public List<THroom>? rooms { get; set; }
        public List<int>? roomNumberList { get; set; }
    }
}
