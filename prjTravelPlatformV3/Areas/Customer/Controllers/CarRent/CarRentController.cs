using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Models;
using System.Security.Claims;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.CarRent
{
    [Area("Customer")]
    public class CarRentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public CarRentController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.GoogleMapsAPIKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            return View();
        }
        public IActionResult CarLists()
        {
            ViewBag.GoogleMapsAPIKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            return View();
        }


        public IActionResult CarDetails(string carId)
        {
            ViewBag.GoogleMapsAPIKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (CustomerId == 0)
            {
                return RedirectToAction("Index", "Login", new { area = "CustomizedIdentity" });
            }
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }
        public IActionResult CarBooking()
        {
            ViewBag.GoogleMapsAPIKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (CustomerId == 0)
            {
                return RedirectToAction("Index", "Login", new { area = "CustomizedIdentity" });
            }
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }
        public IActionResult CarBookingConfirm()
        {
            ViewBag.GoogleMapsAPIKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }

    }
}
