using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    [Area("Employee")]
    public class CServicePointController : Controller
    {
        
        private readonly dbTravalPlatformContext _context;

        public CServicePointController(dbTravalPlatformContext context)
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
                ViewBag.formId = "CreateServicePoint";
                ViewBag.title = "新增據點資料";
                return PartialView("_ServicePointModalPartial", new ServicePointViewModel());
            }
            if (_context.TRservicePoints == null)
            {
                return NotFound();
            }
            var TRservicePoint = _context.TRservicePoints.Find(id);
            if (TRservicePoint == null)
            {
                return NotFound();
            }
            ServicePointViewModel c = new ServicePointViewModel
            {
                fServicePointId = TRservicePoint.FServicePointId,
                fServicePoint = TRservicePoint.FServicePoint,
                fAddress = TRservicePoint.FAddress,
                fPhone = TRservicePoint.FPhone,
                fServicePointInUse = TRservicePoint.FServicePointInUse,
            };
            ViewBag.formId = "EditServicePoint";
            ViewBag.title = "編輯據點資料";
            return PartialView("_ServicePointModalPartial", c);
        }

        public IActionResult GetDeletePartial(int? id)
        {
            if (_context.TRservicePoints == null)
            {
                return NotFound();
            }
            var TRservicePoint = _context.TRservicePoints.Find(id);
            if (TRservicePoint == null)
            {
                return NotFound();
            }
            ServicePointViewModel c = new ServicePointViewModel
            {
                fServicePointId = TRservicePoint.FServicePointId,
                fServicePoint = TRservicePoint.FServicePoint,
                fAddress = TRservicePoint.FAddress,
                fPhone = TRservicePoint.FPhone,
                fServicePointInUse = TRservicePoint.FServicePointInUse,
            };
            ViewBag.formId = "DeleteServicePoint";
            ViewBag.title = "刪除據點資料";
            return PartialView("_ServicePointDeletePartial", c);
        }
    }
}
