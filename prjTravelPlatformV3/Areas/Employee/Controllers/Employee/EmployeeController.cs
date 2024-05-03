using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Employee;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Employee
{
    [Area("Employee")]
    public class EmployeeController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        public EmployeeController(dbTravalPlatformContext context)
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
        
        public IActionResult GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增員工資料";
                return PartialView("_EModalPartial", new EmployeeViewModel());
            }
            if (_context.TEmployees == null)
            {
                return NotFound();
            }
            var tEmployee = _context.TEmployees.Find(id);
            if (tEmployee == null)
            {
                return NotFound();
            }
            EmployeeViewModel e = new EmployeeViewModel
            {
                EmployeeId = tEmployee.FEmployeeId,
                Name = tEmployee.FName,
                AccountNumber = tEmployee.FAccountNumber,
                Password = tEmployee.FPassword,
                Phone = tEmployee.FPhone,
                IdentityNumber = tEmployee.FIdentityNumber,
                Gender = tEmployee.FGender,
                Birth = tEmployee.FBirth,
                Email = tEmployee.FEmail,
                Address = tEmployee.FAddress,
                Status = tEmployee.FStatus,
                StaffId = tEmployee.FStaffId,
            };
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯員工資料";
            return PartialView("_EModalPartial", e);
        }

        private bool TEmployeeExists(int id)
        {
            return (_context.TEmployees?.Any(e => e.FEmployeeId == id)).GetValueOrDefault();
        }
    }
}
