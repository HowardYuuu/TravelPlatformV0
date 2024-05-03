using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiOrderController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public ApiOrderController(dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        //public IActionResult GetHotelOrder()
        //{
        //    var hotelorders=_context.THorders
        //}
        
    }
}
