using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan;
using prjTravelPlatformV3.Models;
using System.Diagnostics;

namespace prjTravelPlatformV3.Areas.Customer.Controllers.Home
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbTravalPlatformContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(ILogger<HomeController> logger, dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}