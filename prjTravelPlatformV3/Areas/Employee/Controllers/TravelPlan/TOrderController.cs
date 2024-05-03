using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.TravelPlan
{
	[Area("Employee")]
	public class TOrderController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		public TOrderController(dbTravalPlatformContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult GetDetail(string? id)
		{
			var freeId = _context.TfreeOrders.FirstOrDefault(x=>x.ForderId==id).FfreeId;
			var h = (from x in _context.TfreePlanDetails
					join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
					where x.FfreeId == freeId
					orderby x.FtravelDay
					select new { x, y }).Select(a=>new TFreePlanDetailView()
					{
						FdestionationName = a.y.FdestinationName,
					});
			ViewBag.detail = h;


			//(TFreePlanDetailView)_context.TfreePlanDetails.Where(x => x.FfreeId == freeId).OrderBy(x => x.FtravelDay);
			//ViewBag.detail
			return PartialView("_DetailPartial");
		}
		[HttpGet]
		public IActionResult GetPartial(string? id)
		{
			ViewBag.Title = "編輯訂單";
			ViewBag.formId = "Edit";
			ViewBag.model = "EditContext";
			ViewBag.coupon = _context.TDcCouponLists.Where(x => x.FProductType == "旅遊").ToList();
			ViewBag.plan = _context.TtravelPlans.ToList();

			var dborder = _context.TfreeOrders.Find(id);
			var dbcustomer = _context.TCustomers.Find(dborder.FmemberId);
			var dbcoupon = _context.TDcCouponLists.Find(dborder.FcoupomId);
			var dbtravelplan = _context.TfreePlans.Find(dborder.FfreeId);
			TravelPlanOrderEditView h = new TravelPlanOrderEditView()
			{
				FOrderId = dborder.ForderId,
				FName = dbcustomer.FName,
				FMemeberId = dborder.FmemberId,
				FEmail = dbcustomer.FEmail,
				FPhone = dbcustomer.FPhone,
				FQty = dborder.Fqty,
				FOrderDate = dborder.ForderDate.Value.ToString("yyyy-MM-dd"),
				FCoupomName = dbcoupon.FCouponName,
				FPrice = (int?)dborder.Fprice,
			};
			return PartialView("_ModalPartial", h);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(TravelPlanOrderEditView userinfo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TfreeOrder h = _context.TfreeOrders.FirstOrDefault(x => x.ForderId == userinfo.FOrderId);
					{
						h.ForderDate = DateTime.Parse(userinfo.FOrderDate);
					};
					_context.Update(h);
					await _context.SaveChangesAsync();

					return Json(new { success = true, message = "資料修改成功" });
				}
				catch (Exception e)
				{
					return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
				}
			}        
			var errors = ModelState.ToDictionary(
				kvp => kvp.Key,
				kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
			);
			return Json(new
			{
				success = false,
				message = "資料驗證失敗",
				errors
			});
		}


		public IActionResult Del(string? id)
		{
			var order = _context.TfreeOrders.FirstOrDefault(x => x.ForderId == id);
			order.ForderSate = false;
			_context.Update(order);
			_context.SaveChanges();
			return Ok();
		}
	}
}
