using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Supplier
{
    [Area("Employee")]

    public class SupplierController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public SupplierController(dbTravalPlatformContext context)
        {
            _context = context;
        }

        //view
        public IActionResult Index()
        {
            return View();
        }

        //api
        public JsonResult allCompanies()
        {
            var Companies = _context.TCcompanyInfos;
            return Json(Companies);
        }
    }
}
