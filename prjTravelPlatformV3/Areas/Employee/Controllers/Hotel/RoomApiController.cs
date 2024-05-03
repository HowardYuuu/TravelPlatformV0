using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Hotel
{
    [Route("/api/RoomType/{action}/{id?}")]
    public class RoomApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomApiController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        #region Room Api
        public IActionResult GetRoomTypeAll()
        {
            try
            {
                var roomTypes = from r in _context.VRoomTypeViews
                                select r;
                return Json(roomTypes);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"錯誤訊息: {e.Message}" });
            }
        }
        public IActionResult GetRoomTypeById(int id)
        {
            var roomType = _context.THroomTypes.FirstOrDefault(h => h.FRoomTypeId == id);
            return Json(roomType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeViewModel roomType, List<IFormFile> file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    THroomType rt = new THroomType
                    {
                        FRoomTypeId = roomType.RoomTypeId,
                        FHotelId = roomType.HotelId,
                        FRoomTypeName = roomType.RoomTypeName,
                        FBedType = roomType.BedType,
                        FBedNum = roomType.BedNum,
                        FMaxCapacity = roomType.MaxCapacity,
                        FPrice = roomType.Price,
                    };

                    _context.Add(rt);
                    await _context.SaveChangesAsync();

                    if (file != null)
                    {
                        var lastId = _context.THroomTypes.Max(h => h.FRoomTypeId);
                        foreach (var f in file)
                        {
                            string fileName = Guid.NewGuid().ToString() + ".jpg";

                            THroomTypeImage img = new THroomTypeImage()
                            {
                                FRoomTypeId = lastId,
                                FRoomImage = fileName
                            };
                            _context.Add(img);
                            await _context.SaveChangesAsync();

                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel/RoomType", $"{fileName}");
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                f.CopyTo(fileStream);
                            }
                        }
                    }


                    return Json(new { success = true, message = "資料新增成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料新增失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomTypeViewModel roomType, List<IFormFile> file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    THroomType rt = new THroomType
                    {
                        FRoomTypeId = roomType.RoomTypeId,
                        FHotelId = roomType.HotelId,
                        FRoomTypeName = roomType.RoomTypeName,
                        FBedType = roomType.BedType,
                        FBedNum = roomType.BedNum,
                        FMaxCapacity = roomType.MaxCapacity,
                        FPrice = roomType.Price,
                    };
                    _context.Update(rt);

                    if (file != null)
                    {
                        foreach (var f in file)
                        {
                            string fileName = Guid.NewGuid().ToString() + ".jpg";
                            THroomTypeImage img = new THroomTypeImage()
                            {
                                FRoomTypeId = roomType.RoomTypeId,
                                FRoomImage = fileName
                            };
                            _context.Add(img);


                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel/RoomType", $"{fileName}");
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                f.CopyTo(fileStream);
                            }
                        }
                    }
                    //房間
                    if (roomType.roomNumberList != null)
                    {
                        foreach (var i in roomType.roomNumberList)
                        {
                            THroom room = new THroom()
                            {
                                FRoomNumber = i.ToString(),
                                FRoomTypeId = roomType.RoomTypeId,
                                FHotelId = (int)roomType.HotelId,
                                FRoomStatus = true
                            };
                            _context.Add(room);

                        }
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "資料修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto([FromBody] List<int> idList)
        {
            if (_context.THroomTypeImages == null)
            {
                return Problem("Entity set 'dbTravalPlatformContext.THotels'  is null.");
            }
            try
            {
                foreach (int id in idList)
                {
                    var image = await _context.THroomTypeImages.FindAsync(id);

                    if (image != null)
                    {
                        //先刪除網站跟目錄之檔案
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel/RoomType", $"{image.FRoomImage}");
                        if (System.IO.File.Exists(filePath))
                        {
                            // 删除文件
                            System.IO.File.Delete(filePath);
                        }
                        else
                        {
                            throw new Exception("圖片不存在");
                        }
                        _context.THroomTypeImages.Remove(image);
                    }
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"照片刪除成功" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"照片刪除失敗，原因{e.Message}" });
            }






        }
        #endregion
    }
}
