using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan;
using prjTravelPlatformV3.Models;
using System.Linq;
using System.Security.Claims;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.TravelPlan
{
	[Area("Customer")]
	public class TravelPlanController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public TravelPlanController(dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Detail()
		{
			return View();
		}

		[Authorize]
		public IActionResult Free()
		{
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}
			ViewBag.type = _context.Ttypes;
			ViewBag.area = _context.TtravelAreas;
			//ViewBag.Coupon = _context.TDcCouponLists.Where(x => x.FProductType == "旅遊");
			if (_context.TfreePlans.FirstOrDefault(x => x.FmemberId == customer && x.Fstate == false) == null)
			{
				TfreePlan plan = new TfreePlan();
				{
					plan.FfreeId = Guid.NewGuid().ToString();
					plan.Fstate = false;
					plan.FmemberId = customer;
				}
				_context.Add(plan);
				_context.SaveChanges();
			}
			ViewBag.freeid = _context.TfreePlans.FirstOrDefault(x => x.FmemberId == customer && x.Fstate == false).FfreeId;
			ViewBag.customer = customer;

			//ViewBag.Day = day;
			//ViewBag.morning = _context.TfreePlanDetails.Where(x => x.FcustomerId == customer).Where(x => x.FdestionationState == true).OrderBy(x => x.FtravelDay);
			//ViewBag.after = _context.TfreePlanDetails.Where(x => x.FcustomerId == customer).Where(x => x.FdestionationState == false).OrderBy(x => x.FtravelDay);

			return View();
		}
		public IActionResult Day()
		{
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}
			var free = _context.TfreePlans.FirstOrDefault(x => x.FmemberId == customer && x.Fstate == false)?.FfreeId;
			if (string.IsNullOrEmpty(free))
			{
				return Json("1");
			}

			var day = _context.TfreePlanDetails.Where(x => x.FfreeId == free).Count();
			day = day / 2;


			return Json(day);
		}
		public IActionResult FreeDetail()
		{
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}
			var free = _context.TfreePlans.FirstOrDefault(x => x.FmemberId == customer && x.Fstate == false)?.FfreeId;
			if (string.IsNullOrEmpty(free))
			{
				TfreePlan tfp = new TfreePlan()
				{
					FfreeId = Guid.NewGuid().ToString(),
					FmemberId = customer,
					Fstate = false
				};
				_context.Add(tfp);
				_context.SaveChanges();
				return Json(new List<TFreePlanDetailView>());
			}
			var day = (from x in _context.TfreePlanDetails
					   join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
					   where x.FfreeId == free
					   orderby x.FtravelDay
					   select new TFreePlanDetailView()
					   {
						   
						   FdestinationDetailId = x.FdestinationDetailId,
						   FcustomerId = x.FcustomerId,
						   FdestinationId = x.FdestinationId,
						   FdestionationName = y.FdestinationName,
						   FdestionationState = x.FdestionationState,
						   FdestionationTime = x.FdestionationTime,
						   FtravelDay = x.FtravelDay,
						   FreeId = x.FfreeId
						   
					   }).ToList();
			return Json(day);
		}
		[Authorize]
		public IActionResult FreePlanDeal(string id)
		{
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}
			ViewBag.FreeId = id;
			ViewBag.Customer = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

			var coupont = from x in _context.TDcCouponLists
						  where x.FProductType == "旅遊"
						  select x.FCouponCode;

			//int[] num = { 3, 4, 5, 6 };
			var dccqv = from x in _context.TDcCusCouponQties
						join y in _context.TDcCouponLists on x.FCouponId equals y.FCouponId
						where x.FCustomerId == customer
						where x.FUsed == false
						where coupont.Contains(y.FCouponCode)
						select new TDcCusCouponQtyView()
						{
							FUsed = x.FUsed,
							FCustomerId = customer,
							FCouponId = x.FCouponId,
							FCouponName = y.FCouponName,
							FEndDate = y.FEndDate,
							FStartDate = y.FStartDate,
						};
			ViewBag.Coupon = dccqv;

			var detail = from x in _context.TfreePlanDetails
						  join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
						  where x.FfreeId == id
						  select new TFreePlanDetailView()
						  {
							  //FdestinationDetailId = x.FdestinationDetailId,
							  FcustomerId = x.FcustomerId,
							  FdestinationId = x.FdestinationId,
							  FdestionationName = y.FdestinationName,
							  FdestionationState = x.FdestionationState,
							  FdestionationTime = x.FdestionationTime,
							  FtravelDay = x.FtravelDay,
							  FreeId = x.FfreeId,
							  FPrice = y.Fprice,
						  };
			int money = 0;
			foreach ( var item in detail )
			{
				money = money + (int)item.FPrice;
			}
			ViewBag.totalmoney = money;
			return View(detail);
		}

		[HttpPost]
		public IActionResult SaveFreeDetail([FromBody] List<TFreePlanDetailView> daydate)
		{
			if (daydate == null)
				return Ok();
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}
			var detail = _context.TfreePlanDetails	
				.Where(x => x.FcustomerId == customer && x.FfreeId == daydate[1].FreeId)	
				.OrderBy(x => x.FtravelDay)	
				.ToList();
			_context.RemoveRange(detail);
			int i = 0;
			foreach(var x in daydate)
			{

				TfreePlanDetail tfpd = new TfreePlanDetail()
				{
					FdestinationId = daydate[i].FdestinationId,
					FcustomerId = daydate[i].FcustomerId,
					//FdestinationId = daydate[i].FdestinationId,
					FtravelDay = daydate[i].FtravelDay,
					FfreeId = daydate[i].FreeId,
					FdestionationTime = daydate[i].FdestionationTime,
					FdestionationState = daydate[i].FdestionationState,
				};
				_context.Add(tfpd);
				i++;
			}
			_context.SaveChanges();
			//var old = detail.ToList();
			//int z = old.Count();
			//int y = daydate.Count(); 
			//if (z < y)
			//{
			//	int i = 0;
			//	for (i = 0; i < y - 1; i++)
			//	{
			//		if (i > z - y)
			//		{
			//			TfreePlanDetail tfpd = new TfreePlanDetail()
			//			{
			//				FcustomerId = daydate[i].FcustomerId,
			//				//FdestinationId = daydate[i].FdestinationId,
			//				FtravelDay = daydate[i].FtravelDay,
			//				FfreeId = daydate[i].FreeId,
			//				FdestionationTime = daydate[i].FdestionationTime,
			//				FdestionationState = daydate[i].FdestionationState,
			//			};
			//			_context.Add(tfpd);
			//		}
			//		else
			//		{
			//			old[i].FdestinationId = daydate[i].FdestinationId;
			//			old[i].FdestionationTime = daydate[i].FdestionationTime;
			//			_context.Update(old[i]);
			//		}
			//	}
			//}
			//else if (z > y)
			//{
			//	int i = 0;
			//	for (i = 0; i < z - 1; i++)
			//	{
					
			//		if (i > z - y)
			//		{
			//			_context.Remove(old[i]);
			//		}
			//		else
			//		{
			//			old[i].FdestinationId = daydate[i].FdestinationId;
			//			old[i].FdestionationTime = daydate[i].FdestionationTime;
			//			_context.Update(old[i]);
			//		}
			//	}
			//}
			//else
			//{
			//	int i = 0;
			//	for (i = 0; i < z - 1; i++)
			//	{
			//		old[i].FdestinationId = daydate[i].FdestinationId;
			//		old[i].FdestionationTime = daydate[i].FdestionationTime;
			//		_context.Update(old[i]);
			//	}
				

			//}
			//_context.SaveChanges();

			var deal = _context.TfreePlans.FirstOrDefault(x => x.FmemberId == customer && x.FfreeId == daydate[1].FreeId);

			return Ok();
		}


		public IActionResult Order(string id, int qty, string date, int coupon)
		{
			if (id == null || qty == null || date == null || coupon == null || date == "年/月/日")
			{
				return Ok(false);
			}
			var customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;

			var freeplan = _context.TfreePlans.FirstOrDefault(x => x.FfreeId == id);
			freeplan.Fstate = true;
			

			var detail = from x in _context.TfreePlanDetails
						 where x.FfreeId == id
						 select x;
			int money = 0;
			var destinations = _context.Tdestinations.ToList(); // 將查詢結果讀取到列表中

			foreach (var x in detail)
			{
				var destination = destinations.FirstOrDefault(y => y.FdestinationId == x.FdestinationId);
				if (destination != null)
				{
					money += (int)destination.Fprice;
				}
			}
			var couponmoney = _context.TDcCouponLists.FirstOrDefault(x => x.FCouponId == coupon).FDiscount;
			if (couponmoney == null)
			{
				couponmoney = 0;
			}
			int price = (money * qty) - (int)couponmoney;
			DateTime dates = DateTime.Parse(date);
			TfreeOrder t = new TfreeOrder()
			{
				ForderId = Guid.NewGuid().ToString(),
				FmemberId = customer,
				FfreeId = id,
				Fqty = qty,
				ForderDate = dates,
				FcoupomId = coupon,
				Fprice = price,
				ForderSate = true
			};
			_context.Update(freeplan);
			_context.Add(t);
			_context.SaveChanges();
			return Ok(true);
		}
	}
}
