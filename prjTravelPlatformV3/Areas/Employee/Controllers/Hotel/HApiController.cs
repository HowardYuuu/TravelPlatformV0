using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;


namespace prjTravelPlatformV3.Areas.Employee.Controllers.Hotel
{
    [Route("/api/Hotels/{action}/{id?}")]
    public class HApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region Hotel Api
        public HApiController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        public IActionResult GetAll()
        {
            var hotels = from h in _context.THotels
                         select h;
            return Json(hotels);
        }
        public IActionResult GetById(int id)
        {
            var hotel = _context.THotels.FirstOrDefault(h => h.FHotelId == id);
            return Json(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel hotel, List<IFormFile> file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    THotel tHotel = new THotel
                    {
                        FHotelId = hotel.HotelId,
                        FHotelName = hotel.HotelName,
                        FHotelEngName = hotel.HotelEngName,
                        FHotelAddress = hotel.HotelAddress,
                        FPhone = hotel.Phone,
                        FRegion = hotel.Region,
                        FTexId = hotel.TexId,
                    };
                    _context.Add(tHotel);
                    await _context.SaveChangesAsync();

                    //圖片
                    var lastHotelId = _context.THotels.Max(h => h.FHotelId);
                    foreach (var f in file)
                    {
                        string fileName = Guid.NewGuid().ToString() + ".jpg";
                        THimage img = new THimage()
                        {
                            FHotelId = lastHotelId,
                            FHotelImage = fileName
                        };
                        _context.Add(img);
                        await _context.SaveChangesAsync();

                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel", $"{fileName}");
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            f.CopyTo(fileStream);
                        }
                    }
                    //設施
                    if (hotel.AddFacilities != null)
                    {
                        foreach (var i in hotel.AddFacilities)
                        {
                            THfacilityRelation relation = new THfacilityRelation
                            {
                                FHotelId = lastHotelId,
                                FHotelFacilityId = i
                            };
                            _context.Add(relation);

                        }
                        await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(HotelViewModel hotel, List<IFormFile> file)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        #region info
                        THotel tHotel = new THotel
                        {
                            FHotelId = hotel.HotelId,
                            FHotelName = hotel.HotelName,
                            FHotelEngName = hotel.HotelEngName,
                            FHotelAddress = hotel.HotelAddress,
                            FPhone = hotel.Phone,
                            FRegion = hotel.Region,
                            FTexId = hotel.TexId,
                        };
                        _context.Update(tHotel);

                        #endregion

                        #region file
                        foreach (var f in file)
                        {
                            string fileName = Guid.NewGuid().ToString() + ".jpg";
                            THimage img = new THimage()
                            {
                                FHotelId = hotel.HotelId,
                                FHotelImage = fileName
                            };
                            _context.Add(img);

                            //寫入圖片
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel", $"{fileName}");
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                f.CopyTo(fileStream);
                            }
                        }
                        #endregion

                        #region facility
                        //處理設施
                        var facility = _context.THfacilityRelations
                            .Where(x => x.FHotelId == hotel.HotelId).ToList();

                        //新增
                        if (hotel.AddFacilities != null)
                        {
                            foreach (var i in hotel.AddFacilities)
                            {
                                if (!facility.Select(x => x.FHotelFacilityId).Contains(i))
                                {
                                    THfacilityRelation relation = new THfacilityRelation
                                    {
                                        FHotelId = hotel.HotelId,
                                        FHotelFacilityId = i
                                    };
                                    _context.Add(relation);
                                }
                            }
                        }
                        //刪除
                        if (hotel.RemoveFacilities != null)
                        {
                            foreach (var i in hotel.RemoveFacilities)
                            {
                                if (facility.Select(x => x.FHotelFacilityId).Contains(i))
                                {
                                    var item = _context.THfacilityRelations.FirstOrDefault(x => x.FHotelId == hotel.HotelId && x.FHotelFacilityId == i);
                                    if (item != null)
                                    {
                                        _context.Remove(item);
                                    }
                                }
                            }
                        }
                        #endregion
                        // 提交

                        await _context.SaveChangesAsync();
                        transaction.Commit();

                        return Json(new { success = true, message = "資料修改成功" });
                    }
                    else
                    {
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
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
                }

            }
        }



        [HttpPost]
        public async Task<IActionResult> DeletePhoto([FromBody] List<int> idList)
        {
            if (_context.THimages == null)
            {
                return Problem("Entity set 'dbTravalPlatformContext.THotels'  is null.");
            }
            try
            {
                foreach (int id in idList)
                {
                    var image = await _context.THimages.FindAsync(id);

                    if (image != null)
                    {
                        //先刪除網站跟目錄之檔案
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel", $"{image.FHotelImage}");
                        if (System.IO.File.Exists(filePath))
                        {
                            // 删除文件
                            System.IO.File.Delete(filePath);
                        }
                        else
                        {
                            throw new Exception("圖片不存在");
                        }
                        _context.THimages.Remove(image);
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
