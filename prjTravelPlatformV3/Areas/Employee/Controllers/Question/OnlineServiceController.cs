using Microsoft.AspNetCore.Mvc;

namespace prjTravelPlatform_release.Areas.Employee.Controllers.Question
{
    public class OnlineServiceController : Controller
    {
        [Area("Employee")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
