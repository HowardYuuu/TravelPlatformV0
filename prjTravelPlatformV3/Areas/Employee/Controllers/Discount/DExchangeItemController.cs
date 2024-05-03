using CoreMVC_SignalR_Chat.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Discount;
using prjTravelPlatformV3.Models;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using prjTravelPlatform_release.Areas.Employee.Controllers.Discount;
using System.Net.Http.Headers;
using System.Text;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Discount
{
    [Area("Employee")]
    public class DExchangeItemController : Controller
    {
        string APIKEY = string.Empty;
        //private readonly HttpClient _httpClient;

        private readonly dbTravalPlatformContext _context;
        public DExchangeItemController(dbTravalPlatformContext context, IConfiguration conf)
        {
            _context = context;
            //_httpClient = httpClient;
            APIKEY = conf.GetSection("OPENAI_API_KEY").Value;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] RequiredImage obj)
        {
            
            string imglink = string.Empty;
            var response = new ResponseModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
                var Message = await client.PostAsync("https://api.openai.com/v1/images/generations", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
                if (Message.IsSuccessStatusCode)
                {
                    var content = await Message.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<ResponseModel>(content);
                    imglink = response.data[0].url.ToString();
                }
            }
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage([FromBody] ImageData imageData)
        {
            try
            {
                if (imageData == null || string.IsNullOrEmpty(imageData.Base64Data))
                {
                    return BadRequest("Invalid image data");
                }

                // 解码base64数据
                var bytes = Convert.FromBase64String(imageData.Base64Data);

                // 生成唯一的文件名
                var fileName = $"{Guid.NewGuid()}.png";

                // 设置保存路径
                var imagePath = Path.Combine("wwwroot", "img", "discount", "ad", fileName);

                // 写入文件
                await System.IO.File.WriteAllBytesAsync(imagePath, bytes);

                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        public class ImageData
        {
            public string Base64Data { get; set; }
        }



        public IActionResult GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增兌換商品資料";
                return PartialView("_ModalPartial", new ExcangeViewModel());
            }
            if (_context.TDcExchangeItems == null)
            {
                return NotFound();
            }
            var tExchange = _context.TDcExchangeItems.Find(id);
            if (tExchange == null)
            {
                return NotFound();
            }
            ExcangeViewModel E = new ExcangeViewModel
            {
                fId = tExchange.FProductId,
                fProductName = tExchange.FName,             
                fPoint = tExchange.FPointsRequired,
                fMoney= tExchange.FMoneyRequired,
                fQty = tExchange.FQuantity,
                fType = tExchange.FProductType,
                fImgPath=tExchange.FImagePath,
                fNote = tExchange.FNote,

            };
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯兌換商品資料";
            return PartialView("_ModalPartial", E);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TDcExchangeItems == null)
            {
                return Problem("Entity set 'dbTravalPlatformContext.TExchangeItems'  is null.");
            }
            var tExchange = await _context.TDcExchangeItems.FindAsync(id);
            if (tExchange != null)
            {
                _context.TDcExchangeItems.Remove(tExchange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TCouponListExists(int id)
        {
            return (_context.TDcExchangeItems?.Any(e => e.FProductId == id)).GetValueOrDefault();
        }


       
    }
}
