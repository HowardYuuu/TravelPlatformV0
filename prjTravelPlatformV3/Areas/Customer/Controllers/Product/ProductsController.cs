using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using prjTravelPlatformV3.Models;
using System.Security.Claims;
using static prjTravelPlatform_release.Areas.Customer.Controllers.Product.ProductApiController;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Product
{
    [Area("Customer")]
    public class ProductsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public ProductsController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ItemDetail(int productId)
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            // 將訂單相關數據傳遞到視圖中
            ViewData["ProductId"] = productId;
            return View();
        }
        public IActionResult ItemCart(int customerId)
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            // 將會員相關數據傳遞到視圖中
            //ViewData["CustomerId"] = customerId;
            return View();
        }
        //生成結帳頁面
        public IActionResult BookingInfoConfirm()
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }

        public IActionResult BookingFinish()
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }
        public class SevenStore
        {
            public required string storeid { get; set; }
            public required string storename { get; set; }
            public required string storeaddress { get; set; }
        }
        public IActionResult RequestUrl(SevenStore _store)
        {
            return View(_store);
        }
    }
}
