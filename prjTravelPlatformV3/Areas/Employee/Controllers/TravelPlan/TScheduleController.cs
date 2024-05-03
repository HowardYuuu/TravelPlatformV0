using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.TravelPlan
{
	[Area("Employee")]
	public class TScheduleController : Controller
    {
		private readonly dbTravalPlatformContext _context;

		public TScheduleController(dbTravalPlatformContext context)
		{
			_context = context;
		}

		
		public IActionResult Index()
		{
			return View();
		}
		
	}
}
