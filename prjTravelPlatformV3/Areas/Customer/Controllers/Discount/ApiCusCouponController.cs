using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Models;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace prjTravelPlatform_release.Areas.Customer.Controllers.Discount
{
    [Route("/api/CusCoupons/{action}/{id?}")]

    public class ApiCusCouponController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiCusCouponController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult GetData()
        {
            var CusCoupon = from c in _context.VDcusCouponViews
                            join d in _context.TDcCouponLists
                            on c.折扣碼 equals d.FCouponCode
                            select new
                            {
                                CusID = c.客戶編號,
                                CusName = c.客戶名稱,
                                CouponCode = c.折扣碼,
                                CouponName = c.優惠卷名稱,
                                Discount = d.FDiscount,
                                Amount = d.FAmount,
                                PType = d.FProductType,
                                StartDate = c.啟用日期,
                                EndDate = c.截止日期,
                                Enabled = c.使用狀態
                            };
            return Json(CusCoupon);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public async Task<IActionResult> getCoupon(int? couponId)
        {
            if(User.Identity != null && User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) == "Customer")
            {
                ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int CustomerId = Convert.ToInt32(ViewBag.CusID);
                //LinQ語法搜尋回傳
                var c = _context.TDcCouponLists.FirstOrDefault(x => x.FCouponId == couponId);
                var cus = _context.TDcCusCouponQties.FirstOrDefault(x => x.FCouponId.Equals(couponId) && x.FCustomerId.Equals(CustomerId));
                //判斷條件
                if (cus != null)//如果已領取過，會員優惠表中的那筆CouponID不再是null
                {
                    return Json(new { success = false, errorMessage = "折扣碼已重複領取" });
                };
                TDcCusCouponQty d = new TDcCusCouponQty();
                d.FCustomerId = CustomerId;
                d.FCouponId = (int)couponId;
                d.FStartDate = Convert.ToDateTime(c.FStartDate);/*DateTime.Now;*/
                d.FEndDate = Convert.ToDateTime(c.FEndDate);/*DateTime.Now;*/
                d.FUsed = false;
                _context.Add(d);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            //取得當前登入客戶的ID，實際內容在CustomizedIdentity/Controllers/ApiLoginController.cs




            //var cus = _context.VDcusCouponViews.FirstOrDefault(x => x.折扣碼.Equals(couponCode) && x.客戶編號.Equals(CustomerId));


            //if (ViewBag.CusID == null)//會員需登入
            //{
            //    
            //}


            //將點擊的優惠卷join優惠卷總表寫入資料庫
            return Json(new { success = false, errorMessage = "請先登入會員" });

        }

        [HttpPost]
        public async Task<IActionResult> inputCoupon(string? couponCode)
        {
            //取得當前登入客戶的ID
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int CustomerId = Convert.ToInt32(ViewBag.CusID);

            //LinQ語法搜尋回傳
            var c = _context.TDcCouponLists.FirstOrDefault(x => x.FCouponCode == couponCode);
            var cus = _context.VDcusCouponViews.FirstOrDefault(x => x.折扣碼.Equals(couponCode) && x.客戶編號.Equals(CustomerId));

            //判斷條件
            if (ViewBag.CusID == null)//需登入會員
            {
                return Json(new { success = false, errorMessage = "請先登入會員" });
            }
            if (c == null)//確認無效折扣碼
            {
                return Json(new { success = false, errorMessage = "折扣碼無效" });
            }
            if (cus != null)//確認會員是否已有該張優惠卷
            {
                return Json(new { success = false, errorMessage = "折扣碼已重複領取" });
            }

            //寫入資料庫
            TDcCusCouponQty d = new TDcCusCouponQty();
            d.FCustomerId = CustomerId;
            d.FCouponId = c.FCouponId;
            d.FStartDate = c.FStartDate;
            d.FEndDate = c.FEndDate;
            d.FUsed = false;
            _context.Add(d);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        public async Task<IActionResult> CouponType(string? CouponType)
        {
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int CustomerID = Convert.ToInt32(ViewBag.CusID);
            var coupon = from d in _context.TDcCouponLists
                         join c in _context.TDcCusCouponQties
                         on d.FCouponId equals c.FCouponId
                         where d.FDiscountType == CouponType && c.FCustomerId == CustomerID
                         select c;

            var CusCoupon = from c in _context.VDcusCouponViews
                            join d in _context.TDcCouponLists
                            on c.折扣碼 equals d.FCouponCode
                            where d.FProductType == CouponType
                            select new
                            {
                                CusID = c.客戶編號,
                                CusName = c.客戶名稱,
                                CouponCode = c.折扣碼,
                                CouponName = c.優惠卷名稱,
                                Discount = d.FDiscount,
                                DisType = d.FDiscountType,
                                Amount = d.FAmount,
                                PType = d.FProductType,
                                StartDate = c.啟用日期,
                                EndDate = c.截止日期,
                                Enabled = c.使用狀態
                            };

            return Json(CusCoupon);
        }

        //[HttpGet]
        //[Route("LoadWin")]
        public void LoadWin()
        {
            ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int CustomerID = Convert.ToInt32(ViewBag.CusID);
            var ic = _context.TIorders.Where(x => x.FMemberId == CustomerID && x.FCoupponId != null).ToList();
            if (ic.Any())
            {

                foreach (var order in ic)
                {
                    //更新會員優惠表中相同優惠券代碼的記錄為已使用
                    var memberDiscounts = _context.TDcCusCouponQties
                        .Where(md => md.FCustomerId == CustomerID && md.FCouponId == order.FCoupponId && md.FUsed == false)
                        .ToList();

                    foreach (var memberDiscount in memberDiscounts)
                    {
                        memberDiscount.FUsed = true;
                    }

                    _context.SaveChanges();

                }
            }
        }


        //public async Task<IActionResult> LoginDay()
        //{
        //    ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (ViewBag.CusID == null)
        //    {
        //        return Json(new { success = false, errorMessage = "請先登入會員" });
        //    }

        //    if (ViewBag.CusID != null)
        //    {
        //        int CustomerID = Convert.ToInt32(ViewBag.CusID);
        //        //var day = _context.TDcusLoginDays.FirstOrDefault(x=>x.FMemberId == CustomerID).FLogindays;

        //        //if(day == null)
        //        //{
        //        //    TDcusLoginDay c = new TDcusLoginDay();
        //        //    c.FMemberId = CustomerID;
        //        //    c.FLogindays = 1;
        //        //}

        //        //if (Convert.ToInt32(day) != null)
        //        //{
        //        //    TDcusLoginDay c = new TDcusLoginDay();
        //        //    c.FMemberId = CustomerID;
        //        //    c.FLogindays = c.FLogindays+1;
        //        //}


        //        //if (Convert.ToInt32(day) == 31)
        //        //{
        //        //    TDcusLoginDay c = new TDcusLoginDay();
        //        //    c.FMemberId = CustomerID;
        //        //    c.FLogindays = 0;
        //        //}

        //        var member = _context.TDcusLoginDays.FirstOrDefault(x => x.FMemberId == CustomerID);
        //        //ViewBag.CusDays = member.FLogindays;
        //        var c = _context.TDcCouponLists;
        //        if (member == null)
        //        {

        //            // 如果會員登入資料不存在，則新增一條登入資料
        //            member = new TDcusLoginDay { FMemberId = CustomerID, FLogindays = 1 };
        //            _context.TDcusLoginDays.Add(member);
        //        }
        //        else
        //        {
        //            // 如果會員登入資料已存在，則增加登入天數
        //            member.FLogindays++;

        //            if (member.FLogindays == 5)
        //            {
        //                TDcCusCouponQty d = new TDcCusCouponQty();
        //                d.FCustomerId = CustomerID;
        //                d.FCouponId = c.FirstOrDefault(x => x.FNote == "5").FCouponId;
        //                d.FStartDate = c.FirstOrDefault(x => x.FNote == "5").FStartDate;
        //                d.FEndDate = c.FirstOrDefault(x => x.FNote == "5").FEndDate;
        //                d.FUsed = false;
        //                _context.Add(d);
        //                _context.SaveChanges();
        //            }
        //            if (member.FLogindays == 15)
        //            {
        //                TDcCusCouponQty d = new TDcCusCouponQty();
        //                d.FCustomerId = CustomerID;
        //                d.FCouponId = c.FirstOrDefault(x => x.FNote == "15").FCouponId;
        //                d.FStartDate = c.FirstOrDefault(x => x.FNote == "15").FStartDate;
        //                d.FEndDate = c.FirstOrDefault(x => x.FNote == "15").FEndDate;
        //                d.FUsed = false;
        //                _context.Add(d);
        //                _context.SaveChanges();
        //            }
        //            if (member.FLogindays == 25)
        //            {
        //                TDcCusCouponQty d = new TDcCusCouponQty();
        //                d.FCustomerId = CustomerID;
        //                d.FCouponId = c.FirstOrDefault(x => x.FNote == "25").FCouponId;
        //                d.FStartDate = c.FirstOrDefault(x => x.FNote == "25").FStartDate;
        //                d.FEndDate = c.FirstOrDefault(x => x.FNote == "25").FEndDate;
        //                d.FUsed = false;
        //                _context.Add(d);
        //                _context.SaveChanges();
        //            }
        //            if (member.FLogindays == 30)
        //            {
        //                TDcCusCouponQty d = new TDcCusCouponQty();
        //                d.FCustomerId = CustomerID;
        //                d.FCouponId = c.FirstOrDefault(x => x.FNote == "30").FCouponId;
        //                d.FStartDate = c.FirstOrDefault(x => x.FNote == "30").FStartDate;
        //                d.FEndDate = c.FirstOrDefault(x => x.FNote == "30").FEndDate;
        //                d.FUsed = false;
        //                _context.Add(d);
        //                _context.SaveChanges();
        //            }
        //            if (member.FLogindays > 31)
        //            {
        //                member.FLogindays = 1; // 歸零
        //            }
        //        }

        //        _context.SaveChanges();
        //    }
        //    return Json(new { success = true });
        //}
    }
}
