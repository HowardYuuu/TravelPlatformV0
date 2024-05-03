using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using prjTravelPlatformV3.Models;
using System.Text;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Airline
{
    [Route("/api/Flight/{action}/{id?}")]
    public class FApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public FApiController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult loadFlightData()
        {
            return Json(_context.FscheduleViews);
        }

        public IActionResult loadOrderData()
        {
            return Json(_context.FordersViews);
        }

        #region 新增航班
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlight(IFormCollection form)
        {
            #region 變數
            int ticketPrice;
            DateTime depTime;
            DateTime ArrTime;
            #endregion
            try
            {
                #region 檢查
                if (!Int32.TryParse(form["FTicketPrice"], out ticketPrice))
                {
                    //錯誤，非數字
                    return Content("請輸入有效的票價數字", "text/html", Encoding.UTF8);
                }

                if (!DateTime.TryParse(form["FDepartureTime"], out depTime))
                {
                    //轉換成DateTime
                }

                if (!DateTime.TryParse(form["FArrivalTime"], out ArrTime))
                {
                    //轉換成DateTime
                }
                #endregion

                #region 資料更新
                var newFlight = new TFflightSchedule
                {
                    //     将视图模型中的数据映射到航班实例中
                    FAirlineId = Int32.Parse(form["FAirlineId"]),
                    FFlightName = form["FFlightName"],
                    FDepartureTime = depTime,
                    FArrivalTime = ArrTime,
                    FDepartureId = Int32.Parse(form["FDepartureId"]),
                    FDestinationId = Int32.Parse(form["FDestinationId"]),
                    FTicketPrice = ticketPrice,
                    FClassId = Int32.Parse(form["FClassId"]),
                    FQty = Int32.Parse(form["FQty"])
                };
                // 然后将新航班添加到数据库上下文中
                _context.TFflightSchedules.Add(newFlight);
                _context.SaveChanges();
                #endregion

                return Content("資料新增成功", "text/html", Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Content("資料新增失敗", "text/html", Encoding.UTF8);
            }
        }
        #endregion

        #region 編輯航班

        //編輯-根據id載入資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFlight_Load(IFormCollection form)
        {
            try
            {
                int id = Int32.Parse(form["id"]);  //前端由URL抓取的id
                var maindata = _context.TFflightSchedules.Where(x => x.FScheduleId == id).FirstOrDefault();
                if (maindata == null)
                {
                    return Content(JsonConvert.SerializeObject(new JObject { { "Error", "無此資料!" } }), "application/json");
                }
                JObject jo_result = new JObject();
                jo_result.Add("fScheduleId", maindata.FScheduleId); //id
                jo_result.Add("fAirlineId", maindata.FAirlineId); //航空公司
                jo_result.Add("fFlightName", maindata.FFlightName); //航班代碼
                jo_result.Add("fDepartureTime", maindata.FDepartureTime); //起飛時間
                jo_result.Add("fArrivalTime", maindata.FArrivalTime); //降落時間
                jo_result.Add("fDepartureId", maindata.FDepartureId); //起飛地
                jo_result.Add("fDestinationId", maindata.FDestinationId); //降落地
                jo_result.Add("fClassId", maindata.FClassId); //艙等
                jo_result.Add("fTicketPrice", maindata.FTicketPrice); //定價
                jo_result.Add("fQty", maindata.FQty);
                return Content(JsonConvert.SerializeObject(jo_result), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new JObject { { "Error", $"資料查詢出錯! {ex.Message}" } }), "application/json");
            }
        }

        //編輯-更新資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFlight_Update(IFormCollection form)
        {
            try
            {
                int id = Int32.Parse(form["fScheduleId"]);  //前端formData中的id
                string FlightName = form["fFlightName"];
                int ticketPrice;

                #region 檢查
                //票價須為數字
                if (!Int32.TryParse(form["FTicketPrice"], out ticketPrice))
                {
                    //轉換失敗
                    return Content("請輸入有效的票價數字", "text/html", Encoding.UTF8);
                }
                //航班代碼不可重複
                int tmp_chkFN = _context.TFflightSchedules.Where(x => x.FFlightName == FlightName && x.FScheduleId != id).Count();
                if (tmp_chkFN > 0)
                {
                    return Content("航班代碼重複", "text/html", Encoding.UTF8);
                }
                #endregion

                #region 更新資料
                var maindata = _context.TFflightSchedules.Where(x => x.FScheduleId == id).FirstOrDefault();
                maindata.FAirlineId = Int32.Parse(form["FAirlineId"]);
                maindata.FFlightName = FlightName;
                maindata.FDepartureTime = DateTime.Parse(form["fDepartureTime"]);
                maindata.FArrivalTime = DateTime.Parse(form["fArrivalTime"]);
                maindata.FDepartureId = Int32.Parse(form["fDepartureId"]);
                maindata.FDestinationId = Int32.Parse(form["fDestinationId"]);
                maindata.FTicketPrice = decimal.Parse(form["fTicketPrice"]);
                maindata.FClassId = Int32.Parse(form["fClassId"]);
                maindata.FQty = Int32.Parse(form["fQty"]);
                _context.TFflightSchedules.Update(maindata);
                _context.SaveChanges();
                #endregion

                return Content("資料儲存成功", "text/html", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content($"資料儲存失敗 {ex.Message}", "text/html", Encoding.UTF8);
            }
        }
        #endregion

        #region 刪除航班
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoDeleteFlight(IFormCollection form)
        {
            int id = Int32.Parse(form["Id"]);

            try
            {
                #region 檢查訂單是否存在
                var maindata = _context.TFflightSchedules.Where(x => x.FScheduleId == id).FirstOrDefault();
                if (maindata == null)
                {
                    return Content($"無此訂單!", "text/html", Encoding.UTF8);
                }
                #endregion

                #region 刪除資料
                _context.TFflightSchedules.Remove(maindata);
                _context.SaveChanges();
                #endregion

                return Content($"刪除成功!", "text/html", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content($"刪除失敗! {ex.Message}", "text/html", Encoding.UTF8);
            }
        }
        #endregion

        #region 航班填充下拉選項
        [HttpGet]
        public IActionResult dr_airline()
        {
            var maindata = _context.TCcompanyInfos.Where(x => x.FType == "F").ToList();
            //創建一個空的JsonArray
            JArray ja_result = new JArray();
            foreach (var itemdata in maindata)
            {
                //用foreach讀取集合中的航空公司資料，塞進去JObject中
                JObject jo_data = new JObject();
                jo_data.Add("Text", itemdata.FCompanyName); //使用Key:Value pair
                jo_data.Add("Value", itemdata.FId);  //使用Key:Value pair
                ja_result.Add(jo_data);  //最後將JObject塞入JArray
            }
            //回傳JArray結果
            return Content(JsonConvert.SerializeObject(ja_result), "application/json");
        }
        [HttpGet]
        public IActionResult dr_airport()
        {
            var maindata = _context.TFairportInfos.ToList();
            JArray ja_result = new JArray();
            foreach (var itemdata in maindata)
            {
                JObject jo_data = new JObject();
                jo_data.Add("Text", itemdata.FAirport);
                jo_data.Add("Value", itemdata.FAirportId);
                ja_result.Add(jo_data);
            }
            return Content(JsonConvert.SerializeObject(ja_result), "application/json");
        }
        public IActionResult dr_class()
        {
            var maindata = _context.TFclasses;
            JArray ja_result = new JArray();
            foreach (var itemdata in maindata)
            {
                JObject jo_data = new JObject();
                jo_data.Add("Text", itemdata.FClass);
                jo_data.Add("Value", itemdata.FClassId);
                ja_result.Add(jo_data);
            }
            return Content(JsonConvert.SerializeObject(ja_result), "application/json");
        }
        #endregion

        #region 編輯訂單

        ////編輯-根據id載入資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrder_Load(IFormCollection form)
        {
            try
            {
                int id = Int32.Parse(form["id"]);
                var maindata = _context.TForders.Where(x => x.FId == id).FirstOrDefault();
                if (maindata == null)
                {
                    return Content(JsonConvert.SerializeObject(new JObject { { "Error", "無此資料!" } }), "application/json");
                }
                //訂單
                JObject jorder_result = new JObject();
                jorder_result.Add("fOrderId", maindata.FOrderId); //訂單編號
                jorder_result.Add("fMemberId", maindata.FMemberId); //會員編號
                jorder_result.Add("fOrderDate", maindata.FOrderDate); //訂單日期
                jorder_result.Add("fOrderStatusId", maindata.FOrderStatusId); //訂單狀態
                jorder_result.Add("fPaymentStatusId", maindata.FPaymentStatusId); //付款狀態
                jorder_result.Add("fPaymentId", maindata.FPaymentId); //付款方式
                jorder_result.Add("fCouponId", maindata.FCouponId); //折扣碼
                jorder_result.Add("fTotal", maindata.FTotal); //總金額
                return Content(JsonConvert.SerializeObject(jorder_result), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new JObject { { "Error", $"資料查詢出錯! {ex.Message}" } }), "application/json");
            }
        }

        //編輯-更新資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrder_Update(IFormCollection form)
        {
            try
            {
                int id = Int32.Parse(form["fId"]);
                int total;
                Console.WriteLine(id);
                #region 檢查
                //票價須為數字
                if (!Int32.TryParse(form["fTotal"], out total))
                {
                    //轉換失敗
                    return Content("請輸入有效的金額數字", "text/html", Encoding.UTF8);
                }
                #endregion

                #region 更新資料
                var maindata = _context.TForders.Where(x => x.FId == id).FirstOrDefault();
                maindata.FCouponId = Int32.Parse(form["fCouponId"]);
                maindata.FTotal = Int32.Parse(form["fTotal"]);
                _context.TForders.Update(maindata);
                _context.SaveChanges();
                #endregion

                return Content("資料儲存成功", "text/html", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content($"資料儲存失敗 {ex.Message}", "text/html", Encoding.UTF8);
            }
        }
        #endregion

        #region 刪除訂單
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoDeleteOrder(IFormCollection form)
        {
            int id = Int32.Parse(form["Id"]);

            try
            {
                #region 檢查訂單是否存在
                var maindata = _context.TForders.Where(x => x.FId == id).FirstOrDefault();
                if (maindata == null)
                {
                    return Content($"無此訂單!", "text/html", Encoding.UTF8);
                }
                #endregion

                #region 刪除相關聯的訂單明細
                var detaildata = _context.TForderDetails.Where(x => x.FOrderId == maindata.FOrderId).ToList();
                _context.TForderDetails.RemoveRange(detaildata);
                #endregion

                #region 刪除訂單資料
                _context.TForders.Remove(maindata);
                _context.SaveChanges();
                #endregion

                return Content($"刪除成功!", "text/html", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content($"刪除失敗! {ex.Message}", "text/html", Encoding.UTF8);
            }
        }
        #endregion

        #region 訂單填充下拉選項

        //訂單狀態、付款狀態選項
        [HttpGet]
        public IActionResult dr_status()
        {
            var maindata = _context.TForderStatuses.ToList();

            JArray ja_result = new JArray();
            foreach (var itemdata in maindata)
            {
                JObject jo_data = new JObject();
                jo_data.Add("Text", itemdata.FOrderStatus);
                jo_data.Add("Value", itemdata.FOrderStatusId);
                ja_result.Add(jo_data);
            }
            //回傳JArray結果
            return Content(JsonConvert.SerializeObject(ja_result), "application/json");

        }
        //付款方式
        public IActionResult dr_payment()
        {
            var maindata = _context.TFpayments.ToList();

            JArray ja_result = new JArray();
            foreach (var itemdata in maindata)
            {
                JObject jo_data = new JObject();
                jo_data.Add("Text", itemdata.FPayment);
                jo_data.Add("Value", itemdata.FPaymentId);
                ja_result.Add(jo_data);
            }
            //回傳JArray結果
            return Content(JsonConvert.SerializeObject(ja_result), "application/json");

        }

        #endregion
    }
}
