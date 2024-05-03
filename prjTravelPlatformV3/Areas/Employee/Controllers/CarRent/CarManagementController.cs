using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent;
using prjTravelPlatformV3.Models;
using System.Dynamic;
using System.Net;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    [Area("Employee")]
    public class CarManagementController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public CarManagementController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateCar()
        {
            return View();
        }
        public IActionResult EditCar()
        {
            return View();
        }
        public IActionResult DeleteCar()
        {
            return View();
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
        public IActionResult GetPartial(int? id)
        {

            if (id == 0)
            {
                ViewBag.formId = "CreateCar";
                ViewBag.title = "新增車輛資料";
                // 獲取所需的數據
                var carModels2 = _context.TRcarModels.Where(cm => cm.FModelInUse == true).ToList();
                var servicePoints2 = _context.TRservicePoints.Where(cm => cm.FServicePointInUse == true).ToList();
                var compamys2 = _context.TCcompanyInfos.Where(c => c.FType == "R").ToList();
                // 建立新的 CarInfoViewModel 並為其屬性賦值
                CarInfoViewModel ci = new CarInfoViewModel
                {
                    Companys = compamys2,
                    ServicePoints = servicePoints2,
                    CarModels = carModels2,
                    fImagePath = "", 
                };

                return PartialView("_CarModalPartial", ci);
            }
            if (_context.TRcarInfos == null)
            {
                return NotFound();
            }
            var TRcarInfo = _context.TRcarInfos.Find(id);
            if (TRcarInfo == null)
            {
                return NotFound();
            }
            var carModel = _context.TRcarModels.Find(TRcarInfo.FModelId);
            if (carModel == null)
            {
                return NotFound();
            }
            var carModels = _context.TRcarModels.Where(cm => cm.FModelInUse == true).ToList();
            var servicePoints = _context.TRservicePoints.Where(cm => cm.FServicePointInUse == true).ToList();
            var compamys = _context.TCcompanyInfos.Where(c => c.FType == "R").ToList();
            CarInfoViewModel c = new CarInfoViewModel
            {
                Companys = compamys,
                ServicePoints = servicePoints,
                CarModels = carModels,
                fImagePath = carModel.FImagePath,
                fCarId = TRcarInfo.FCarId,
                fLicensePlateNum = TRcarInfo.FLicensePlateNum,
                fModelId = TRcarInfo.FModelId,
                fRentStatus = TRcarInfo.FRentStatus,
                fCompanyId = TRcarInfo.FCompanyId,
                fLocationId = TRcarInfo.FLocationId,
            };
            ViewBag.formId = "EditCar";
            ViewBag.title = "編輯車輛資料";
            return PartialView("_CarModalPartial", c);
        }

        public IActionResult GetDeletePartial(int? id)
        {
            if (_context.TRcarInfos == null)
            {
                return NotFound();
            }
            var TRcarInfo = _context.TRcarInfos.FirstOrDefault(car => car.FCarId == id);
            if (TRcarInfo == null)
            {
                return NotFound();
            }
            var carModel = _context.TRcarModels.Find(TRcarInfo.FModelId);
            if (carModel == null)
            {
                return NotFound();
            }
            var carModels = _context.TRcarModels.ToList();
            var servicePoints = _context.TRservicePoints.ToList();
            var compamys = _context.TCcompanyInfos.Where(c => c.FType == "R").ToList();
            CarInfoViewModel cd = new CarInfoViewModel
            {
                Companys = compamys,
                ServicePoints = servicePoints,
                CarModels = carModels,
                fImagePath = carModel.FImagePath,
                fCarId = TRcarInfo.FCarId,
                fLicensePlateNum = TRcarInfo.FLicensePlateNum,
                fModelId = TRcarInfo.FModelId,
                fRentStatus = TRcarInfo.FRentStatus,
                fCompanyId = TRcarInfo.FCompanyId,
                fLocationId = TRcarInfo.FLocationId,
            };
            ViewBag.formId = "DeleteCar";
            ViewBag.title = "刪除車輛資料";
            return PartialView("_CarDeletePartial", cd);
        }
    }
}
