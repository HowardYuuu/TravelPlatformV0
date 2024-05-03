using Microsoft.AspNetCore.Mvc;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Product
{
    public class ILogisticsManagementController : Controller
    {
        [Area("Employee")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
