using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Discount;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;
using System.Text;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Discount
{
    [Area("Employee")]
    public class DCouponController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        public DCouponController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public static string GenerateCouponCode(int length)
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder couponCode = new StringBuilder();

            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(characters.Length);
                couponCode.Append(characters[index]);
            }

            return couponCode.ToString();
        }
        public IActionResult GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增優惠卷資料";
                return PartialView("_DModalPartial", new CouponViewModel());
            }
            if (_context.TDcCouponLists== null)
            {
                return NotFound();
            } 
            var tCoupon = _context.TDcCouponLists.Find(id);
            if (tCoupon == null)
            {
                return NotFound();
            }
            CouponViewModel C = new CouponViewModel
            {
                CouponId = tCoupon.FCouponId,
                CouponCode = GenerateCouponCode(10),
                CouponName = tCoupon.FCouponName,
                Amount = tCoupon.FAmount,
                StartDate = tCoupon.FStartDate,
                EndDate = tCoupon.FEndDate,
                //Rule = tCoupon.FRule,
                ProductType= tCoupon.FProductType,
                Discount = tCoupon.FDiscount,
                //Note = tCoupon.FNote,
                Enable = tCoupon.FEnable
            };
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯優惠卷資料";
            return PartialView("_DModalPartial", C);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TDcCouponLists == null)
            {
                return Problem("Entity set 'dbTravalPlatformContext.TCouponLists'  is null.");
            }
            var tCoupon = await _context.TDcCouponLists.FindAsync(id);
            if (tCoupon != null)
            {
                _context.TDcCouponLists.Remove(tCoupon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TCouponListExists(int id)
        {
            return (_context.TDcCouponLists?.Any(e => e.FCouponId == id)).GetValueOrDefault();
        }
    }
}
