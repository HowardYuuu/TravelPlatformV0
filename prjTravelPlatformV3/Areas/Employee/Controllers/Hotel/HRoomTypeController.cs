using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;
using System;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Hotel
{
    [Area("Employee")]
    public class HRoomTypeController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HRoomTypeController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增房型資料";
                
                var hotels = from h in _context.THotels
                             select h;

                RoomTypeViewModel roomType1 = new RoomTypeViewModel();
                foreach (var h in hotels) 
                {
                    roomType1.HotelSelectList.Add(new SelectListItem { Value = h.FHotelId.ToString(), Text = h.FHotelName });
                };

                
                return PartialView("_ModalPartial", roomType1);
            }

            if (_context.THroomTypes == null)
            {
                return NotFound();
            }
            var roomType = await _context.THroomTypes.FindAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }

            //info
            RoomTypeViewModel rt = new RoomTypeViewModel
            {
                RoomTypeId = roomType.FRoomTypeId,
                HotelId = roomType.FHotelId,
                RoomTypeName = roomType.FRoomTypeName,
                BedType = roomType.FBedType,
                BedNum = roomType.FBedNum,
                MaxCapacity = roomType.FMaxCapacity,
                Price = roomType.FPrice,
            };

            //image
            var roomTypeImgs = from rtt in _context.THroomTypes
                               join rtImage in _context.THroomTypeImages on rtt.FRoomTypeId equals rtImage.FRoomTypeId into a
                               from b in a.DefaultIfEmpty()
                               where rtt.FRoomTypeId == id
                               select b;

            if (roomTypeImgs.FirstOrDefault() == null)
            {
                rt.RoomTypeImages = null;
            }
            else
            {
                foreach (var rtImg in roomTypeImgs)
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel/RoomType", $"{rtImg.FRoomImage}");
                    if (!System.IO.File.Exists(filePath))
                    {
                        rtImg.FRoomImage = "noPhoto.jpg";
                    }
                }
                rt.RoomTypeImages = roomTypeImgs.ToList();
            }

            //room
            var roomList = _context.THrooms.Where(r=>r.FRoomTypeId == id).ToList();
            rt.rooms = roomList;

            ViewBag.formId = "Edit";
            ViewBag.title = "編輯房型資料";
            return PartialView("_ModalPartial", rt);
        }
    }
}
