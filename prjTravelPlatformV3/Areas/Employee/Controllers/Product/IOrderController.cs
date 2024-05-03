using Microsoft.AspNetCore.Mvc;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Product
{
    [Area("Employee")]
    public class IOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderDetail(string orderId)
        {
            // 根據訂單編號從數據庫或其他地方檢索相關數據
            // 這裡僅作示例，實際情況可能需要從數據庫中查詢相關數據

            // 將訂單相關數據傳遞到視圖中
            ViewData["OrderId"] = orderId;

            // 返回帶有訂單詳細信息的視圖
            return View();
        }
    }
}
