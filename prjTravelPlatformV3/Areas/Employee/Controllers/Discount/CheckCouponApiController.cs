using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Employee.Controllers.Discount
{
    [Route("/api/CheckCoupon/{action}/{id?}")]
    //[ApiController]
    public class CheckCouponApiController : Controller
    {

        private readonly dbTravalPlatformContext _dbContext;

        public CheckCouponApiController(dbTravalPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[HttpGet("SendBirthday")]
        public IActionResult SendBirthdayCoupon()
        {
            if (DateTime.Now.Day == 1)
            {
                SendBirthdayCouponLogic();
                return Ok("生日優惠卷已發放成功！");
            }

            return Ok("檢查完畢，非月初無須發放優惠卷。");
        }

        //[HttpGet("UpdateStatus")]
        public IActionResult UpdateCouponStatus()
        {
            SetCouponEnable();
            return Ok("優惠卷狀態已更新！");
        }

        private void SetCouponEnable()
        {
            DateTime currentDate = DateTime.Now;

            var couponsToActivate = _dbContext.TDcCouponLists
                .Where(coupon => coupon.FStartDate <= currentDate)
                .ToList();

            foreach (var coupon in couponsToActivate)
            {
                coupon.FEnable = true;
            }

            var couponsToDisable = _dbContext.TDcCouponLists
                .Where(c => c.FEndDate <= currentDate)
                .ToList();
            foreach (var c in couponsToDisable)
            {
                c.FEnable = false;
            }

            _dbContext.SaveChanges();
        }

        private void SendBirthdayCouponLogic()
        {
            var birthdayList = from c in _dbContext.TCustomers
                               select new { c.FCustomerId, c.FBirth };

            foreach (var c in birthdayList)
            {
                if (Convert.ToDateTime(c.FBirth).Month == DateTime.Now.Month)
                {
                    TDcCouponList birthdayCoupon = GenerateBirthdayCoupon();
                    TDcCusCouponQty x = new TDcCusCouponQty();
                    x.FCustomerId = c.FCustomerId;
                    x.FCouponId = birthdayCoupon.FCouponId;
                    x.FStartDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM"));
                    x.FEndDate = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy/MM"));
                    x.FUsed = true;
                    _dbContext.TDcCusCouponQties.Add(x);
                    _dbContext.SaveChanges();
                }
                return;
            }
        }

        private TDcCouponList GenerateBirthdayCoupon()
        {
            TDcCouponList x = new TDcCouponList();
            x.FCouponCode = "HAPPYBIRTH" + "0" + DateTime.Now.Month.ToString();
            x.FCouponName = "生日專屬優惠卷";
            x.FProductType = "平台";
            x.FStartDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM"));
            x.FEndDate = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy/MM"));
            x.FAmount = 0;
            x.FDiscount = 500;
            x.FRule = "壽星";
            x.FNote = "主题: 生日快樂！" +
                "\r\n親愛的顧客，" +
                "\r\n祝賀您生日快樂！為了慶祝這個特殊的日子，我們特別為您準備了一份生日優惠券。" +
                "\r\n優惠券碼: " + "HAPPYBIRTH" + "0" + DateTime.Now.Month.ToString() +
                "\r\n有效期至:" + DateTime.Now.ToString("yyyy/MM") + "至" + DateTime.Now.AddMonths(1).ToString("yyyy/MM") +
                "\r\n希望您度過一個愉快的生日，並感謝您一直以來的支持。";
            x.FEnable = true;

            _dbContext.TDcCouponLists.Add(x);
            _dbContext.SaveChanges();
            return x;
        }
    }
}

