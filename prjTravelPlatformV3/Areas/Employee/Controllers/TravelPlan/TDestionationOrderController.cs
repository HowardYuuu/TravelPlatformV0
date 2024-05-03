using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;


namespace prjTravelPlatform_release.Areas.Employee.Controllers.TravelPlan
{
	[Area("Employee")]
	public class TDestionationOrderController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		public TDestionationOrderController(dbTravalPlatformContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult GetPartial(string? id)
		{
			ViewBag.Title = "編輯訂單";
			ViewBag.formId = "Edit";
			ViewBag.model = "EditContext";
			ViewBag.coupon = _context.TDcCouponLists.Where(x => x.FProductType == "旅遊").ToList();
			ViewBag.destionation = _context.Tdestinations.ToList();
			ViewBag.plan = _context.Tdestinations.ToList();
			var dborder = _context.TdestinationOrders.Find(id);
			var dbcustomer = _context.TCustomers.Find(dborder.FmemberId);
			var dbcoupon = _context.TDcCouponLists.Find(dborder.Fcoupomid);
			DestionationOrderEditView h = new DestionationOrderEditView()
			{
				FOrderId = dborder.ForderId,
				FName = dbcustomer.FName,
				FMemeberId = dborder.FmemberId,
				FDestionationId = dborder.FdestinationId,
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
		public async Task<IActionResult> Edit(DestionationOrderEditView userinfo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TdestinationOrder h = _context.TdestinationOrders.FirstOrDefault(x => x.ForderId == userinfo.FOrderId);
					{
						h.FdestinationId = userinfo.FDestionationId;
						h.Fqty = userinfo.FQty;
						h.ForderDate = DateTime.Parse(userinfo.FOrderDate);
						h.Fprice = userinfo.FPrice;
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
			var order = _context.TdestinationOrders.FirstOrDefault(x=>x.ForderId==id);
			order.ForderState = false;
			_context.Update(order);
			_context.SaveChanges();
			return Ok();
		}
	}
}
