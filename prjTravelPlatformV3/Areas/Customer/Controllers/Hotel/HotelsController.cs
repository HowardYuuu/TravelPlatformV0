using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using prjTravelPlatform_release.Areas.Customer.Model.Hotel;
using prjTravelPlatform_release.Areas.Customer.SendMailService;
using prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels;
using prjTravelPlatform_release.Extended.MailSend;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Hotel
{
    [Area("Customer")]
    public class HotelsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        //private readonly IEmailSender _emailSender;
        private readonly MailService _sendMailService;

        public HotelsController(dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor, MailService sendMailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _sendMailService = sendMailService;
        }
        public IActionResult Index()
        {
            ViewBag.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var hotels2= _context.THotels.Include(x => x.THroomTypes).Where(x => x.THroomTypes != null).ToList(); from i in hotels1
            var h= (from i in _context.THotels
                           join o in _context.THroomTypes
                           on i.FHotelId equals o.FHotelId
                           group i by i.FHotelId into g
                           select g.Key);

            var hotels = _context.THotels.Where(x => h.Contains(x.FHotelId)).ToList();

        

            List<int> selectedNumbers = new List<int>();

            Random rand = new Random();
            while (selectedNumbers.Count < 8)
            {
                int randomNumber = rand.Next(0, hotels.Count());
                if (!selectedNumbers.Contains(randomNumber))
                {
                    // 過濾重複後存入selectedList
                    selectedNumbers.Add(randomNumber);
                }
            }

            List<THotel> hotels1 = selectedNumbers.Select(index => hotels[index]).ToList();

            //var hotels2 = from i in hotels1
            //                          join o in _context.THroomTypes
            //                          on i.FHotelId equals o.FHotelId into temp
            //                          from o in temp.DefaultIfEmpty()
            //                          where o != null 
            //                          select i;

            List<HotelSuggestViewModel> hotelSuggestViews = new List<HotelSuggestViewModel>();
            HotelFilterModel hotelFilterModel = new HotelFilterModel();
            foreach (var i in hotels1)
            {
                string city = i.FRegion;
                string region = i.FHotelAddress.Substring(3, 3);
                string ScoreStr = "";

                if ((double)i.FScore > 4.5)
                {
                    ScoreStr = "超讚";
                }
                else if ((double)i.FScore <= 4.5 && (double)i.FScore > 4)
                {
                    ScoreStr = "讚";
                }
                else
                {
                    ScoreStr = "普通";
                }
                var viewModel = new HotelSuggestViewModel
                {
                    HotelId = i.FHotelId,
                    HotelName = i.FHotelName,
                    HotelImg = _context.THimages
                                    .Where(x => x.FHotelId == i.FHotelId)
                                    .Select(x => x.FHotelImage)
                                    .FirstOrDefault(),
                    Address = city + " " + region,
                    Score = i.FScore,
                    ScoreStr = ScoreStr,
                    Price = _context.THroomTypes.Where(x => x.FHotelId == i.FHotelId).OrderByDescending(y => y.FPrice).Select(y => y.FPrice).FirstOrDefault()
                };
                hotelSuggestViews.Add(viewModel);
            }
            hotelFilterModel.HotelSuggests = hotelSuggestViews.OrderByDescending(p=>p.Score).ToList();

            return View(hotelFilterModel);
        }


        [HttpPost]
        public IActionResult List(HotelFilterModel filterLIst)
        {
            var tHotel = _context.THotels.ToList();
            var hotelCards = new List<HotelCard>();
            foreach (var hotel in tHotel)
            {
                var h = new HotelCard();
                h.HotelName = hotel.FHotelName;
                h.HotelEngName = hotel.FHotelEngName;
                h.City = hotel.FHotelAddress.Substring(0, 3);
                h.Region = hotel.FHotelAddress.Substring(3, 3);

                h.Price = _context.THroomTypes
                        .Where(r => r.FHotelId == hotel.FHotelId)
                        .Select(r => r.FPrice).FirstOrDefault().ToString("#,0");

                var f = _context.THfacilityRelations
                    .Where(r => r.FHotelId == hotel.FHotelId).ToList();

                List<string> fList = new List<string>();
                foreach (var i in f)
                {
                    string name = FacilityIdToName(i.FHotelFacilityId);
                    fList.Add(name);
                }
                h.HotelsFacilitiesName = fList;

                var p = _context.THimages
                    .Where(r => r.FHotelId == hotel.FHotelId).Select(x => x.FHotelImage).ToList();

                h.HotelsImagies = p;

                hotelCards.Add(h);
            }
            var hotelCardViewModel = new HotelListViewModel()
            {
                HotelCards = hotelCards,
                HotelFilter = filterLIst
            };


            return View(hotelCardViewModel);
        }

        public IActionResult ListEdit()
        {
            return View();
        }


        public IActionResult HotelDetail(string id)
        {
            int hotelId = Int32.Parse(id);

            var hotel = _context.THotels.FirstOrDefault(h => h.FHotelId == hotelId);

            if (hotel == null)
            {
                return RedirectToAction("ListEdit");
            }
            //飯店設施
            var hFacilities = _context.THfacilityRelations
                .Where(f => f.FHotelId == hotelId)
                .Join(_context.THfacilities,
                ra => ra.FHotelFacilityId,
                f => f.FHotelFacilityId,
                (ra, f) => f.FHotelFacilityName).ToList();

            //飯店圖片
            var hImgs = _context.THimages
                .Where(i => i.FHotelId == hotelId)
                .Select(i => i.FHotelImage).ToList();

            //房間設施
            var rFacilities = (from ra in _context.THroomTypeFacilityRelations
                               join r in _context.THroomFacilities
                               on ra.FRoomTypeFacilityId equals r.FRoomTypeFacilityId
                               join rt in _context.THroomTypes
                               on ra.FRoomTypeId equals rt.FRoomTypeId
                               where rt.FHotelId == hotelId
                               select r.FRoomTypeFacilityName).ToList();
            //房間圖片        
            var rImgs = (from rti in _context.THroomTypeImages
                         join rt in _context.THroomTypes
                         on rti.FRoomTypeId equals rt.FRoomTypeId
                         join h in _context.THotels
                         on rt.FHotelId equals h.FHotelId
                         where rt.FHotelId == hotelId
                         select rti.FRoomImage).ToList();

            HotelDetailViewModel hViewModel = new HotelDetailViewModel
            {
                HotelId = hotel.FHotelId,
                HotelName = hotel.FHotelName,
                HotelEngName = hotel.FHotelEngName,
                Region = hotel.FRegion,
                Address = hotel.FHotelAddress,
                HotelFacilities = hFacilities,
                HotelsImgs = hImgs,
                RoomFacilities = rFacilities,
                RoomImgs = rImgs,
                Intro = hotel.FIntro,
                Rank = hotel.FRank,
                Score = hotel.FScore,
                HotelComments = _context.HotelCommentViews
                                .Where(h => h.FHotelId == hotel.FHotelId).ToList(),
            };

            return View(hViewModel);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> HotelOrder(int HotelId, int RoomTypeId, string StartDate, string EndDate, string PersonNum, string RoomNum, int RoomNumSuggest)
        {
            var availabelRoom = await _context
                .GetProcedures()
                .GetAvailableRoomsEditFilterByHotelIdAsync(HotelId, DateTime.Parse(StartDate), DateTime.Parse(EndDate));

            int roomNumFianl = Int32.Parse(RoomNum);
            ////處理房間數量
            //if (RoomNumSuggest != 0)
            //{
            //    if (roomNumFianl != RoomNumSuggest)
            //    {
            //        roomNumFianl = (int)RoomNumSuggest;
            //    }
            //}

            //依數量選取房間
            var tt = availabelRoom.Where(r => r.fRoomTypeId == RoomTypeId).Take(roomNumFianl);



            //依選取到的房間回推房型資訊
            var roomTypeInfoFromSelectedRoom = _context.THroomTypes.Where(r => r.FRoomTypeId == tt.First().fRoomTypeId).First();

            var roomSelectedImg = _context.THroomTypeImages.Where(ri => ri.FRoomTypeId == roomTypeInfoFromSelectedRoom.FRoomTypeId).Select(r => r.FRoomImage).First();
            int dayCount = (DateTime.Parse(EndDate) - DateTime.Parse(StartDate)).Days;
            decimal price = roomTypeInfoFromSelectedRoom.FPrice;
            decimal totalPrice = (price * roomNumFianl * dayCount);
            double tax = (double)totalPrice * 0.05;
            int totalPriceAddTax = (int)(totalPrice) + (int)(tax);
            var hotelReserveViewModel = new HotelOrderViewModel
            {
                HotelId = HotelId,
                HotelName = _context.THotels.FirstOrDefault(h => h.FHotelId == roomTypeInfoFromSelectedRoom.FHotelId).FHotelName,
                RoomTypeId = RoomTypeId,
                RoomTypeName = roomTypeInfoFromSelectedRoom.FRoomTypeName,
                StartDate = StartDate,
                EndDate = EndDate,
                PersonNum = PersonNum,
                RoomNum = roomNumFianl.ToString(),
                Price = price.ToString("#,0"),
                TotalPrice = totalPrice.ToString("#,0"),
                TotalPriceAddTax = totalPriceAddTax.ToString("#,0"),
                RoomImg = roomSelectedImg,
                BedType = roomTypeInfoFromSelectedRoom.FBedType,
                BedNum = roomTypeInfoFromSelectedRoom.FBedNum.ToString(),
                Tax = tax.ToString("#,0"),
                DayCount = dayCount
            };


            hotelReserveViewModel.RoomIds = new List<int>();
            hotelReserveViewModel.RoomNumbers = new List<string>();
            foreach (var i in tt)
            {
                hotelReserveViewModel.RoomIds?.Add(i.fRoomId);
                hotelReserveViewModel.RoomNumbers?.Add(i.fRoomNumber);
            }
            return View(hotelReserveViewModel);
        }



        [Authorize(Roles = "Customer")]
        public IActionResult HotelOrderConfirm()
        {
            var orderInfo = _context.THorders.OrderByDescending(hr => hr.FHotelOrderId).FirstOrDefault();
            if (orderInfo != null)
            {
                var hotelConfirmViewModel = new HotelConfirmViewModel
                {
                    HotelOrderId = orderInfo.FHotelOrderId,
                    CustomerId = orderInfo.FCustomerId,
                    CustomerName = _context.TCustomers.Where(c => c.FCustomerId == orderInfo.FCustomerId).Select(c => c.FName).First(),
                    HotelId = orderInfo.FHotelId,
                    RoomTypeId = orderInfo.FRoomTypeId,
                    HotelName = _context.THotels.Where(c => c.FHotelId == orderInfo.FHotelId).Select(c => c.FHotelName).First(),
                    RoomTypeName = _context.THroomTypes.Where(c => c.FRoomTypeId == orderInfo.FRoomTypeId).Select(c => c.FRoomTypeName).First(),
                    StartDate = orderInfo.FCheckInDate?.ToString("yyyy/MM/dd"),
                    EndDate = orderInfo.FCheckOutDate?.ToString("yyyy/MM/dd"),
                    PersonNum = orderInfo.FGuestCount,
                    RoomNum = orderInfo.FRoomCount,
                    Price = orderInfo.FOriginalUnitPrice?.ToString("#,0"),
                    TotalPrice = orderInfo.FTotalPrice?.ToString("#,0"),
                    OrderDate = orderInfo.FOrderDate,
                    PayDate = "",
                    DayCount = (orderInfo.FCheckOutDate - orderInfo.FCheckInDate)?.Days,
                    Tax = ((double)orderInfo.FTotalPrice * 0.05).ToString("#,0"),
                    Name = orderInfo.FGuestName,
                    Email = orderInfo.FGuestEmail,
                    Phone = orderInfo.FGuestPhone,
                    Address = orderInfo.FGuestAddress,
                    BedNum = _context.THroomTypes.Where(rt => rt.FRoomTypeId == orderInfo.FRoomTypeId).Select(x => x.FBedNum).First(),
                    BedType = _context.THroomTypes.Where(rt => rt.FRoomTypeId == orderInfo.FRoomTypeId).Select(x => x.FBedType).First(),
                };
                var roomImg = (from ri in _context.THroomTypeImages
                               where ri.FRoomTypeId == orderInfo.FRoomTypeId
                               select ri.FRoomImage).FirstOrDefault();

                hotelConfirmViewModel.RoomImg = roomImg;

                var room = from od in _context.THorderDetails
                           join o in _context.THorders
                           on od.FHotelOrderId equals o.FHotelOrderId
                           join r in _context.THrooms
                           on od.FRoomId equals r.FRoomId
                           where od.FHotelOrderId == orderInfo.FHotelOrderId
                           select new
                           {
                               roomId = od.FRoomId,
                               roomNumber = r.FRoomNumber,
                           };

                hotelConfirmViewModel.RoomIds = new List<string>();
                hotelConfirmViewModel.RoomsNumbers = new List<string>();
                foreach (var od in room)
                {
                    hotelConfirmViewModel.RoomIds.Add(od.roomId.ToString());
                    hotelConfirmViewModel.RoomsNumbers.Add(od.roomNumber.ToString());
                }
                return View(hotelConfirmViewModel);
            }
            return View();
        }

        [Authorize(Roles = "Customer")]
        public IActionResult HotelOrderCancel()
        {
            return View();
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> HotelOrderCompleted(string hotelOrderId)
        {
            var order = _context.THorders.FirstOrDefault(o => o.FHotelOrderId == hotelOrderId);
            if (order == null)
            {
                return View("Error");
            }
            var hotelCompletedViewModel = new HotelConfirmViewModel
            {
                HotelOrderId = order.FHotelOrderId,
                CustomerId = order.FCustomerId,
                CustomerName = _context.TCustomers.Where(c => c.FCustomerId == order.FCustomerId).Select(c => c.FName).First(),
                HotelId = order.FHotelId,
                RoomTypeId = order.FRoomTypeId,
                HotelName = _context.THotels.Where(c => c.FHotelId == order.FHotelId).Select(c => c.FHotelName).First(),
                HotelAddress = _context.THotels.Where(c => c.FHotelId == order.FHotelId).Select(c => c.FHotelAddress).First(),
                HotelPhone = _context.THotels.Where(c => c.FHotelId == order.FHotelId).Select(c => c.FPhone).First(),
                RoomTypeName = _context.THroomTypes.Where(c => c.FRoomTypeId == order.FRoomTypeId).Select(c => c.FRoomTypeName).First(),
                StartDate = order.FCheckInDate?.ToString("yyyy/MM/dd"),
                EndDate = order.FCheckOutDate?.ToString("yyyy/MM/dd"),
                PersonNum = order.FGuestCount,
                RoomNum = order.FRoomCount,
                Price = order.FOriginalUnitPrice?.ToString("#,0"),
                TotalPrice = order.FTotalPrice?.ToString("#,0"),
                OrderDate = order.FOrderDate,
                PayDate = order.FPayDate,
                DayCount = (order.FCheckOutDate - order.FCheckInDate)?.Days,
                Tax = ((double)order.FTotalPrice * 0.05).ToString("#,0"),
                Name = order.FGuestName,
                Email = order.FGuestEmail,
                Phone = order.FGuestPhone,
                Address = order.FGuestAddress,
                BedNum = _context.THroomTypes.Where(rt => rt.FRoomTypeId == order.FRoomTypeId).Select(x => x.FBedNum).First(),
                BedType = _context.THroomTypes.Where(rt => rt.FRoomTypeId == order.FRoomTypeId).Select(x => x.FBedType).First(),
            };
            var roomImg = (from ri in _context.THroomTypeImages
                           where ri.FRoomTypeId == order.FRoomTypeId
                           select ri.FRoomImage).FirstOrDefault();

            hotelCompletedViewModel.RoomImg = roomImg;

            var room = from od in _context.THorderDetails
                       join o in _context.THorders
                       on od.FHotelOrderId equals o.FHotelOrderId
                       join r in _context.THrooms
                       on od.FRoomId equals r.FRoomId
                       where od.FHotelOrderId == order.FHotelOrderId
                       select new
                       {
                           roomId = od.FRoomId,
                           roomNumber = r.FRoomNumber,
                       };

            hotelCompletedViewModel.RoomIds = new List<string>();
            hotelCompletedViewModel.RoomsNumbers = new List<string>();

            foreach (var od in room)
            {
                hotelCompletedViewModel.RoomIds.Add(od.roomId.ToString());
                hotelCompletedViewModel.RoomsNumbers.Add(od.roomNumber.ToString());
                hotelCompletedViewModel.RoomNumberStr = hotelCompletedViewModel.RoomNumberStr + od.roomNumber.ToString() + " ";
            }

            //寄信
            string mailContent = MailContentFormat.HotelFormatStr(hotelCompletedViewModel);
            string receiverEmail = hotelCompletedViewModel.Email;
            string subject = "訂房成功通知";

            //await _emailSender.SendEmailAsync(receiverEmail, subject, mailContent, hotelCompletedViewModel.RoomImg);SendMailAsync
            await _sendMailService.SendMailAsync(order.FGuestName, receiverEmail, subject, MailContentFormat.HotelFormatStr(hotelCompletedViewModel), roomImg);
            return View(hotelCompletedViewModel);
        }

        public string? FacilityIdToName(int id)
        {
            string? name = _context.THfacilities.Where(r => r.FHotelFacilityId == id).Select(r => r.FHotelFacilityName).FirstOrDefault(); ;
            return name;
        }


    }
}
