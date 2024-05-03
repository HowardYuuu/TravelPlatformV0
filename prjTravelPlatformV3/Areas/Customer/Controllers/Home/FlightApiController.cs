using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using prjTravelPlatform_release.Areas.Customer.ViewModel.Airline;
using prjTravelPlatform_release.Models;
using prjTravelPlatformV3.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.Json;
using static NuGet.Packaging.PackagingConstants;
using System.Security.Claims;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Azure.Core;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using prjTravelPlatformV3.Areas.Employee.Controllers.Airline;
using Org.BouncyCastle.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text;
using System.Web;
using System.Security.Cryptography;


namespace prjTravelPlatform_release.Areas.Customer.Controllers.Home
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightApiController : ControllerBase
    {

        private readonly dbTravalPlatformContext _context;
        private ICompositeViewEngine _viewEngine;
        public FlightApiController(dbTravalPlatformContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }


        //路徑：~/api/flightapi/airports
        //調出機場選項
        [HttpGet]
        [Route("Airports")]
        public IActionResult Airports()
        {
            var airports = _context.TFairportInfos.Select(a => a.FAirport).Distinct();
            return new JsonResult(airports);
        }

        //路徑：~/api/flightapi/GetFlights
        //調出單程航班資訊
        [HttpGet]
        [Route("GetFlights")]
        public async Task<ActionResult<IEnumerable<FlightSearchDTO>>> GetFlights()
        {
            var flights = await _context.TFflightSchedules
                .Select(f => new FlightSearchDTO
                {
                    fId = Convert.ToInt32(f.FScheduleId),
                    AirlineId = Convert.ToInt32(f.FAirlineId),
                    Departure = f.FDeparture.FAirport,
                    Destination = f.FDestination.FAirport,
                    DepartureTime = Convert.ToDateTime(f.FDepartureTime),
                    ArrivalTime = Convert.ToDateTime(f.FArrivalTime),
                    // 計算飛行時間
                    Duration = f.FArrivalTime != null && f.FDepartureTime != null ? (int)(f.FArrivalTime - f.FDepartureTime).Value.TotalMinutes : 0,
                    TicketPrice = f.FTicketPrice
                })
                .ToListAsync();

            return new JsonResult(flights);
        }

        //顯示搜尋結果
        [HttpGet]
        [Route("FlightsSearch")]
        public async Task<ActionResult> FlightsSearch(string departure, string destination, string departureDate, string returnDate, string Adult, string Children, string Baby)
        {
            try
            {
                DateTime date1 = Convert.ToDateTime(departureDate).Date;
                DateTime date2 = Convert.ToDateTime(returnDate).Date;
                var TicketQty = Convert.ToInt32(Adult) + Convert.ToInt32(Children);
                var outgoingFlights = await _context.TFflightSchedules
                    .Where(f => f.FDeparture.FAirport == departure &&
                                f.FDestination.FAirport == destination &&
                                f.FDepartureTime.Value.Date == date1 &&
                                f.FQty.Value >= TicketQty)
                    .Select(f => new
                    {
                        f.FScheduleId, //航班Id
                        f.FAirlineId,  //航空公司Id
                        f.FAirline.FCompanyName, //航空公司
                        f.FFlightName,　//航班代碼
                        DepartureTime = f.FDepartureTime.Value.ToString("yyyy/MM/dd tt HH:mm"), //起飛時間
                        DepartureAirport = f.FDeparture.FAirport,  //起飛地
                        ArrivalTime = f.FArrivalTime.Value.ToString("yyyy/MM/dd tt HH:mm"),  //降落時間
                        DestinationAirport = f.FDestination.FAirport, //降落地
                        f.FTicketPrice, //票價
                        f.FClass.FClass,  //艙等
                        f.FQty, //數量
                        //飛行時間
                        Duration = f.FArrivalTime != null && f.FDepartureTime != null ? (int)(f.FArrivalTime - f.FDepartureTime).Value.TotalMinutes : 0,
                    })
                    .ToListAsync();

                var returnFlights = await _context.TFflightSchedules
                    .Where(f => f.FDeparture.FAirport == destination &&
                                f.FDestination.FAirport == departure &&
                                f.FDepartureTime.Value.Date == date2 &&
                                f.FDepartureTime.Value.Date > date1 &&
                                f.FQty.Value >= TicketQty)
                    .Select(f => new
                    {
                        f.FScheduleId, //航班Id
                        f.FAirlineId,  //航空公司Id
                        f.FAirline.FCompanyName, //航空公司
                        f.FFlightName,　//航班代碼
                        DepartureTime = f.FDepartureTime.Value.ToString("yyyy/MM/dd tt HH:mm"), //起飛時間
                        DepartureAirport = f.FDeparture.FAirport,  //起飛地
                        ArrivalTime = f.FArrivalTime.Value.ToString("yyyy/MM/dd tt HH:mm"),  //降落時間
                        DestinationAirport = f.FDestination.FAirport, //降落地
                        f.FTicketPrice, //票價
                        f.FClass.FClass,  //艙等
                        f.FQty, //數量
                        //飛行時間
                        Duration = f.FArrivalTime != null && f.FDepartureTime != null ? (int)(f.FArrivalTime - f.FDepartureTime).Value.TotalMinutes : 0,

                    })
                    .ToListAsync();

                var result = new
                {
                    OutgoingFlights = outgoingFlights,
                    ReturnFlights = returnFlights
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //訂單頁面秀出航班資訊
        [HttpGet]
        [Route("SelectedFlights")]
        public async Task<ActionResult> SelectedFlights(string outgoingScheduleId, string returnScheduleId)
        {
            try
            {
                var outgoingFlight = _context.TFflightSchedules
                    .Where(f => f.FScheduleId == Convert.ToInt32(outgoingScheduleId))
                    .Select(f => new
                    {
                        f.FScheduleId, //航班Id
                        f.FAirlineId,  //航空公司Id
                        f.FAirline.FCompanyName, //航空公司
                        f.FFlightName,　//航班代碼
                        DepartureTime = f.FDepartureTime.Value.ToString("yyyy/MM/dd tt HH:mm"), //起飛時間
                        DepartureAirport = f.FDeparture.FAirport,  //起飛地
                        ArrivalTime = f.FArrivalTime.Value.ToString("yyyy/MM/dd tt HH:mm"),  //降落時間
                        DestinationAirport = f.FDestination.FAirport, //降落地
                        f.FTicketPrice, //票價
                        f.FClass.FClass,  //艙等
                        f.FQty, //數量
                        //飛行時間
                        Duration = f.FArrivalTime != null && f.FDepartureTime != null ? (int)(f.FArrivalTime - f.FDepartureTime).Value.TotalMinutes : 0,
                    })
                    .ToList();

                var returnFlight = _context.TFflightSchedules
                    .Where(f => f.FScheduleId == Convert.ToInt32(returnScheduleId))
                    .Select(f => new
                    {
                        f.FScheduleId, //航班Id
                        f.FAirlineId,  //航空公司Id
                        f.FAirline.FCompanyName, //航空公司
                        f.FFlightName,　//航班代碼
                        DepartureTime = f.FDepartureTime.Value.ToString("yyyy/MM/dd tt HH:mm"), //起飛時間
                        DepartureAirport = f.FDeparture.FAirport,  //起飛地
                        ArrivalTime = f.FArrivalTime.Value.ToString("yyyy/MM/dd tt HH:mm"),  //降落時間
                        DestinationAirport = f.FDestination.FAirport, //降落地
                        f.FTicketPrice, //票價
                        f.FClass.FClass,  //艙等
                        f.FQty, //數量
                        //飛行時間
                        Duration = f.FArrivalTime != null && f.FDepartureTime != null ? (int)(f.FArrivalTime - f.FDepartureTime).Value.TotalMinutes : 0,
                    })
                    .ToList();

                var result = new
                {
                    OutgoingFlights = outgoingFlight,
                    ReturnFlights = returnFlight
                };
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }



        //路徑：~/api/flightapi/RoundTripOrder
        //成立訂單
        [HttpPost]
        [Route("RoundTripOrder")]
        public async Task<ActionResult> RoundTripOrder([FromBody] OrderViewModel model)
        {
            //使用transaction
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region 新增訂單
                    // 新建訂單
                    var order = new TForder
                    {
                        FMemberId = model.CustomerId,
                        FOrderDate = DateTime.Now.ToLocalTime(),
                        FOrderStatusId = 2, //已付款
                        FPaymentStatusId = 2, //已付款
                        FPaymentId = model.PaymentId, //信用卡
                        FTotal = 0  //總額
                    };

                    _context.TForders.Add(order);
                    await _context.SaveChangesAsync();

                    // 從 order 物件中取得訂單編號
                    int orderId = order.FId;
                    #endregion

                    decimal totalAmount = 0; // 訂單總額 


                    // 建立訂單細節
                    foreach (var passenger in model.Passengers)
                    {
                        #region 處理前端資料
                        int ticketTypeId;
                        if (passenger.Type == "adult")
                        {
                            ticketTypeId = 1;
                        }
                        else if (passenger.Type == "baby")
                        {
                            ticketTypeId = 2;
                        }
                        else if (passenger.Type == "children")
                        {
                            ticketTypeId = 3;
                        }
                        else
                        {
                            ticketTypeId = 1;
                        }

                        // 取得ticketRate
                        var ticketType = await _context.TFticketTypes.FindAsync(ticketTypeId);
                        if (ticketType == null)
                        {
                            continue;
                        }

                        // 去程票價
                        var outgoingFlightSchedule = await _context.TFflightSchedules.FindAsync(model.OutgoingScheduleId);
                        if (outgoingFlightSchedule == null)
                        {
                            continue;
                        }
                        // 回程票價
                        var returnFlightSchedule = await _context.TFflightSchedules.FindAsync(model.ReturnScheduleId);
                        if (returnFlightSchedule == null)
                        {
                            continue;
                        }

                        // 計算票價
                        decimal outgoingPrice = (decimal)ticketType.FTicketRate * (decimal)outgoingFlightSchedule.FTicketPrice;
                        decimal returnPrice = (decimal)ticketType.FTicketRate * (decimal)returnFlightSchedule.FTicketPrice;

                        #endregion

                        #region 新增來回訂單細節
                        // 新建去程訂單細節
                        var outgoingOrderDetail = new TForderDetail
                        {
                            FOrderId = order.FOrderId,
                            FScheduleId = model.OutgoingScheduleId,
                            FTicketTypeId = ticketTypeId,
                            FPrice = outgoingPrice,
                            FPsgrName = passenger.LastName + passenger.FirstName,
                            FNationalId = passenger.NationalId,
                            FGender = passenger.Gender,
                            FBirth = passenger.Birth,
                            FEmail = passenger.Email,
                            FPhone = passenger.Phone,
                        };
                        // 新建回程訂單細節
                        var returnOrderDetail = new TForderDetail
                        {
                            FOrderId = order.FOrderId,
                            FScheduleId = model.ReturnScheduleId,
                            FTicketTypeId = ticketTypeId,
                            FPrice = returnPrice,
                            FPsgrName = passenger.LastName + passenger.FirstName,
                            FNationalId = passenger.NationalId,
                            FGender = passenger.Gender,
                            FBirth = passenger.Birth,
                            FEmail = passenger.Email,
                            FPhone = passenger.Phone,
                        };

                        _context.TForderDetails.Add(outgoingOrderDetail);
                        _context.TForderDetails.Add(returnOrderDetail);
                        #endregion

                        // 更新訂單總金額
                        totalAmount += outgoingPrice + returnPrice;
                    }

                    // 總金額存回資料庫
                    order.FTotal = totalAmount;
                    await _context.SaveChangesAsync();

                    // 訂單成立後扣除航班票數的庫存
                    int passengerCount = model.Passengers.Count;

                    // 去程回程的航班ID
                    var outgoingFlightScheduleId = await _context.TFflightSchedules.FindAsync(model.OutgoingScheduleId);
                    var returnFlightScheduleId = await _context.TFflightSchedules.FindAsync(model.ReturnScheduleId);

                    // 航班的票數扣除乘客數量
                    if (outgoingFlightScheduleId != null && returnFlightScheduleId != null)
                    {
                        outgoingFlightScheduleId.FQty -= passengerCount;
                        returnFlightScheduleId.FQty -= passengerCount;

                        await _context.SaveChangesAsync();
                    }

                    // 提交Commit
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    #region 金流
                    var latestOrderId = _context.TForders.OrderByDescending(o => o.FOrderId)
                        .Select(o => o.FOrderId).FirstOrDefault();

                    var amount = Convert.ToInt32(totalAmount);

                    var TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    var checkstr = "HashKey=pwFHCqoQZGmho4w6&ChoosePayment=Credit&EncryptType=1&ItemName=Item&MerchantID=3002607&MerchantTradeDate=" + TradeDate + "&MerchantTradeNo=" + latestOrderId
                    + $"&OrderResultURL=https://localhost:7119/Customer/Flight/OrderConfirm"
                    + "&PaymentType=aio&ReturnURL=https://localhost:7119/Customer/Flight/OrderConfirm" + "&TotalAmount=" + amount
                    + "&TradeDesc=test&HashIV=EkRm7iFT261dpevs";

                    var encodedCheckstr = HttpUtility.UrlEncode(checkstr).ToLower();
                    string hashedString = "";
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
                        ReturnURL = "https://localhost:7119/Customer/Flight/OrderConfirm",
                        OrderResultURL = "https://localhost:7119/Customer/Flight/OrderConfirm",
                        ChoosePayment = "Credit",
                        EncryptType = 1,
                        CheckMacValue = hashedString
                    };
                    #endregion

                    //回傳訂單編號和支付參數
                    return Ok(new { OrderId = orderId, PaymentParams = paymentParams });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, "訂單成立失敗，請重新再試");
                }
            }
        }



        //路徑：~/api/flightapi/Confirm
        //訂單確認畫面資訊
        [HttpGet]
        [Route("Confirm")]
        public async Task<ActionResult> Confirm(string outgoingScheduleId, string returnScheduleId, string orderId)
        {
            try
            {
                #region 變數處理
                int OutgoingScheduleId = Convert.ToInt32(outgoingScheduleId);
                int ReturnScheduleId = Convert.ToInt32(returnScheduleId);
                int OrderId = Convert.ToInt32(orderId);
                #endregion

                #region 訂單資訊處理
                // 根據 orderId 查詢相關訂單信息
                var order = await _context.TForders
                    .Where(f => f.FId == OrderId)
                    .Select(o => new
                    {
                        o.FId,
                        o.FOrderId,
                        o.FOrderDate,
                        o.FMember.FName, //會員姓名
                        o.FMember.FPhone, //會員電話
                        o.FMember.FEmail, //會員email
                        o.FTotal, //總金額
                        o.FPayment.FPayment, //付款方式
                        o.TForderDetails, //細節
                    })
                    .FirstOrDefaultAsync();

                // 檢查訂單是否存在
                if (order == null)
                {
                    return NotFound();
                }
                #endregion

                #region 去程資料處理
                // 根據 outgoingScheduleId 取得去程航班
                var outgoingFlightDetail = _context.TFflightSchedules
                    .Where(d => d.FScheduleId == OutgoingScheduleId)
                    .Select(d => new
                    {
                        d.FAirline,  //航空公司
                        d.FFlightName,//航班代碼
                        d.FDepartureTime,
                        d.FArrivalTime,
                        d.FDeparture,
                        d.FDestination,
                    })
                    .FirstOrDefault();

                var outgoingPsgerDetail = order.TForderDetails
                    .Where(d => d.FScheduleId == OutgoingScheduleId)
                    .Select(d => new
                    {
                        d.FPsgrName,
                        d.FGender,
                    });

                #endregion

                #region 回程航班資料處理
                // 根據 returnScheduleId 取得回程航班
                var returnFlightDetail = _context.TFflightSchedules
                    .Where(d => d.FScheduleId == ReturnScheduleId)
                    .Select(d => new
                    {
                        d.FAirline,  //航空公司
                        d.FFlightName,//航班代碼
                        d.FDepartureTime,
                        d.FArrivalTime,
                        d.FDeparture,
                        d.FDestination,
                    })
                    .FirstOrDefault();

                #endregion

                // 構建回傳的 JSON 對象
                var response = new
                {
                    Order = order,
                    OutgoingFlightDetail = outgoingFlightDetail,
                    OutgoingPsgerDetail = outgoingPsgerDetail,  //乘客資料
                    ReturnFlightDetail = returnFlightDetail,
                };

                #region 寄送訂單確認信

                // 處理乘客資料
                StringBuilder passengerDetails = new StringBuilder();
                int passengerCount = 1;

                foreach (var passenger in outgoingPsgerDetail)
                {
                    string gender = (bool)passenger.FGender ? "男性" : "女性"; // 將布林值轉換為對應的性別字串
                    passengerDetails.AppendLine($"<p>乘客{passengerCount}</p>");
                    passengerDetails.AppendLine($"<p>乘客姓名：{passenger.FPsgrName}</p>");
                    passengerDetails.AppendLine($"<p>性別：{gender}</p>");
                    passengerCount++;
                }
                //處理顯示參數
                string phoneForEmail = order.FPhone;
                string totalForEmail = ((int)order.FTotal).ToString("0");

                // 客戶信箱
                string customerEmail = order.FEmail;

                // 設置郵件內容
                string subject = $"【TravVita】訂單號 {order.FOrderId} 已成立!";  // 主旨
                string message = @$"<p>親愛的會員，您的訂單號&nbsp;<span style=""color:#3498db""><strong>{order.FOrderId}</strong></span>已成立！</p>
<p><span style=""font-size:16px""><a href=""https://localhost:7119/Customer/Userprofile"">點擊連結至會員中心確認訂單內容</a></span></p>
<hr />
<p><strong>【訂單細節】</strong></p>
<p>【訂單編號】{order.FOrderId}</p>
<p>【訂單日期】{order.FOrderDate}</p>
<p>【總金額】{totalForEmail}</p>
<p>【付款方式】{order.FPayment}</p>
<h4><strong>【訂購人資料】</strong></h4>
<p>【訂購會員姓名】{order.FName}</p>
<p>【電話】{order.FPhone}</p>
<p>【電子郵件】{order.FEmail}</p>
<h4>【行程資料】</h4>
<p>{outgoingFlightDetail.FDeparture.FAirport}　直達　{outgoingFlightDetail.FDestination.FAirport}</p>
<p>【去程】</p>
<p>起飛時間：{outgoingFlightDetail.FDepartureTime}</p>
<p>降落時間：{outgoingFlightDetail.FArrivalTime}</p>
<p>【回程】</p>
<p>起飛時間：{returnFlightDetail.FDepartureTime}</p>
<p>降落時間：{returnFlightDetail.FArrivalTime}</p>
<p><strong>【乘客資料】</strong></p>
{passengerDetails}
<hr />
<p>感謝您使用TravVita服務，有任何問題歡迎隨時與我們聯繫。</p>
";

                // 使用郵件發送者來發送郵件
                EmailSender mailsend = new EmailSender();
                await mailsend.SendEmailAsync(customerEmail, subject, message);
                #endregion


                // 返回 JSON
                return Ok(response);
            }
            catch (Exception ex)
            {
                // 處理異常
                return StatusCode(500, "Internal server error");
            }
        }


    }
}

