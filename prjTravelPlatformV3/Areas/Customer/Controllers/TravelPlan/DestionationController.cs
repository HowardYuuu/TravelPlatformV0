using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
using prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.TravelPlan
{
	[Area("Customer")]
	public class DestionationController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public DestionationController(dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		public IActionResult Index(string type)
		{
			var Recommend = from i in _context.Tdestinations
							let j = _context.TdestinationPhotos.FirstOrDefault(y => y.FdestinationId == i.FdestinationId)
							where i.Fpriority == 2 && i.FdestinationId != "0"
							select new TDestinationPhotoView()
							{
								FdestinationId = i.FdestinationId,
								FdestinationName = i.FdestinationName,
								FphotoId = j.FphotoId,
								FphotoPath = j.FphotoPath,
							};
			ViewBag.recommend = Recommend;
			ViewBag.search = type;
			return View();
		}

		public IActionResult Type()
		{
			var type = from x in _context.Ttypes
					   select x;
			return Json(type);
		}

		public IActionResult Area()
		{
			var area = from x in _context.TtravelAreas
					   select x;
			return Json(area);
		}

		private IQueryable<DestionationList> GetDestinationListQuery(int page, string type, string place, string sort, bool follow, string keyword)
		{
			var customer = 0;
			if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null)
			{
				customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			}

			var query = (from x in _context.Tdestinations
						 let y = _context.TdestinationPhotos.FirstOrDefault(y => y.FdestinationId == x.FdestinationId)
						 let z = _context.TdestionationFollows.FirstOrDefault(i => i.FdestionationId == x.FdestinationId && i.FcustomerId == customer)
						 let totalFollows = _context.TdestionationFollows.Count(i => i.FdestionationId == x.FdestinationId)
						 where x.FdestinationId != "0"
						 where x.Fstate == true
						 select new DestionationList()
						 {
							 FDestinationId = x.FdestinationId,
							 FDestinationName = x.FdestinationName,
							 FtypeId = x.FdestinationTypeId,
							 Ftype = x.FdestinationType.Ftype,
							 FPrice = x.Fprice,
							 FareaId = x.FareaId,
							 FAreaName = x.Farea.FareaName,
							 FCount = x.Fcount,
							 FphotoPath = y.FphotoPath,
							 FFollow = z.Ffollow,
							 FPriority = x.Fpriority,
							 FTotalFollow = totalFollows,
						 });

			if (type != null && type.Length > 2)
			{
				var typeArray = JsonConvert.DeserializeObject<int[]>(type);
				query = query.Where(item => typeArray.Select(x => (int?)x).Contains(item.FtypeId));
			}
			if (keyword != null && keyword != "")
			{
				var search = query.Where(x =>
				x.FDestinationName.Contains(keyword) || x.FAreaName.Contains(keyword) || x.Ftype.Contains(keyword)
				);
				if (search != null)
				{
					query = search;
				}
			}
			if (place != null && place.Length > 2)
			{
				var areaArray = JsonConvert.DeserializeObject<int[]>(place);
				query = query.Where(item => areaArray.Select(x => (int?)x).Contains(item.FareaId));
			}
			if (follow)
			{
				query = query.Where(item => item.FFollow == true);
			}
			switch (sort)
			{
				case "follow":
					query = query.OrderByDescending(item => item.FTotalFollow);
					break;
				case "az":
					query = query.OrderByDescending(item => item.FPrice);
					break;
				case "za":
					query = query.OrderBy(item => item.FPrice);
					break;
				case "latest":
					query = query.OrderBy(item => item.FPriority);
					break;
				default:
					break;
			}
			return query;
		}

		//更新景點內容
		public IActionResult List(int page, string type, string place, string sort, bool follow, string keyword)
		{
			if (page == null || page == 0)
			{
				page = 1;
			}
			var query = GetDestinationListQuery(page, type, place, sort, follow, keyword);
			page--;
			var pageSize = 9;
			var pagedResult = query.Skip(page * pageSize).Take(pageSize).ToList();

			return Json(pagedResult);
		}

		//更新自由行內容
		public IActionResult List2(int page, string type, string place, string sort, bool follow, string keyword)
		{
			if (page == null || page == 0)
			{
				page = 1;
			}
			var query = GetDestinationListQuery(page, type, place, sort, follow, keyword);
			page--;
			var pageSize = 6;
			var pagedResult = query.Skip(page * pageSize).Take(pageSize).ToList();

			return Json(pagedResult);
		}

		//生成景點當前頁數內容
		public IActionResult Page(int page, string type, string place, string sort, bool follow, string keyword)
		{
			if (page == null || page == 0)
			{
				page = 1;
			}
			var query = GetDestinationListQuery(page, type, place, sort, follow, keyword);
			var pagedResult = query.Count();
			int totalpage;
			if ((pagedResult % 9) == 0)
			{
				totalpage = pagedResult / 9;
			}
			else
			{
				totalpage = (pagedResult / 9) + 1;
			}

			return Json(totalpage);
		}

		//生成自由行當前頁數內容
		public IActionResult Page2(int page, string type, string place, string sort, bool follow, string keyword)
		{
			if (page == null || page == 0)
			{
				page = 1;
			}
			var query = GetDestinationListQuery(page, type, place, sort, follow, keyword);
			var pagedResult = query.Count();
			int totalpage;
			if ((pagedResult % 6) == 0)
			{
				totalpage = pagedResult / 6;
			}
			else
			{
				totalpage = (pagedResult / 6) + 1;
			}

			return Json(totalpage);
		}

		//追蹤功能
		public IActionResult Follow(string id)
		{
			TdestionationFollow check = _context.TdestionationFollows.FirstOrDefault(x => x.Fcustomer.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) && x.FdestionationId == id && x.Ffollow == true);
			if (check != null)
			{
				_context.Remove(check);
				_context.SaveChanges();
				return Ok(false);
			}
			var customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			
			
			TdestionationFollow tr = new TdestionationFollow()
			{
				FdestionationId = id,
				FcustomerId = customer,
				Ffollow = true
			};
			_context.Add(tr);
			_context.SaveChanges();

			return Ok(true);
		}

		//景點明細畫面
		public IActionResult Detail(string id)
		{

			var detail = from x in _context.Tdestinations
						 join y in _context.Ttypes on x.FdestinationTypeId equals y.FtypeId
						 join z in _context.TtravelAreas on x.FareaId equals z.FareaId
						 where x.FdestinationId == id
						 select new DestionationList()
						 {
							 FDestinationId = x.FdestinationId,
							 FDestinationName = x.FdestinationName,
							 FtypeId = x.FdestinationTypeId,
							 Ftype = x.FdestinationType.Ftype,
							 FPrice = x.Fprice,
							 FareaId = x.FareaId,
							 FAreaName = x.Farea.FareaName,
							 FCount = x.Fcount,
							 FAddress = x.Faddress,
							 FDestinationContent = x.FdestinationContent,
						 };
			DestionationList card = detail.FirstOrDefault();
			ViewBag.Photo = from x in _context.TdestinationPhotos
							where x.FdestinationId == id
							select x;
			var remark  = (from x in _context.TdestinationRemarks
							 join y in _context.TdestinationOrders on x.ForderId equals y.ForderId
							 join z in _context.Tdestinations on y.FdestinationId equals z.FdestinationId
							 join i in _context.TCustomers on y.FmemberId equals i.FCustomerId
							 where z.FdestinationId == id
							 where y.ForderState == true
							 orderby x.Fstar descending
							 select new TdestinationRemarkView()
							 {
								 Fstar = x.Fstar,
								 FremarkId = x.FremarkId,
								 Fremark = x.Fremark,
								 FCustomer = i.FName,
								 FCustomerPhoto = i.FImagePath,
							 }).Take(3).ToList();

			ViewBag.Remark = remark;

			int? totalStarSum = (from x in _context.TdestinationRemarks
								join y in _context.TdestinationOrders on x.ForderId equals y.ForderId
								where y.FdestinationId == id
								select x.Fstar).Sum();

			int? totalStarCount = (from x in _context.TdestinationRemarks
								  join y in _context.TdestinationOrders on x.ForderId equals y.ForderId
								  where y.FdestinationId == id
								  select x.Fstar).Count();

			if (totalStarSum.HasValue && totalStarCount.HasValue && totalStarCount.Value != 0)
			{
				double averageStar = Math.Round((double)totalStarSum.Value / totalStarCount.Value, 2);
				// 如果要顯示整數值，可以再將結果轉換為int類型
				int roundedAverageStar = (int)Math.Round(averageStar);
				ViewBag.Star = roundedAverageStar;
			}
			else
			{
				ViewBag.Star = 0;
			}

			TdestionationFollow check = _context.TdestionationFollows.FirstOrDefault(x => x.Fcustomer.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) && x.FdestionationId == id && x.Ffollow == true);
			if (check == null)
			{
				ViewBag.Follow = false;
			}
			else
			{
				ViewBag.Follow = true;
			}
			var count = _context.Tdestinations.FirstOrDefault(x => x.FdestinationId == id);
			count.Fcount++;
			_context.Update(count);
			_context.SaveChanges();

			return View(card);
		}

		//結帳畫面
		[Authorize]
		public IActionResult Deal(string id, int qty = 1)
		{
			var customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			ViewBag.Customer = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
			ViewBag.Qty = qty;
			int[] num = { 3, 4, 5, 6 };
			var dccqv = from x in _context.TDcCusCouponQties
						join y in _context.TDcCouponLists on x.FCouponId equals y.FCouponId
						where x.FCustomerId == customer
						where x.FUsed == false
						where num.Contains(x.FCouponId)
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
			var detail = _context.Tdestinations.FirstOrDefault(x => x.FdestinationId == id);

			return View(detail);
		}

		//優惠券折購
		public IActionResult Coupon(int id)
		{
			decimal? cp = _context.TDcCouponLists.FirstOrDefault(x => x.FCouponId == id).FDiscount;
			return Json(cp);
		}

		//成立訂單
		public IActionResult Order(string id, int qty, string date, int coupon)
		{
			if (id == null || qty == null || date == null || coupon == null || date == "年/月/日")
			{
				return Ok(false);
			}
			var customer = _context.TCustomers.FirstOrDefault(x => x.FName == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)).FCustomerId;
			var destionation = _context.Tdestinations.FirstOrDefault(x => x.FdestinationId == id).Fprice;
			var couponmoney = _context.TDcCouponLists.FirstOrDefault(x => x.FCouponId == coupon).FDiscount;
			if (couponmoney == null)
			{
				couponmoney = 0;
			}
			int price = ((int)destionation * qty) - (int)couponmoney;
			DateTime dates = DateTime.Parse(date);
			TdestinationOrder t = new TdestinationOrder()
			{
				ForderId = Guid.NewGuid().ToString(),
				FmemberId = customer,
				FdestinationId = id,
				Fqty = qty,
				ForderDate = dates,
				Fcoupomid = coupon,
				Fprice = price,
				ForderState = true
			};
			if(coupon != 16)
			{
				TDcCusCouponQty a = _context.TDcCusCouponQties.FirstOrDefault(x => x.FCustomerId == customer && x.FCouponId == coupon);
				a.FUsed = true;
				_context.Update(a);
			}
			_context.Add(t);
			_context.SaveChanges();

			return Ok(true);
		}

		//判定是否有登入
		public bool IsNullLogin()
		{
			return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) != null;
		}


		//Angulr
		[HttpGet]
		public async Task<IActionResult> Remark(string id, int star)
		{
			var remark = await (from x in _context.TdestinationRemarks
								join y in _context.TdestinationOrders on x.ForderId equals y.ForderId
								join z in _context.Tdestinations on y.FdestinationId equals z.FdestinationId
								join i in _context.TCustomers on y.FmemberId equals i.FCustomerId
								where z.FdestinationId == id
								where y.ForderState == true
								where x.Fstar == star
								select new TdestinationRemarkView()
								{
									Fstar = x.Fstar,
									FremarkId = x.FremarkId,
									Fremark = x.Fremark,
									FCustomer = i.FName,
									imageBytes = Convert.ToBase64String(
										System.IO.File.ReadAllBytes(
											string.IsNullOrEmpty(i.FImagePath) ?
											"wwwroot/img/uploads/avatar.png" :
											"wwwroot/" + i.FImagePath))
								}).ToListAsync();
			return Ok(remark);
		}
	}
}
