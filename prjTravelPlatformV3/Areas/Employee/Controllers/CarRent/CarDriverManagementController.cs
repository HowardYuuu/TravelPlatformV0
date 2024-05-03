using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    [Area("Employee")]
    public class CarDriverManagementController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public CarDriverManagementController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "CreateDriver";
                ViewBag.title = "新增駕駛資料";
                return PartialView("_DriverModalPartial", new DriverInfoViewModel());
            }
            if (_context.TRdriverInfos == null)
            {
                return NotFound();
            }
            var TRdriverInfo = _context.TRdriverInfos.Find(id);
            if (TRdriverInfo == null)
            {
                return NotFound();
            }
            DriverInfoViewModel c = new DriverInfoViewModel
            {
                fDriverId = TRdriverInfo.FDriverId,
                fId = TRdriverInfo.FId,
                fName = TRdriverInfo.FName,
                fPhone = TRdriverInfo.FPhone,
                fLicenseImagePath = TRdriverInfo.FLicenseImagePath,
            };
            ViewBag.formId = "EditDriver";
            ViewBag.title = "編輯駕駛資料";
            return PartialView("_DriverModalPartial", c);
        }

        public IActionResult GetDeletePartial(int? id)
        {
            if (_context.TRdriverInfos == null)
            {
                return NotFound();
            }
            var TRdriverInfo = _context.TRdriverInfos.Find(id);
            if (TRdriverInfo == null)
            {
                return NotFound();
            }
            DriverInfoViewModel c = new DriverInfoViewModel
            {
                fDriverId = TRdriverInfo.FDriverId,
                fId = TRdriverInfo.FId,
                fName = TRdriverInfo.FName,
                fPhone = TRdriverInfo.FPhone,
                fLicenseImagePath = TRdriverInfo.FLicenseImagePath,
            };
            ViewBag.formId = "DeleteDriver";
            ViewBag.title = "刪除駕駛資料";
            return PartialView("_DriverDeleteModalPartial", c);
        }
    }
}
