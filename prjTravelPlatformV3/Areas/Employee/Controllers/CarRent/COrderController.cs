using Microsoft.AspNetCore.Mvc;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.CarRent
{
    public class COrderController : Controller
    {
        [Area("Employee")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
