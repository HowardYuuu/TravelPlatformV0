using Microsoft.AspNetCore.Mvc;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Question
{
    public class QuestionController : Controller
    {
        [Area("Employee")]
        public IActionResult Index()
        {
            return View();
        }

    }
}
