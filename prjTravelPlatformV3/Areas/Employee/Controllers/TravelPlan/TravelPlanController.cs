using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.TravelPlan
{
	[Area("Employee")]
	public class TravelPlanController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		public TravelPlanController(dbTravalPlatformContext context)
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
			ViewBag.type = _context.Ttypes.ToList();
			ViewBag.area = _context.TtravelAreas.ToList();
			if (id == "0" || id == null)
			{
				ViewBag.Title = "新增方案";
				ViewBag.formId = "Create";
				ViewBag.model = "AddContext";

				return PartialView("_ModalPartial", new TravelplanEditView());
			}
			ViewBag.Title = "編輯景點";
			ViewBag.formId = "Edit";
			ViewBag.model = "EditContext";

			var db = _context.TtravelPlans.Find(id);
			TravelplanEditView h = new TravelplanEditView()
			{
				FAreaId = db.FareaId,
				FDay = db.Fday,
				FPrice = db.Fprice,
				FState = db.Fstate,
				FStock = db.Fstock,
				//FTransport = db.Ftransport,
				FTravelContent = db.FtravelContent,
				FTravelId = db.FtravelId,
				FTravelName = db.FtravelName,
				FTypeId = db.FtravelTypeId

			};
			return PartialView("_ModalPartial", h);
		}

		public IActionResult GetMorDestionation(string? id, int? day)
		{
			if (day == null)
			{
				day = 1;
			}

			var checkContext = (from x in _context.TdestinationDetails
								join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
								where x.FtravelId == id
								where x.FtravelDay == day
								where x.FdestionationState == true
								select y).FirstOrDefault();
			if (checkContext == null)
			{
				TdestinationDetail detail = new TdestinationDetail();
				detail.FtravelId = id;
				detail.FdestinationId = "0";
				detail.FdestionationState = true;
				detail.FtravelDay = day;
				detail.FdestionationTime = DateTime.Now.TimeOfDay;
				_context.Add(detail);
				_context.SaveChanges();
			}
			var Context = from x in _context.TdestinationDetails
						  join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
						  where x.FtravelId == id
						  where x.FtravelDay == day
						  where x.FdestionationState == true
						  select new { x.FdestinationId, y.FdestinationName, x.FdestionationTime };
			return Json(Context);
		}
		public IActionResult GetAfterDestionation(string? id, int? day)
		{
			if (day == null)
			{
				day = 1;
			}
			var checkContext = (from x in _context.TdestinationDetails
								join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
								where x.FtravelId == id
								where x.FtravelDay == day
								where x.FdestionationState == false
								select y).FirstOrDefault();
			if (checkContext == null)
			{
				TdestinationDetail detail = new TdestinationDetail();
				detail.FtravelId = id;
				detail.FdestinationId = "0";
				detail.FdestionationState = false;
				detail.FtravelDay = day;
				detail.FdestionationTime = DateTime.Now.TimeOfDay;
				_context.Add(detail);
				_context.SaveChanges();
			}
			var Context = from x in _context.TdestinationDetails
						  join y in _context.Tdestinations on x.FdestinationId equals y.FdestinationId
						  where x.FtravelId == id
						  where x.FtravelDay == day
						  where x.FdestionationState == false
						  select new { x.FdestinationId, y.FdestinationName, x.FdestionationTime };
			return Json(Context);
		}

		public IActionResult UpdateDestionationMorning(string? id, string des, int? day, TimeSpan time)
		{
			TdestinationDetail h = _context.TdestinationDetails.FirstOrDefault(x => x.FtravelId == id & x.FtravelDay == day & x.FdestionationState == true);
			{
				h.FdestinationId = des;
				h.FdestionationTime = time;
			}
			_context.Update(h);
			_context.SaveChanges();
			return Ok();
		}
		public IActionResult UpdateDestionationAfter(string? id, string des, int? day, TimeSpan time)
		{
			TdestinationDetail h = _context.TdestinationDetails.FirstOrDefault(x => x.FtravelId == id & x.FtravelDay == day & x.FdestionationState == false);
			{
				h.FdestinationId = des;
				h.FdestionationTime = time;
			}
			_context.Update(h);
			_context.SaveChanges();
			return Ok();
		}
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TravelplanEditView userinfo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TtravelPlan des = new TtravelPlan();
					des.FtravelId = Guid.NewGuid().ToString();

					var destinationDetails = _context.TdestinationDetails
						.Where(d => d.FtravelId == des.FtravelId)
						.ToList();
					int DayCount = destinationDetails.Count;

					var hoteldetails = _context.ThotelDetails
						.Where(d => d.FtravelId == des.FtravelId)
						.ToList();
					int NightCount = hoteldetails.Count + 1;

					des.FtravelName = userinfo.FTravelName;
					des.FtravelContent = userinfo.FTravelContent;
					des.FtravelTypeId = userinfo.FTypeId;
					des.Fday = userinfo.FDay;
					//des.Ftransport = userinfo.FTransport;
					des.FareaId = userinfo.FAreaId;
					des.Fprice = userinfo.FPrice;
					des.Fstock = userinfo.FStock;
					des.Fstate = DayCount == des.Fday * 2 && NightCount == des.Fday;
					des.Fself = false;
					_context.Add(des);

					await _context.SaveChangesAsync();

					return Json(new { success = true, message = "資料新增成功" });
				}
				catch (Exception e)
				{
					return Json(new { success = false, message = $"資料新增失敗：{e.Message}" });
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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(TravelplanEditView userinfo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TtravelPlan h = _context.TtravelPlans.FirstOrDefault(x => x.FtravelId == userinfo.FTravelId);
					{
						var destinationDetails = _context.TdestinationDetails
						.Where(d => d.FtravelId == h.FtravelId)
						.ToList();
						int DayCount = destinationDetails.Count;

						var hoteldetails = _context.ThotelDetails
							.Where(d => d.FtravelId == h.FtravelId)
							.ToList();
						int NightCount = hoteldetails.Count + 1;


						h.FtravelName = userinfo.FTravelName;
						h.FtravelTypeId = userinfo.FTypeId;
						h.FtravelContent = userinfo.FTravelContent;
						h.Fday = userinfo.FDay;
						//h.Ftransport = userinfo.FTransport;
						h.FareaId = userinfo.FAreaId;
						h.Fprice = userinfo.FPrice;
						h.Fstock = userinfo.FStock;
						h.Fstate = DayCount == userinfo.FDay * 2 && NightCount == userinfo.FDay;
						h.Fself = false;
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
		public IActionResult Change(string id)
		{
			var plan = _context.TtravelPlans.FirstOrDefault(x => x.FtravelId == id);
			var destinationDetails = _context.TdestinationDetails
						.Where(d => d.FtravelId == plan.FtravelId && d.FdestinationId != "0")
						.ToList();
			int DayCount = destinationDetails.Count;

			//var hoteldetails = _context.ThotelDetails
			//	.Where(d => d.FtravelId == plan.FtravelId)
			//	.ToList();
			//int NightCount = hoteldetails.Count + 1;

			if (plan.Fstate == false)
			{
				if (DayCount == plan.Fday * 2 /*&& NightCount == plan.Fday*/)
				{
					plan.Fstate = !plan.Fstate;
					_context.Update(plan);
					_context.SaveChanges();
					return Ok(true);
				}
				return Ok(false);
			}

			plan.Fstate = !plan.Fstate;
			_context.Update(plan);
			_context.SaveChanges();
			return Ok(true);
		}
	}
}
