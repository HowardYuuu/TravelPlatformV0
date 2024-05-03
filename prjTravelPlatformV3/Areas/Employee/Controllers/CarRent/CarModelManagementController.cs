using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.CarRent;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    [Area("Employee")]
    public class CarModelManagementController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public CarModelManagementController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }


        public IActionResult GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增車型資料";
                return PartialView("_CarModelModalPartial", new CarModelViewModel());
            }
            if (_context.TRcarModels == null)
            {
                return NotFound();
            }
            var TRcarModel = _context.TRcarModels.Find(id);
            if (TRcarModel == null)
            {
                return NotFound();
            }
            CarModelViewModel c = new CarModelViewModel
            {
                FModelId = TRcarModel.FModelId,
                FModelName = TRcarModel.FModelName,
                FNumOfLuggage = TRcarModel.FNumOfLuggage,
                FNumOfPsgr = TRcarModel.FNumOfPsgr,
                FRentalFee = TRcarModel.FRentalFee,
                FImagePath = TRcarModel.FImagePath,
                FModelInUse = TRcarModel.FModelInUse,
            };
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯車型資料";
            return PartialView("_CarModelModalPartial", c);
        }

        public IActionResult GetDeletePartial(int? id)
        {
            if (_context.TRcarModels == null)
            {
                return NotFound();
            }
            var TRcarModel = _context.TRcarModels.Find(id);
            if (TRcarModel == null)
            {
                return NotFound();
            }
            CarModelViewModel c = new CarModelViewModel
            {
                FModelId = TRcarModel.FModelId,
                FModelName = TRcarModel.FModelName,
                FNumOfLuggage = TRcarModel.FNumOfLuggage,
                FNumOfPsgr = TRcarModel.FNumOfPsgr,
                FRentalFee = TRcarModel.FRentalFee,
                FImagePath = TRcarModel.FImagePath,
                FModelInUse = TRcarModel.FModelInUse,
            };
            ViewBag.formId = "Delete";
            ViewBag.title = "刪除車型資料";
            return PartialView("_CarModelDeletePartial", c);
        }


    }
}
