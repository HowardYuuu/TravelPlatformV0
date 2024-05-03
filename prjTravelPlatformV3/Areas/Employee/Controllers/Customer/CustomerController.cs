using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Customer;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Employee;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Customer
{
    [Area("Employee")]
    public class CustomerController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        public CustomerController(dbTravalPlatformContext context)
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
                ViewBag.title = "新增會員資料";
                return PartialView("_CModalPartial", new CustomerViewModel());
            }
            if (_context.TCustomers == null)
            {
                return NotFound();
            }
            var tCustomer = _context.TCustomers.Find(id);
            if (tCustomer == null)
            {
                return NotFound();
            }
            CustomerViewModel c = new CustomerViewModel
            {
                CustomerId = tCustomer.FCustomerId,
                Name = tCustomer.FName,
                Password = tCustomer.FPassword,
                Phone = tCustomer.FPhone,
                IdentityNumber = tCustomer.FIdentityNumber,
                Gender = tCustomer.FGender,
                Birth = tCustomer.FBirth,
                Email = tCustomer.FEmail,
                Address = tCustomer.FAddress,
                IsCheck = tCustomer.FIsCheck,
            };
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯會員資料";
            return PartialView("_CModalPartial", c);
        }

        private bool TCustomerExists(int id)
        {
            return (_context.TCustomers?.Any(c => c.FCustomerId == id)).GetValueOrDefault();
        }
    }
}
