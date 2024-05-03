using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Airline
{
    [Area("Employee")]
    public class FOrderController : Controller
    {

        private readonly dbTravalPlatformContext _context;
        public FOrderController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
    }
}
