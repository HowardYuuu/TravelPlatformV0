using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatform_release.Areas.Customer.ViewModel.Discounts;
using prjTravelPlatformV3.Models;
using System.Security.Claims;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Discount
{
    [Area("Customer")]
    public class DiscountsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public DiscountsController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        //會員優惠卷的partial view
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> Index()
        {
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coupon = await _context.TDcCouponLists.ToListAsync();
            var cuscoupon = await _context.TDcCusCouponQties.ToListAsync();
            var vCouponQty = await _context.VDcusCouponViews.ToListAsync();
            var viewModel = new CusCouponViewModel
            {
                Coupon = coupon,
                CusCouQty = cuscoupon,
                VCouponQty = vCouponQty,
            };

            return View(viewModel);
        }

        //領取優惠卷頁面
        public async Task<IActionResult> GetCoupon()
        {
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coupon = await _context.TDcCouponLists.ToListAsync();
            var cuscoupon = await _context.TDcCusCouponQties.ToListAsync();
            var viewModel = new CusCouponViewModel
            {
                Coupon = coupon,
                CusCouQty = cuscoupon
            };

            ViewBag.Count = coupon.Count();
            return View(viewModel);
        }


        public IActionResult GameUI()
        {
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);




            return View();
        }

        public IActionResult ChatUITest()
        {
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);




            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CusCouponParrialView()
        {
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coupon = await _context.TDcCouponLists.ToListAsync();
            var cuscoupon = await _context.TDcCusCouponQties.ToListAsync();
            var vCouponQty = await _context.VDcusCouponViews.ToListAsync();
            var ViewModel = new CusCouponViewModel
            {
                Coupon = coupon,
                CusCouQty = cuscoupon,
                VCouponQty = vCouponQty,
            };

            return PartialView("_CusCouponPartial");

        }
    }
}
