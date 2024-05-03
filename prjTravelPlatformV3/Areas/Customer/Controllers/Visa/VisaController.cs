using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Models;
using System.Security.Claims;
using prjTravelPlatformV3.Areas.Customer.ViewModels.Visa;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using System.Xml;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Visa
{
    [Area("Customer")]
    public class VisaController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public VisaController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult Index()
        {
            List<string> 台灣 = new List<string>
            {
                "台灣"
            };
            List<object> regions = new List<object>
            {
                "台灣"
            };
            var r = _context.TVcountries.Select(t => t.FRegion).Distinct().ToList();
            regions.AddRange(r);
            var 國家列表 = _context.TVcountries.ToList();
            var 亞洲 = _context.TVcountries.Where(t => t.FRegion == "亞洲").ToList();
            var 北美洲 = _context.TVcountries.Where(t => t.FRegion == "北美洲").ToList();
            var 中南美洲 = _context.TVcountries.Where(t => t.FRegion == "中南美洲").ToList();
            var 歐洲 = _context.TVcountries.Where(t => t.FRegion == "歐洲").ToList();
            var 非洲 = _context.TVcountries.Where(t => t.FRegion == "非洲").ToList();
            var 大洋洲 = _context.TVcountries.Where(t => t.FRegion == "大洋洲").ToList();
            ViewBag.Regions = regions;
            ViewData["國家列表"] = 國家列表;
            ViewData["亞洲"] = 亞洲;
            ViewData["北美洲"] = 北美洲;
            ViewData["中南美洲"] = 中南美洲;
            ViewData["歐洲"] = 歐洲;
            ViewData["非洲"] = 非洲;
            ViewData["大洋洲"] = 大洋洲;
            ViewData["台灣"] = 台灣;
            return View();
        }
        public IActionResult Map()
        {
            return View();
        }

        public IActionResult List()
        {
            var 國家列表 = _context.TVcountries.ToList();
            ViewData["國家列表"] = 國家列表;
            return View();
        }

        public IActionResult ProductSelect()
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (CustomerId == 0)
            {
                return RedirectToAction("Index", "Login", new { area = "CustomizedIdentity" });
            }
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return View();
        }


        public IActionResult InfoInput()
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (CustomerId == 0)
            {
                return RedirectToAction("Index", "Login", new { area = "CustomizedIdentity" });
            }
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return View();
        }


        public IActionResult OrderConfirm()
        {
            var CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (CustomerId == 0)
            {
                return RedirectToAction("Index", "Login", new { area = "CustomizedIdentity" });
            }
            ViewBag.CustomerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return View();
        }

        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult ChatExample()
        {
            return View();
        }

        //新增訂單
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VOrderViewModel vo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TVorder tVorder = new TVorder
                    {
                        FId = vo.FId,
                        FProductId = vo.FProductId,
                        FCustomerId = vo.FCustomerId,
                        FPrice = vo.FPrice,
                        FQuantity = vo.FQuantity,
                        FDepartureDate = vo.FDepartureDate,
                        FForPickupOrDeliveryAddress = vo.FForPickupOrDeliveryAddress,
                        FInterviewReminder = vo.FInterviewReminder,
                        FEvaluate = vo.FEvaluate,
                        FMemo = vo.FMemo,
                        FStatusId = vo.FStatusId,
                        FCouponId = vo.FCouponId,
                        TVtravelerInfos = vo.TVtravelerInfos
                    };
                    _context.Add(tVorder);
                    await _context.SaveChangesAsync();


                    return Json(new { success = true, message = "新增訂單成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"訂單新增失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }

        public JsonResult VProductById(int Id)
        {
            var product = _context.VVproductViews.FirstOrDefault(t => t.商品編號 == Id);
            return Json(product);
        }

        public JsonResult VOrderById()
        {
            var order = _context.VVorderViews.OrderByDescending(e => e.FId).FirstOrDefault();
            return Json(order);
        }

        public JsonResult VProductFormsById(int Id)
        {
            var Forms = new List<TVformPath>();
            var formIds = _context.TVproductFormsRequireds.Where(t => t.FProductId == Id).Select(d => d.FFormId).ToList();
            foreach(var formId in formIds)
            {
                var f = _context.TVformPaths.FirstOrDefault(t => t.FId == formId);
                if (f != null)
                {
                    Forms.Add(f);
                }
            }

            return Json(Forms);
        }

        public JsonResult VVProductEnabled()
        {
            var VVProductEnabled = _context.VVproductViews.Where(e => e.啟用狀態 == true);
            return Json(VVProductEnabled);
        }

        public IActionResult countries()
        {
            var countries = _context.TVcountries.ToList();
            if (countries != null)
            {
                return Json(countries);
            }
            return Json(null);
        }

        public class CountryUrlData
        {
            public string Country { get; set; }
            public string Url { get; set; }
        }
        static string _countryName;

        public async Task<JsonResult> Embassies()
        {
            List<CountryUrlData> countryUrlList = new List<CountryUrlData>();

            string targetUrl = "https://www.mofa.gov.tw/OfficesInROC.aspx?n=169&sms=86";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(targetUrl);
                    response.EnsureSuccessStatusCode(); // 確認請求成功

                    // 讀取網頁內容
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // 找所有<a>
                    var linkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

                    foreach (var linkNode in linkNodes)
                    {
                        // 找包含 <span class="flagbox"> 的子元素
                        var spanNode = linkNode.SelectSingleNode(".//span[@class='flagbox']");

                        // 如果找到匹配条件的 <span> 元素
                        if (spanNode != null)
                        {
                            // 抓出裡面的href
                            string hrefValue = linkNode.Attributes["href"].Value;

                            // 如果是相對路徑，就補上前面那段組成完整網址
                            Uri uriResult;
                            if (Uri.TryCreate(hrefValue, UriKind.Relative, out uriResult))
                            {
                                Uri baseUri = new Uri(targetUrl);
                                Uri absoluteUri = new Uri(baseUri, uriResult);

                                var countryNode = linkNode.SelectSingleNode(".//span[@class='countryname']");
                                if (countryNode != null)
                                {
                                    string countryName = countryNode.InnerText.Trim();

                                    _countryName = countryName;
                                }


                                //再進入該連結
                                using (HttpClient newClient = new HttpClient())
                                {
                                    try
                                    {
                                        HttpResponseMessage newResponse = await newClient.GetAsync(absoluteUri);
                                        newResponse.EnsureSuccessStatusCode();

                                        string newHtmlContent = await newResponse.Content.ReadAsStringAsync();

                                        HtmlDocument newHtmlDocument = new HtmlDocument();
                                        newHtmlDocument.LoadHtml(newHtmlContent);

                                        // 抓出 '網址：'
                                        var targetNode = newHtmlDocument.DocumentNode.SelectSingleNode("//li[contains(span[@class='introtitle01'], '網址：')]");

                                        if (targetNode != null)
                                        {
                                            string text = targetNode.InnerText;
                                            string url = text.Replace("網址：", "").Trim();
                                            countryUrlList.Add(new CountryUrlData { Country = _countryName, Url = url });
                                        }
                                    }
                                    catch (HttpRequestException e)
                                    {
                                        Console.WriteLine($"HTTP 错误: {e.Message}");
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine(hrefValue);
                            }
                        }
                    }

                    foreach (var c in countryUrlList)
                    {
                        Console.WriteLine(c.Country + " : " + c.Url);
                    }


                    return Json(countryUrlList);


                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"HTTP 錯誤: {e.Message}");
                }
            }
            return null;
        }
    }
}
