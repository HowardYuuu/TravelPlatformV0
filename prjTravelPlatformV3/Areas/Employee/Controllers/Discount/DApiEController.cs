using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Text;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Discount;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Discount
{
    [Route("/api/Exchange/{action}/{id?}")]
    public class DApiEController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        public DApiEController(dbTravalPlatformContext context)
        {
            this._context = context;
        }

        public IActionResult GetData()
        {
            var exc = from e in _context.VDexchangeViews
                      select new 
                      {
                          fProduct_id = e.編號,
                          fName = e.商品名稱,
                          fPoints_Required = e.所需點數,
                          fMoney_Required = "$"+(int?)e.所需金額,
                          fProduct_type = e.庫存數量,
                          fQuantity = e.商品類型,
                          fImage_path = e.圖片,
                          fNote = e.備註
                      };
            return Json(exc);
        }
        public IActionResult GetById(int id)
        {
            var coupons = _context.TDcCouponLists.FirstOrDefault(h => h.FCouponId == id);
            return Json(coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExcangeViewModel exchange)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TDcExchangeItem tExchange = new TDcExchangeItem
					{
                        FProductId = exchange.fId,
                        FName= exchange.fProductName,
                        FPointsRequired = exchange.fPoint,
                        FMoneyRequired = exchange.fMoney,
                        FQuantity=exchange.fQty,
                        FProductType = exchange.fType,
                        FImagePath = exchange.fImgPath,
                        FNote = exchange.fNote,

                    };
                    _context.Add(tExchange);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "資料新增成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料新增失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExcangeViewModel exchange)
        {
            if (ModelState.IsValid)
            {
                try
                {
					TDcExchangeItem tExchange = new TDcExchangeItem
					{
                        FProductId = exchange.fId,
                        FName = exchange.fProductName,
                        FPointsRequired = exchange.fPoint,
                        FMoneyRequired = exchange.fMoney,
                        FProductType = exchange.fType,
                        FImagePath = exchange.fImgPath,
                        FNote = exchange.fNote,

                    };
                    _context.Update(tExchange);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "資料修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }
    }
}
