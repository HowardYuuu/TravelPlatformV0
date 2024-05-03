using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;
using System.Linq;

namespace prjTravelPlatform_release.Areas.Employee.Controllers.TravelPlan
{
	[Route("/api/TApi/{action}/{id?}")]
	public class TApiController : Controller
	{
		private readonly dbTravalPlatformContext _context;

		public TApiController(dbTravalPlatformContext context)
		{
			_context = context;
		}
		public IActionResult GetAllFreePlanOrder()
		{
			var Context = (from h in _context.TfreeOrders
						   join x in _context.TCustomers on h.FmemberId equals x.FCustomerId
						   join y in _context.TDcCouponLists on h.FcoupomId equals y.FCouponId
						   where h.ForderSate == true
						   where h.ForderDate > DateTime.Today
						   select new { h, x,y}).OrderBy(a => a.h.FmemberId)
						   .Select(a => new TravelPlanOrderView()
						   {
							   FOrderId = a.h.ForderId,
							   FName = a.x.FName,
							   FPhone = a.x.FPhone,
							   FQty = a.h.Fqty,
							   FOrderDate = a.h.ForderDate.Value.ToString("yyyy-MM-dd"),
							   FPrice = a.h.Fprice,
							   FCoupon_name = a.y.FCouponName
						   });
			return Json(Context);
		}
		public IActionResult GetAllDestionationOrder()
		{
			var Context = (from h in _context.TdestinationOrders
						   join x in _context.TCustomers on h.FmemberId equals x.FCustomerId
						   join y in _context.Tdestinations on h.FdestinationId equals y.FdestinationId
						   join z in _context.TDcCouponLists on h.Fcoupomid equals z.FCouponId
						   where h.ForderState == true
						   where h.ForderDate > DateTime.Now
						   select new { h, x, y, z }).OrderBy(a => a.h.ForderDate)
						   .Select(a => new DestionationOrderView()
						   {
							   FOrderId = a.h.ForderId,
							   FName = a.x.FName,
							   FEmail = a.x.FEmail,
							   FDestinationName = a.y.FdestinationName,
							   FQty = a.h.Fqty,
							   FOrderDate = a.h.ForderDate.Value.ToString("yyyy-MM-dd"),
							   FCoupon_name = a.z.FCouponName,
							   FPrice = a.h.Fprice,
							   FPhone = a.x.FPhone,
							   
						   });
			return Json(Context);
		}
		public IActionResult GetAllDestionation()
		{
			var Context = (from h in _context.Tdestinations
						   join x in _context.Ttypes on h.FdestinationTypeId equals x.FtypeId
						   join y in _context.TtravelAreas on h.FareaId equals y.FareaId
						   join z in _context.Tpriorities on h.Fpriority equals z.Fid
						   where h.FdestinationId != "0"
						   select new { h, x, y,z })
						   .OrderBy(a => a.h.Fstate)
						   .Select(a => new DestionationView()
						   {
							   FDestinationId = a.h.FdestinationId,
							   FDestinationName = a.h.FdestinationName,
							   FStock = a.h.Fstock,
							   Ftype = a.x.Ftype,
							   FAddress = a.h.Faddress,
							   FAreaName = a.y.FareaName,
							   FPrice = a.h.Fprice,
							   FDestinationContent = a.h.FdestinationContent,
							   FState = (bool)a.h.Fstate ? "上架中" : "已下架",
							   FPriority = a.z.FpriorityName,
						   });
			return Json(Context);
		}
		public IActionResult GetAllOnLineDestionation()
		{
			var Context = (from h in _context.Tdestinations
						   join x in _context.Ttypes on h.FdestinationTypeId equals x.FtypeId
						   join y in _context.TtravelAreas on h.FareaId equals y.FareaId
						   join z in _context.Tpriorities on h.Fpriority equals z.Fid
						   where h.FdestinationId != "0"
						   where h.Fstate == true
						   select new { h, x, y, z })
						   .OrderBy(a => a.h.Fstate)
						   .Select(a => new DestionationView()
						   {
							   FDestinationId = a.h.FdestinationId,
							   FDestinationName = a.h.FdestinationName,
							   FStock = a.h.Fstock,
							   Ftype = a.x.Ftype,
							   FAddress = a.h.Faddress,
							   FAreaName = a.y.FareaName,
							   FPrice = a.h.Fprice,
							   FDestinationContent = a.h.FdestinationContent,
							   FState = (bool)a.h.Fstate ? "上架中" : "已下架",
							   FPriority = a.z.FpriorityName,
						   });
			return Json(Context);
		}
		public IActionResult GetAllTravelPlan()
		{
			var Context = (from h in _context.TtravelPlans
						   join x in _context.Ttypes on h.FtravelTypeId equals x.FtypeId
						   join y in _context.TtravelAreas on h.FareaId equals y.FareaId
						   where h.Fself != true
						   select new { h, x, y })
							.OrderBy(a => a.h.Fstate)
							.Select(a => new TravelplanView()
							{
								FTravelId = a.h.FtravelId,
								FTravelName = a.h.FtravelName,
								FTravelType = a.x.Ftype,
								FDay = a.h.Fday,
								//FTransport = a.h.Ftransport,
								FArea = a.y.FareaName,
								FPrice = a.h.Fprice,
								FStock = a.h.Fstock,
								FTravelPlanContent = a.h.FtravelContent,
								FState = (bool)a.h.Fstate ? "上架中" : "已下架"
							});
			return Json(Context);
		}

		[HttpGet]
		public IActionResult ShowPic(string id)
		{
			var images = _context.TdestinationPhotos
								.Where(photo => photo.FdestinationId == id)
								.ToList();

			var imageBase64List = new List<string>();

			foreach (var image in images)
			{
				var base64String = Convert.ToBase64String(image.FphotoPath);
				imageBase64List.Add(base64String);
			}

			return Json(imageBase64List);
		}
	}
}
