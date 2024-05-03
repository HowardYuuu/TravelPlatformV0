using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using prjTravelPlatformV3.Models;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using static prjTravelPlatform_release.Areas.Customer.Controllers.CarRent.RApiController;
using static System.Net.WebRequestMethods;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.CarRent
{
    [Route("api/[controller]")]
    [ApiController]
    public class RApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        public RApiController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //讀取租車據點
        [HttpGet]
        [Route("GetServicePoints")]
        public IActionResult GetServicePoints(string keyword)
        {
            // 從資料庫中查詢包含指定關鍵字的服務點
            var servicePoints = _context.TRservicePoints
                .Where(a => a.FServicePoint.Contains(keyword))
                .Select(a => new { Location = a.FServicePoint }) // 將結果投影到新的匿名類型中
                //.Take(5) // 僅返回前5個結果
                .ToList();

            // 返回 JSON 格式的數據
            return Ok(servicePoints);
        }

        //讀取所有車輛資料
        [HttpGet]
        [Route("GetCar")]
        public async Task<IActionResult> GetCar()
        {
            var carList = await _context.RCarInfoViews
                .Select(car => new
                {
                    car.FCarId,
                    car.FServicePoint,
                    car.FModelName,
                    car.FNumOfPsgr,
                    car.FNumOfLuggage,
                    car.FDescription,
                    car.FDescription2,
                    car.FRentalFee,
                    car.FImagePath
                })
                .ToListAsync();

            // 返回 JSON 格式的数据
            return Json(carList);
        }

        [HttpGet]
        [Route("GetRandomCars")]
        public async Task<IActionResult> GetRandomCars()
        {
            var carList = await _context.RCarInfoViews
                .OrderBy(x => Guid.NewGuid()) // 在資料庫層面對資料進行隨機排序
                .Take(4) // 選取五筆資料
                .Select(car => new
                {
                    car.FCarId,
                    car.FServicePoint,
                    car.FModelName,
                    car.FNumOfPsgr,
                    car.FNumOfLuggage,
                    car.FDescription,
                    car.FDescription2,
                    car.FRentalFee,
                    car.FImagePath
                })
                .ToListAsync();

            // 返回 JSON 格式的數據
            return Json(carList);
        }

        public class paramsCar
        {
            public string? pickup { get; set; }
            public int startIndex { get; set; }
            public int dataPerPage { get; set; }
            public string? sortOrder { get; set; }
            public string? pickupDateTime { get; set; }
            public string? returnDateTime { get; set; }
            public int minPrice { get; set; }
            public int maxPrice { get; set; }
            public List<string>? selectedCompanies { get; set; } = new List<string>();
            public List<string>? selectedCarTypes { get; set; } = new List<string>();
            public List<string>? selectedFuelTypes { get; set; } = new List<string>();
            public List<string>? selectedSeats { get; set; } = new List<string>();
        }
        
        [HttpPost]
        [Route("GetCarList")]
        public async Task<IActionResult> CarLists([FromBody] paramsCar paramsCar)
        {
            // 根據 pickup 參數值查詢相應的車輛資訊，並根據分頁參數進行分頁
            var query = _context.RCarInfoViews
                .Where(car => car.FServicePoint.Contains(paramsCar.pickup))
                .Where(car => car.FRentalFee >= paramsCar.minPrice && car.FRentalFee <= paramsCar.maxPrice); // 新增價格範圍篩選條件

            // 如果有勾選的供應商，則進行篩選
            if (paramsCar.selectedCompanies != null && paramsCar.selectedCompanies.Any())
            {
                query = query.Where(car => paramsCar.selectedCompanies.Contains(car.FCompanyName));
            }

            if (paramsCar.selectedCarTypes != null && paramsCar.selectedCarTypes.Any())
            {
                query = query.Where(car => paramsCar.selectedCarTypes.Contains(car.FDescription));
            }

            if (paramsCar.selectedFuelTypes != null && paramsCar.selectedFuelTypes.Any())
            {
                query = query.Where(car => paramsCar.selectedFuelTypes.Contains(car.FDescription2));
            }

            if (paramsCar.selectedSeats != null && paramsCar.selectedSeats.Any())
            {
                bool seat6Selected = paramsCar.selectedSeats.Contains("6");
                bool seat7Selected = paramsCar.selectedSeats.Contains("7");

                if (seat6Selected && !seat7Selected)
                {
                    query = query.Where(car => car.FNumOfPsgr <= 6);
                }
                else if (!seat6Selected && seat7Selected)
                {
                    query = query.Where(car => car.FNumOfPsgr >= 7);
                }
            }

            if (paramsCar.sortOrder == "asc")
            {
                query = query.OrderBy(car => car.FRentalFee);
            }
            else if (paramsCar.sortOrder == "desc")
            {
                query = query.OrderByDescending(car => car.FRentalFee);
            }

            var dtR = Convert.ToDateTime(paramsCar.returnDateTime);
            var dtP = Convert.ToDateTime(paramsCar.pickupDateTime);

            var rentedCarIds = await _context.TRcarRentOrderDetails
                .Where(order => order.FFromDateTime <= dtR && order.FRentDateTime >= dtP)
                .Select(order => order.FCarId)
                .Distinct()
                .ToListAsync();

            // 排除已被租走的車輛
            query = query.Where(car => !rentedCarIds.Contains(car.FCarId));

            var carList = await query
                .Select(car => new
                {
                    car.FCarId,
                    car.FServicePoint,
                    car.FModelName,
                    car.FNumOfPsgr,
                    car.FNumOfLuggage,
                    car.FDescription,
                    car.FDescription2,
                    car.FRentalFee,
                    car.FImagePath,
                    car.FCompanyName
                })
                .Skip(paramsCar.startIndex)
                .Take(paramsCar.dataPerPage)
                .ToListAsync();

            // 獲取符合條件的所有車輛的總數量
            var totalCount = await query
                .CountAsync();

            return Json(new { carList, totalCount }); // 返回 JSON 格式的車輛資訊和總數量
        }

        [HttpGet]
        [Route("GetCarDetails")]
        public IActionResult GetCarDetails(string carId)
        {
            // 根據車輛ID查詢單一車輛資料
            var carDetails = _context.RCarInfoViews
                .Where(car => car.FCarId.ToString() == carId)
                .Select(car => new
                {
                    car.FCarId,
                    car.FModelName,
                    car.FNumOfPsgr,
                    car.FNumOfLuggage,
                    car.FDescription,
                    car.FDescription2,
                    car.FRentalFee,
                    car.FImagePath,
                    car.FServicePoint
                })
                .FirstOrDefault(); // 只返回一條資料

            if (carDetails == null)
            {
                // 如果找不到對應的車輛，返回404 Not Found
                return NotFound();
            }

            // 返回 JSON 格式的數據
            return Ok(carDetails);
        }

        [HttpGet("getLocationId")]
        public IActionResult GetLocationId(string locationName)
        {
            var location = _context.TRservicePoints.FirstOrDefault(s => s.FServicePoint == locationName);

            if (location == null)
            {
                return NotFound($"地點 '{locationName}' 不存在！");
            }

            return Ok(location.FServicePointId);
        }

        public class requestData
        {
            public int customerId { get; set; }
            public int carId { get; set; }
            public int dropoffLocationId { get; set; }
            public string driverName { get; set; }
            public DateTime fromDateTime { get; set; }
            public DateTime rentDateTime { get; set; }
            public string driverId { get; set; }
            public int driverAge { get; set; }
            public string phoneNumber { get; set; }
            public decimal totalFee { get; set; }
        }

        [HttpPost]
        [Route("getFrontData")]
        public IActionResult getFrontData(requestData rd)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 建立駕駛資料
                    var driverInfo = new TRdriverInfo
                    {
                        FName = rd.driverName,
                        FId = rd.driverId,
                        FAge = rd.driverAge,
                        FPhone = rd.phoneNumber
                    };

                    // 將駕駛資料加入 DbContext 中
                    _context.TRdriverInfos.Add(driverInfo);
                    _context.SaveChanges(); // 可能會報錯

                    // 建立新的訂單
                    var order = new TRcarRentOrder
                    {
                        FMemberId = rd.customerId,
                        FPaymentId = 1,
                        FPaymentStatusId = 2,
                        FOrderStatusId = 2,
                        FOrderDate = DateTime.Now.ToString(),
                    };

                    // 將新的訂單加入 DbContext 中
                    _context.TRcarRentOrders.Add(order);
                    _context.SaveChanges(); // 可能會報錯

                    var lastDriverId = _context.TRdriverInfos
                        .OrderByDescending(d => d.FDriverId)
                        .Select(d => d.FDriverId)
                        .FirstOrDefault();

                    // 建立訂單明細
                    var orderDetail = new TRcarRentOrderDetail
                    {
                        // 使用新建訂單的流水號作為關聯
                        FOrderId = order.FOrderId,
                        // 填入其他相關資料
                        FCarId = rd.carId,
                        FDropLocId = rd.dropoffLocationId,
                        FFromDateTime = rd.fromDateTime.ToLocalTime(),
                        FRentDateTime = rd.rentDateTime.ToLocalTime(),
                        FDriverId = lastDriverId,
                        FPrice = rd.totalFee,
                    };

                    // 將訂單明細加入 DbContext 中
                    _context.TRcarRentOrderDetails.Add(orderDetail);
                    _context.SaveChanges(); // 可能會報錯

                    transaction.Commit(); // 提交事務

                    var latestOrderId = _context.TRcarRentOrders
                    .OrderByDescending(o => o.FOrderId)
                    .Select(o => o.FOrderId).FirstOrDefault();

                    var amount = _context.RCarRentOrderInfoViews
                        .OrderByDescending(o => o.FOrderId)
                        .Select(o => Convert.ToInt32(o.FTotalPrice)).FirstOrDefault();
                    var TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    var checkstr = "HashKey=pwFHCqoQZGmho4w6&ChoosePayment=Credit&EncryptType=1&ItemName=Item&MerchantID=3002607&MerchantTradeDate=" + TradeDate + "&MerchantTradeNo=" + latestOrderId 
                    + "&OrderResultURL=https://localhost:7119/Customer/CarRent/CarBookingConfirm"
                    + "&PaymentType=aio&ReturnURL=https://localhost:7119/Customer/CarRent/CarBookingConfirm" + "&TotalAmount=" + amount
                    + "&TradeDesc=test&HashIV=EkRm7iFT261dpevs";

                    var encodedCheckstr = HttpUtility.UrlEncode(checkstr).ToLower();
                    string hashedString;
                    // 將字串轉換為位元組陣列
                    byte[] bytes = Encoding.UTF8.GetBytes(encodedCheckstr);

                    // 建立 SHA256 實例
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        // 計算雜湊值
                        byte[] hash = sha256.ComputeHash(bytes);

                        // 將位元組陣列轉換為十六進位字串
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (byte b in hash)
                        {
                            stringBuilder.Append(b.ToString("x2"));
                        }
                        hashedString = stringBuilder.ToString().ToUpper();
                    }

                    // 構建支付參數
                    var paymentParams = new
                    {
                        // 根據綠界的API文件設置參數
                        MerchantID = "3002607",
                        MerchantTradeNo = latestOrderId,
                        MerchantTradeDate = TradeDate,
                        PaymentType = "aio",
                        TotalAmount = amount,
                        TradeDesc = "test",
                        ItemName = "Item",
                        ReturnURL = "https://localhost:7119/Customer/CarRent/CarBookingConfirm",
                        OrderResultURL = "https://localhost:7119/Customer/CarRent/CarBookingConfirm",
                        ChoosePayment = "Credit",
                        EncryptType = 1,
                        CheckMacValue = hashedString
                    };

                    return Ok(paymentParams); // 如果一切順利，回傳成功訊息
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // 發生異常時回滾事務
                    return StatusCode(500, "An error occurred while processing your request."); // 回傳錯誤訊息
                }
            }
        }

        [HttpGet]
        [Route("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment()
        {
            try
            {
                var latestOrderId = _context.TRcarRentOrders
                    .OrderByDescending(o => o.FOrderId)
                    .Select(o => o.FOrderId).FirstOrDefault();

                var amount = _context.RCarRentOrderInfoViews
                    .OrderByDescending(o => o.FOrderId)
                    .Select(o => Convert.ToInt32(o.FTotalPrice)).FirstOrDefault();
                var TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                var checkstr = "HashKey=pwFHCqoQZGmho4w6&ChoosePayment=Credit&EncryptType=1&ItemName=Item&MerchantID=3002607&MerchantTradeDate=" + TradeDate + "&MerchantTradeNo=" + latestOrderId
                    + "&OrderResultURL=https://67c6-118-166-204-4.ngrok-free.app/Customer/CarRent/CarBookingConfirm" +
                    "&PaymentType=aio&ReturnURL=https://67c6-118-166-204-4.ngrok-free.app/Customer/CarRent/CarBookingConfirm" + "&TotalAmount=" + amount
                    + "&TradeDesc=test&HashIV=EkRm7iFT261dpevs";

                var encodedCheckstr = HttpUtility.UrlEncode(checkstr).ToLower();
                string hashedString;
                // 將字串轉換為位元組陣列
                byte[] bytes = Encoding.UTF8.GetBytes(encodedCheckstr);

                // 建立 SHA256 實例
                using (SHA256 sha256 = SHA256.Create())
                {
                    // 計算雜湊值
                    byte[] hash = sha256.ComputeHash(bytes);

                    // 將位元組陣列轉換為十六進位字串
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        stringBuilder.Append(b.ToString("x2"));
                    }
                    hashedString = stringBuilder.ToString().ToUpper();

                    Console.WriteLine(hashedString);
                }

                // 構建支付參數
                var paymentParams = new
                {
                    // 根據綠界的API文件設置參數
                    MerchantID = "3002607",
                    MerchantTradeNo = latestOrderId,
                    MerchantTradeDate = TradeDate,
                    PaymentType = "aio",
                    TotalAmount = amount,
                    TradeDesc = "test",
                    ItemName = "Item",
                    ReturnURL = "https://67c6-118-166-204-4.ngrok-free.app/Customer/CarRent/CarBookingConfirm",
                    OrderResultURL = "https://67c6-118-166-204-4.ngrok-free.app/Customer/CarRent/CarBookingConfirm",
                    ChoosePayment = "Credit",
                    EncryptType = "1",
                    CheckMacValue = hashedString
                };

                // 建立 HttpClient 實例
                using (HttpClient client = new HttpClient())
                {
                    // 像綠界發送支付請求
                    HttpResponseMessage response = await client.PostAsJsonAsync("https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5", paymentParams);

                    // 確保請求成功
                    response.EnsureSuccessStatusCode();

                    // 請響應中獲取支付頁面URL
                    string paymentPageUrl = await response.Content.ReadAsStringAsync();

                    // 將支付頁面URL傳回前端
                    return Json(new { paymentPageUrl });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "支付請求失敗：" + ex.Message });
            }
        }

        [HttpGet]
        [Route("getOrderNDetails")]
        public async Task<IActionResult> getOrderNDetails()
        {
            try
            {
                var latestOrder = await _context.RCarRentOrderInfoViews
                    .OrderByDescending(o => o.FOrderId)
                    .Select(o => new
                    {
                        OrderId = o.FOrderId,
                        MemberName = o.FName,
                        MemberPhone = o.FPhone,
                        MemberEmail = o.FEmail,
                        Payment = o.FPayment,
                        PaymentStatus = o.FPaymentStatus,
                        OrderStatus = o.FOrderStatus,
                        OrderDate = o.FOrderDate,
                        TotalPrice = o.FTotalPrice
                    })
                    .FirstOrDefaultAsync();

                var latestOrderDetail = await _context.RCarRentOrderDetailViews
                    .OrderByDescending(o => o.FOrderDetailId)
                    .Select(o => new
                    {
                        OrderId = o.FOrderId,
                        DriverName = o.FName,
                        DriverId = o.FId,
                        PhoneNumber = o.FPhone,
                        CarModel = o.FModelName,
                        PickupDateTime = o.FFromDateTime,
                        DropoffDateTime = o.FRentDateTime,
                        PickupLocation = o.FServicePoint,
                        DropoffLocation = o.FRentServicePoint,
                    })
                    .FirstOrDefaultAsync();

                if (latestOrder == null || latestOrderDetail == null)
                {
                    return NotFound(); // 如果沒有找到相應的數據，返回404錯誤
                }

                var mergedOrderData = new OrderData
                {
                    OrderId = latestOrder.OrderId,
                    MemberName = latestOrder.MemberName,
                    TotalPrice = latestOrder.TotalPrice?.ToString("N0"),
                    OrderDate = latestOrder.OrderDate,
                    DriverName = latestOrderDetail.DriverName,
                    CarModel = latestOrderDetail.CarModel,
                    PickupDateTime = latestOrderDetail.PickupDateTime,
                    DropoffDateTime = latestOrderDetail.DropoffDateTime,
                    PickupLocation = latestOrderDetail.PickupLocation,
                    DropoffLocation = latestOrderDetail.DropoffLocation
                };

                await ProcessAndSendToBot(mergedOrderData);


                return Ok(new { latestOrder, latestOrderDetail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request."); // 回傳錯誤訊息
            }
        }


        public class OrderData
        {
            public string OrderId { get; set; }
            public string MemberName { get; set; }
            public string TotalPrice { get; set; }
            public string OrderDate { get; set; }
            public string DriverName { get; set; }
            public string CarModel { get; set; }
            public DateTime? PickupDateTime { get; set; }
            public DateTime? DropoffDateTime { get; set; }
            public string PickupLocation { get; set; }
            public string DropoffLocation { get; set; }
        }
        private async Task ProcessAndSendToBot(OrderData orderData)
        {
            // 組合訂單資料並發送到 Line Bot
            var message = $"訂單 ID: {orderData.OrderId}\n" +
                          $"會員姓名: {orderData.MemberName}\n" +
                          $"訂單日期: {orderData.OrderDate}\n";

            if (!string.IsNullOrEmpty(orderData.DriverName))
            {message += $"駕駛姓名: {orderData.DriverName}\n";}

            if (!string.IsNullOrEmpty(orderData.CarModel))
            {message += $"預訂車型: {orderData.CarModel}\n";}

            if (!string.IsNullOrEmpty(orderData.PickupLocation) && orderData.PickupDateTime != null)
            {message += $"取車: {orderData.PickupLocation} " + $"{orderData.PickupDateTime}\n";}

            if (!string.IsNullOrEmpty(orderData.DropoffLocation) && orderData.DropoffDateTime != null)
            {message += $"還車: {orderData.DropoffLocation} " + $"{orderData.DropoffDateTime}\n";}

            if (orderData.TotalPrice != null)
            {message += $"訂單金額:NT${orderData.TotalPrice}\n";}

            await SendLineBotMessage(message);
        }
        private async Task SendLineBotMessage(string message)
        {
            // Line Bot 的 Channel Access Token
            string lineAccessToken = "MnJTMBz5RUJopMqYXQUnwbdg/rNGWKsXwlFWb9VXo2w38tIWwskzV4kijsFt40nNDijMWandv9fIzm4FTpjGHrcnascFdUaK75Mn1VJknPjzCn+" +
                "iFXenSUAksByyPT50rO8pj2XjFzI4ZIAPa9bnBgdB04t89/1O/w1cDnyilFU=";

            // 構建訂單消息
            var payload = new
            {
                to = "U146a066324d731c2b065a847888419c8", // Line Bot 的 ID
                messages = new[]
                {
            new
            {
                type = "text",
                text = message
            }
        }
            };

            // 發送 HTTP POST 請求到 Line Bot API
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + lineAccessToken);
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);

                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        [HttpGet]
        [Route("getPointAddress")]
        public IActionResult getPointAddress(string pickupLocation)
        {
            var location = _context.TRservicePoints.FirstOrDefault(s => s.FServicePoint == pickupLocation);

            if (location == null)
            {
                return NotFound($"地點 '{pickupLocation}' 不存在！");
            }

            return Ok(location.FAddress);
        }

        public class ServicePointData
        {
            public string FServicePoint { get; set; }
            public string FAddress { get; set; }
        }
        [HttpGet]
        [Route("getAllCarLocation")]
        public IActionResult getAllCarLocation()
        {
            // 從資料庫中獲取所有租車據點的地址
            var allLocations = _context.TRservicePoints
                .Select(point => new
                {
                    point.FServicePoint,
                    point.FAddress
                })
                .ToList(); // 將資料轉換為列表
                           // 檢查是否有找到租車據點地址
            if (allLocations == null || allLocations.Count == 0)
            {
                return NotFound("找不到任何租車據點地址。");
            }

            return Ok(allLocations);
        }

        public class Coordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        [HttpPost]
        [Route("getNearbyLocations")]
        public async Task<IActionResult> getNearbyLocations([FromBody] Coordinates coordinates)
        {
            double latitude = coordinates.Latitude;
            double longitude = coordinates.Longitude;

            // 從資料庫中獲取所有地址
            List<string> allAddresses = await _context.TRservicePoints.Select(sp => sp.FAddress).ToListAsync();

            // 對所有地址執行地理編碼
            var tasks = allAddresses.Select(async address =>
            {
                return await GetCoordinatesFromAddress(address);
            });

            // 等待所有任務完成
            var coordinatesList = await Task.WhenAll(tasks);

            // 計算每個地址的距離並篩選出1公里範圍內的據點
            var nearbyServicePoints = new List<object>();
            for (int i = 0; i < allAddresses.Count; i++)
            {
                var (lat, lng) = coordinatesList[i];
                double distance = CalculateDistance(latitude, longitude, lat, lng);
                if (distance <= 5.0)
                {
                    // 添加相符的據點到列表中
                    nearbyServicePoints.Add(new
                    {
                        Address = allAddresses[i],
                        Distance = distance,
                        PointName = await _context.TRservicePoints.Where(sp => sp.FAddress == allAddresses[i]).Select(sp => sp.FServicePoint).FirstOrDefaultAsync()
                    });
                }
            }

            return Ok(nearbyServicePoints);
        }
        private async Task<(double, double)> GetCoordinatesFromAddress(string address)
        {
            // 使用地理編碼服務（例如 Google 地理編碼 API）來獲取地址的經緯度
            // 這個方法應該發送一個 HTTP 請求到地理編碼 API，並解析回傳的 JSON 數據以獲取經緯度
            // 這裡是一個簡單的示例，您需要根據您選擇的地理編碼服務來編寫適當的代碼
            // 以下是一個使用 Google 地理編碼 API 的示例代碼

            string apiKey = "AIzaSyALKcN-DCxYRsBl0P5hqyiQUvVu3HL_nak";
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var location = result["results"][0]["geometry"]["location"];
                    double lat = (double)location["lat"];
                    double lng = (double)location["lng"];
                    return (lat, lng);
                }
                else
                {
                    throw new Exception("Failed to get coordinates from address.");
                }
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // 地球半徑，單位為公里

            // 將角度轉換為弧度
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            // 使用 Haversine 公式計算距離
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
