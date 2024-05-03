using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    [Route("/api/CarRent/{action}/{id?}")]
    public class RApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RApiController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public IActionResult GetAllCar()
        {
            return Json(_context.RCarInfoViews);
        }
        public IActionResult CarModels()
        {
            return Json(_context.TRcarModels);
        }
        public IActionResult Drivers()
        {
            return Json(_context.TRdriverInfos);
        }
        public IActionResult Orders()
        {
            return Json(_context.RCarRentOrderInfoViews);
        }
        public IActionResult OrderDetails()
        {
            return Json(_context.RCarRentOrderDetailViews);
        }

        public IActionResult ServicePoints()
        {
            return Json(_context.TRservicePoints);
        }

        public IActionResult GetCarModelById(int id)
        {
            var carmodel = _context.TRcarModels.FirstOrDefault(c => c.FModelId == id);
            return Json(carmodel);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetailsByOrderId(string orderId)
        {
            try
            {
                var orderDetails = await _context.RCarRentOrderDetailViews
                    .Where(od => od.FOrderId.Equals(orderId))
                    .ToListAsync();
                if (orderDetails != null)
                {
                    return Json(orderDetails);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "資料儲存失敗"
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                return Json(new { success = false, message = "無法獲取訂單明細：" + ex.Message });
            }
        }


        //讀取車型
        public IActionResult getCarModels()
        {
            var carModels = _context.TRcarModels.Select(a => a.FModelName).Distinct();
            return Json(carModels);
        }
        public IActionResult GetCarImage(int modelId)
        {
            var carModel = _context.TRcarModels.Find(modelId);
            if (carModel == null)
            {
                return NotFound();
            }

            return Json(new { imagePath = carModel.FImagePath });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarModelViewModel carModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 處理圖片上傳
                    string? uniqueFileName = null;
                    if (carModel.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await carModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    TRcarModel rcarModel = new TRcarModel
                    {
                        FModelId = carModel.FModelId,
                        FModelName = carModel.FModelName,
                        FNumOfLuggage = carModel.FNumOfLuggage,
                        FNumOfPsgr = carModel.FNumOfPsgr,
                        FImagePath = uniqueFileName,
                        FRentalFee = carModel.FRentalFee,
                        FModelInUse = carModel.FModelInUse
                    };
                    _context.Add(rcarModel);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> CreateCar(CarInfoViewModel carInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TRcarInfo rcarInfo = new TRcarInfo
                    {
                        FCarId = carInfo.fCarId,
                        FLicensePlateNum = carInfo.fLicensePlateNum,
                        FModelId = carInfo.fModelId,
                        FRentStatus = carInfo.fRentStatus,
                        FCompanyId = carInfo.fCompanyId,
                        FLocationId = carInfo.fLocationId,
                    };
                    _context.Add(rcarInfo);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> CreateDriver(DriverInfoViewModel carDriver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 處理圖片上傳
                    string? uniqueFileName = null;
                    if (carDriver.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await carDriver.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    TRdriverInfo rcarDriver = new TRdriverInfo
                    {
                        FDriverId = carDriver.fDriverId,
                        FId = carDriver.fId,
                        FName = carDriver.fName,
                        FPhone = carDriver.fPhone,
                        FLicenseImagePath = uniqueFileName,
                    };
                    _context.Add(rcarDriver);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> CreateServicePoint(ServicePointViewModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TRservicePoint rservicePoint = new TRservicePoint
                    {
                        FServicePointId = servicePoint.fServicePointId,
                        FServicePoint = servicePoint.fServicePoint,
                        FAddress = servicePoint.fAddress,
                        FPhone = servicePoint.fPhone,
                        FServicePointInUse = servicePoint.fServicePointInUse,
                    };
                    _context.Add(rservicePoint);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(CarModelViewModel carModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 檢查是否有新的圖片上傳
                    string? uniqueFileName = carModel.FImagePath; // 預設為原有的圖片路徑
                    if (carModel.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await carModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                        TRcarModel rcarModel = new TRcarModel
                    {
                        FModelId = carModel.FModelId,
                        FModelName = carModel.FModelName,
                        FNumOfLuggage = carModel.FNumOfLuggage,
                        FNumOfPsgr = carModel.FNumOfPsgr,
                        FImagePath = uniqueFileName,
                        FRentalFee = carModel.FRentalFee,
                        FModelInUse = carModel.FModelInUse,
                    };
                    _context.Update(rcarModel);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(CarInfoViewModel carInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TRcarInfo? rcarInfo = _context.TRcarInfos.Find(carInfo.fCarId);
                    if (rcarInfo != null)
                    {
                        rcarInfo.FCarId = carInfo.fCarId;
                        rcarInfo.FLicensePlateNum = carInfo.fLicensePlateNum;
                        rcarInfo.FModelId = carInfo.fModelId;
                        rcarInfo.FRentStatus = carInfo.fRentStatus;
                        rcarInfo.FCompanyId = carInfo.fCompanyId;
                        rcarInfo.FLocationId = carInfo.fLocationId;
                        _context.Update(rcarInfo);

                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "資料修改成功" });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "資料儲存失敗"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "資料驗證失敗",
                        carInfo = carInfo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"資料修改失敗：{ex.ToString()}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDriver(DriverInfoViewModel carDriver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 檢查是否有新的圖片上傳
                    string? uniqueFileName = carDriver.fLicenseImagePath; // 預設為原有的圖片路徑
                    if (carDriver.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await carDriver.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    TRdriverInfo rcarDriver = new TRdriverInfo
                    {
                        FDriverId = carDriver.fDriverId,
                        FId = carDriver.fId,
                        FName = carDriver.fName,
                        FPhone = carDriver.fPhone,
                        FLicenseImagePath = uniqueFileName,
                    };
                    _context.Update(rcarDriver);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServicePoint(ServicePointViewModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TRservicePoint rcarDriver = new TRservicePoint
                    {
                        FServicePointId = servicePoint.fServicePointId,
                        FServicePoint = servicePoint.fServicePoint,
                        FAddress = servicePoint.fAddress,
                        FPhone = servicePoint.fPhone,
                        FServicePointInUse = servicePoint.fServicePointInUse,
                    };
                    _context.Update(rcarDriver);
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

        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var carModelToDelete = await _context.TRcarModels.FindAsync(id);
                if (carModelToDelete == null)
                {
                    return Json(new { success = false, message = "找不到要刪除的車型" });
                }

                _context.TRcarModels.Remove(carModelToDelete);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "車型已成功刪除" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"刪除車型失敗：{e.Message}" });
            }
        }
        [HttpDelete, ActionName("DeleteCar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarConfirmed(int id)
        {
            try
            {
                var carToDelete = await _context.TRcarInfos.FindAsync(id);
                if (carToDelete == null)
                {
                    return Json(new { success = false, message = "找不到要刪除的車輛" });
                }

                _context.TRcarInfos.Remove(carToDelete);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "車輛已成功刪除" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"刪除車輛失敗：{e.Message}" });
            }
        }

        [HttpDelete, ActionName("DeleteDriver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDriverConfirmed(int id)
        {
            try
            {
                var carDriverToDelete = await _context.TRdriverInfos.FindAsync(id);
                if (carDriverToDelete == null)
                {
                    return Json(new { success = false, message = "找不到要刪除的駕駛資料" });
                }

                _context.TRdriverInfos.Remove(carDriverToDelete);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "駕駛資料已成功刪除" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"刪除駕駛資料失敗：{e.Message}" });
            }
        }

        [HttpDelete, ActionName("DeleteServicePoint")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServicePointConfirmed(int id)
        {
            try
            {
                var servicePointToDelete = await _context.TRservicePoints.FindAsync(id);
                if (servicePointToDelete == null)
                {
                    return Json(new { success = false, message = "找不到要刪除的據點" });
                }

                _context.TRservicePoints.Remove(servicePointToDelete);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "據點已成功刪除" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"刪除據點失敗：{e.Message}" });
            }
        }
        private bool TRcarModelsExists(int id)
        {
            return (_context.TRcarModels?.Any(e => e.FModelId == id)).GetValueOrDefault();
        }
    }
}
