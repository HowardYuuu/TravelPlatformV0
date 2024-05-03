using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.Customer.ViewModel.Hotels;
//using prjTravelPlatform_release.Data.DTO.Hotel;
using prjTravelPlatformV3.Models;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using prjTravelPlatform_release.Areas.Customer.Model.Hotel;
using System.Linq;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Hotel
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelApiController : ControllerBase
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelApiController(dbTravalPlatformContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        [Route("RegionFilter")]
        public IActionResult GetRegionFilter([FromQuery] string keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                return BadRequest();
            }
            var list = new List<HotelDFilterModel>();

            var city = _context.TtravelAreas.Where(x => x.FareaName.Contains(keyword));
            var hotel = _context.THotels.Where(x => x.FRegion.Contains(keyword) || x.FHotelName.Contains(keyword));
            if (city != null)
            {
                foreach (var item in city)
                {
                    list.Add(new HotelDFilterModel
                    {
                        Name = item.FareaName,
                        Category = "region"
                    });
                }
            }
            if (hotel != null)
            {
                var hDistinct = hotel.GroupBy(x => x.FHotelName).Select(g => g.First()).ToList();
                foreach (var item in hDistinct)
                {
                    list.Add(new HotelDFilterModel
                    {
                        Name = item.FHotelName,
                        Category = "hotel"
                    });
                }
            }
            return Ok(list);
        }


        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> PostHotelList([FromBody] HotelSearchDTO searchDTO)
        {

            List<THotel>? hotel = null;
            List<int> hFacilitiesIdList = new List<int>();
            List<int> rFacilitiesIdList = new List<int>();

            int pageSize = !searchDTO.PageSize.HasValue ? 5 : (int)searchDTO.PageSize;
            int page = !searchDTO.Page.HasValue ? 1 : (int)searchDTO.Page;

            string sortBy = String.IsNullOrEmpty(searchDTO.SortBy) ? "id" : searchDTO.SortBy;
            string sortType = String.IsNullOrEmpty(searchDTO.SortType) ? "asc" : searchDTO.SortType;

            var allHotel = await _context.THotels.ToListAsync();

            if (searchDTO != null)
            {

                //處理destination
                if (searchDTO.Category == "hotel")
                {
                    hotel = allHotel.Where(h => h.FHotelName == searchDTO.Destination).ToList();
                }
                else
                {
                    hotel = allHotel.Where(x => x.FRegion.Contains(searchDTO.Destination)).ToList();

                    if (searchDTO.FilterObj != null)
                    {
                        //價錢篩選
                        var joined = (from h in hotel
                                      join rt in _context.THroomTypes
                                      on h.FHotelId equals rt.FHotelId
                                      select new
                                      {
                                          hotelId = h.FHotelId,
                                          price = rt.FPrice
                                      })
                                      .Where(x => x.price >= searchDTO.FilterObj.MinPrice && x.price <= searchDTO.FilterObj.MaxPrice)
                                      .Select(x => x.hotelId)
                                      .Distinct();

                        hotel = hotel.Where(h => joined.Contains(h.FHotelId)).ToList();

                        //星等篩選
                        if (searchDTO.FilterObj.Rank != null && searchDTO.FilterObj.Rank.Count != 0)
                        {
                            hotel = hotel.Where(h => searchDTO.FilterObj.Rank.Contains((int)h.FRank)).ToList();
                        }
                        //評分篩選
                        if (searchDTO.FilterObj.Score != null && searchDTO.FilterObj.Score.Count != 0)
                        {
                            hotel = hotel.Where(h => searchDTO.FilterObj.Score.Any(Score => h.FScore > Score)).ToList();
                        }
                        //設施篩選
                        if (searchDTO.FilterObj.Facilities != null && searchDTO.FilterObj.Facilities.Count != 0)
                        {
                            foreach (var item in searchDTO.FilterObj.Facilities)
                            {
                                string category = item.Split("|")[0];
                                int id = Int32.Parse(item.Split("|")[1]);
                                if (category == "HotelFacility")
                                {
                                    hFacilitiesIdList.Add(id);
                                }
                                else if (category == "RoomFacility")
                                {
                                    rFacilitiesIdList.Add(id);
                                }

                            }
                            //飯店設施
                            var hFacitities = from a in allHotel
                                              join hf in _context.THfacilityRelations
                                              on a.FHotelId equals hf.FHotelId
                                              where hFacilitiesIdList.Contains(hf.FHotelFacilityId)
                                              group a by a.FHotelId into g
                                              select g.Key;
                            //房型設施
                            var rFacilities = from a in allHotel
                                              join rt in _context.THroomTypes
                                              on a.FHotelId equals rt.FHotelId
                                              join rh in _context.THroomTypeFacilityRelations
                                              on rt.FRoomTypeId equals rh.FRoomTypeId
                                              where rFacilitiesIdList.Contains(rh.FRoomTypeFacilityId)
                                              group a by a.FHotelId into g
                                              select g.Key;

                            var grouped = hFacitities.Concat(rFacilities).Distinct().ToList();
                            hotel = hotel.Where(h => grouped.Contains(h.FHotelId)).ToList();
                        }
                    }
                }
            }

            var hotelCards = new List<HotelCard>();

            var hotelCardList = getHotelCardList(hotel, hotelCards);

            switch (sortBy)
            {
                case "price":
                    hotelCardList = sortType == "asc" ? hotelCardList.OrderBy(s => Decimal.Parse(s.Price.ToString())).ToList() : hotelCardList.OrderByDescending(s => Decimal.Parse(s.Price.ToString())).ToList();
                    break;

                case "comment":
                    hotelCardList = sortType == "asc" ? hotelCardList.OrderBy(s => Decimal.Parse(s.Score)).ToList() : hotelCardList.OrderByDescending(s => Decimal.Parse(s.Score)).ToList();
                    break;

                default:
                    hotelCardList = sortType == "asc" ? hotelCardList.OrderBy(s => s.HotelId).ToList() : hotelCardList.OrderByDescending(s => s.HotelId).ToList();
                    break;
            }

            //使用者收藏處理
            if (User.Identity != null && User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) == "Customer")
            {
                int currentUserId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var collectedHotelsId = _context.THcollections.Where(x => x.FCustomerId == currentUserId).Select(x => x.FHotelId);
                foreach (var hotelCard in hotelCardList)
                {
                    // 檢查 hotel card 的 ID 是否存在於 collectedHotelsId 中
                    if (collectedHotelsId.Contains(hotelCard.HotelId))
                    {
                        // 如果存在，將 IsCollected 設置為 true
                        hotelCard.isCollected = true;
                    }
                }
            }
            var searchResDTO = new HotelSearchResDTO()
            {
                TotalPages = (int)Math.Ceiling((decimal)hotelCardList.Count() / pageSize),
                TotalCount = hotelCardList.Count(),
                HotelCards = hotelCardList.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList(),
            };
            return Ok(searchResDTO);
        }


        [HttpGet]
        [Route("HotelFacilities")]
        public IActionResult GetHotelFacilities()
        {
            var fHList = from item in _context.THfacilities
                         select new
                         {
                             id = item.FHotelFacilityId,
                             name = item.FHotelFacilityName,
                             category = "HotelFacility"
                         };

            return Ok(fHList);
        }



        [HttpGet]
        [Route("RoomFacilities")]
        public IActionResult GetRoomFacilities()
        {
            var fRList = from item in _context.THroomFacilities
                         select new
                         {
                             id = item.FRoomTypeFacilityId,
                             name = item.FRoomTypeFacilityName,
                             category = "RoomFacility"
                         };
            return Ok(fRList);
        }


        [HttpPost]
        [Route("Room")]
        public async Task<IActionResult> PostRoomInfo([FromBody] RoomSearchDTO roomSearch)
        {
            if(roomSearch == null)
            {
                return BadRequest(new { success = false, message = "無效的payload" });
            }

            //篩選日期及hotelId
            var availabelRoom = await _context
                .GetProcedures()
                .GetAvailableRoomsEditFilterByHotelIdAsync(roomSearch.HotelId, DateTime.Parse(roomSearch.StartDate), DateTime.Parse(roomSearch.EndDate));

            //將相同roomTypeId group起來，並取得入住限制人數
            var availabelRoomGroupByRoomTypeId = availabelRoom.GroupBy(r => r.fRoomTypeId)
                                .Select(g => new
                                {
                                    roomTypeId = g.Key,
                                    maxCapacity = g.FirstOrDefault()?.fMaxCapacity
                                });

                                         
            List<THroomType> roomTypes = new List<THroomType>();

            //根據availabelRoom有的roomTypeId加入roomTypes
            foreach (var item in availabelRoomGroupByRoomTypeId)
            {
                var room = _context.THroomTypes.FirstOrDefault(r => r.FRoomTypeId == item.roomTypeId);            
                roomTypes.Add(room);
            }

            List<RoomCardDTO> roomCards = new List<RoomCardDTO>();
            int personNum = Int32.Parse(roomSearch.PersonNum);

            //加入roomCards中，用於回傳資料
            foreach (var item in roomTypes)
            {
                var roomCard = new RoomCardDTO();
                switch (personNum)
                {
                    case 1:
                        roomCard.Message = "";
                        roomCard.MessageValue = 0;
                        break;
                    case 2:
                        roomCard.Message = "";
                        roomCard.MessageValue = 0;
                        break;
                    case 3:
                        if (item.FMaxCapacity == 2)
                        {
                            roomCard.Message = $"選取的人數不符合房型人數限制，建議訂2間房";
                            roomCard.MessageValue = 2;
                        }
                        else
                        {
                            roomCard.Message = "";
                            roomCard.MessageValue = 0;
                        }
                        break;
                    case 4:
                        if (item.FMaxCapacity == 2)
                        {
                            roomCard.Message = $"選取的人數不符合房型人數限制，建議訂2間房";
                            roomCard.MessageValue = 2;
                        }
                        else
                        {
                            roomCard.Message = "";
                            roomCard.MessageValue = 0;
                        }
                        break;
                    default:
                        if (item.FMaxCapacity == 2)
                        {
                            int a = personNum / 2;
                            int b = personNum % 2;
                            roomCard.Message = $"選取的人數不符合房型人數限制，建議訂{a + 1}間房";
                            roomCard.MessageValue = a + 1;
                        }
                        if (item.FMaxCapacity == 4)
                        {
                            int a = personNum / 4;
                            int b = personNum % 4;
                            roomCard.Message = $"選取的人數不符合房型人數限制，建議訂{a + 1}間房";
                            roomCard.MessageValue = a + 1;
                        }
                        break;
                }

                roomCard.RoomTypeId = item.FRoomTypeId;
                roomCard.RoomTypeName = item.FRoomTypeName;
                roomCard.RoomPrice = item.FPrice.ToString("#,0");
                roomCard.BedNum = item.FBedNum;
                roomCard.BedType = item.FBedType;
                roomCard.MaxCapacity = item.FMaxCapacity;



                roomCard.RooomFacilities = (from i in _context.THroomTypeFacilityRelations
                                            join rf in _context.THroomFacilities
                                            on i.FRoomTypeFacilityId equals rf.FRoomTypeFacilityId
                                            where i.FRoomTypeId == item.FRoomTypeId
                                            select rf.FRoomTypeFacilityName).ToList();

                var roomCount = from room in availabelRoom
                                where room.fRoomTypeId == item.FRoomTypeId
                                group room by room.fRoomTypeId into g
                                select g.Count();

                roomCard.RoomCount = roomCount.FirstOrDefault();
                if (roomCount.FirstOrDefault() < 4)
                {
                    roomCard.RoomCountMsg = $"只剩{roomCount.FirstOrDefault()}間嘍!";
                }
                else
                {
                    roomCard.RoomCountMsg = "";
                }

                var roomImg = from roomImgs in _context.THroomTypeImages
                              where roomImgs.FRoomTypeId == item.FRoomTypeId
                              select roomImgs.FRoomImage;

                if (!roomImg.Any())
                {
                    roomCard.RoomImg = "";
                }
                else
                {
                    roomCard.RoomImg = roomImg.FirstOrDefault();
                }
                roomCards.Add(roomCard);
            }

            return Ok(roomCards.OrderBy(x => x.MaxCapacity).ThenBy(x=>x.RoomPrice));
        }


        [HttpPost]
        [Route("OrderInit")]
        public IActionResult OrderInit([FromBody] HotelOrderInitDTO hotelOrderInitDTO)
        {
            if (hotelOrderInitDTO == null)
            {
                return BadRequest();
            }
            var selectedRoomtype = _context.THroomTypes.FirstOrDefault(rt => rt.FRoomTypeId == hotelOrderInitDTO.RoomTypeId);
            if (selectedRoomtype == null)
            {
                return StatusCode(500);
            }
            var roomSelectedImg = _context.THroomTypeImages.Where(ri => ri.FRoomTypeId == selectedRoomtype.FRoomTypeId).Select(r => r.FRoomImage).FirstOrDefault();
            int dayCount = (DateTime.Parse(hotelOrderInitDTO.EndDate) - DateTime.Parse(hotelOrderInitDTO.StartDate)).Days;
            decimal price = selectedRoomtype.FPrice;
            decimal totalPrice = (price * Int32.Parse(hotelOrderInitDTO.RoomNum) * dayCount);
            double tax = (double)totalPrice * 0.05;
            int totalPriceAddTax = (int)(totalPrice) + (int)(tax);
            var hotelReserveViewModel = new HotelOrderViewModel
            {
                HotelId = (int)selectedRoomtype.FHotelId,
                HotelName = _context.THotels.FirstOrDefault(h => h.FHotelId == selectedRoomtype.FHotelId).FHotelName,
                RoomTypeId = hotelOrderInitDTO.RoomTypeId,
                RoomTypeName = selectedRoomtype.FRoomTypeName,
                StartDate = hotelOrderInitDTO.StartDate,
                EndDate = hotelOrderInitDTO.EndDate,
                PersonNum = hotelOrderInitDTO.PersonNum,
                RoomNum = hotelOrderInitDTO.RoomNum.ToString(),
                Price = price.ToString("#,0"),
                TotalPrice = totalPrice.ToString("#,0"),
                TotalPriceAddTax = totalPriceAddTax.ToString("#,0"),
                RoomImg = roomSelectedImg,
                BedType = selectedRoomtype.FBedType,
                BedNum = selectedRoomtype.FBedNum.ToString(),
                Tax = tax.ToString("#,0"),
                DayCount = dayCount
            };
            return Ok(hotelReserveViewModel);
        }


        [HttpPost]
        [Route("Order")]       
        public IActionResult PostOrder([FromBody] HotelOrderDTO hotelOrderDTO)
        {
            if (hotelOrderDTO == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "無效的payload"
                });
            }

            if(User.Identities == null || !User.Identity.IsAuthenticated || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) != "Customer")
            {
                return Unauthorized(new 
                {
                    success = false,
                    message = "未授權"
                });

            }

            var order = new THorder
            {
                FCustomerId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                FHotelId = hotelOrderDTO.HotelId,
                FRoomTypeId = hotelOrderDTO.RoomTypeId,
                FCheckInDate = DateTime.Parse(hotelOrderDTO.StartDate),
                FCheckOutDate = DateTime.Parse(hotelOrderDTO.EndDate),
                FGuestCount = Int32.Parse(hotelOrderDTO.PersonNum),
                FRoomCount = Int32.Parse(hotelOrderDTO.RoomNum),
                FOriginalUnitPrice = Decimal.Parse(hotelOrderDTO.Price),
                FTotalPrice = Decimal.Parse(hotelOrderDTO.TotalPriceAddTax),
                FPaymentId = hotelOrderDTO.PaymentId,
                FPaymentStatusId = hotelOrderDTO.PaymentStatus,
                FOrderDate = DateTime.Now.ToString("yyyy/MM/dd"),
                FPayDate = "",
                FGuestName = hotelOrderDTO.Name,
                FGuestEmail = hotelOrderDTO.Email,
                FGuestAddress = hotelOrderDTO.Address,
                FGuestPhone = hotelOrderDTO.Phone,
            };

            var lastOrderId = InsertOrderToDb(order);

            if (lastOrderId.Success)
            {
                for (int i = 0; i < hotelOrderDTO.RoomIds.Count(); i++)
                {
                    var oderDetail = new THorderDetail
                    {
                        FHotelOrderId = lastOrderId.LastOrderId,
                        FRoomId = Int32.Parse(hotelOrderDTO.RoomIds[i]),
                    };
                    _context.THorderDetails.Add(oderDetail);
                }
                _context.SaveChanges();
                return Ok(new
                {
                    status = lastOrderId.Success,
                    lastOrderId = lastOrderId.LastOrderId,
                });
            }
            return StatusCode(500);
        }


        [HttpPut]
        [Route("UpdateOrder")]
        public IActionResult UpdateOrder([FromBody] HotelOrderUpdateDTO orderUpdateDTO)
        {
            var order = _context.THorders.FirstOrDefault(o => o.FHotelOrderId == orderUpdateDTO.OrderId);
            if (order != null)
            {
                order.FPayDate = DateTime.Now.ToString();
                order.FPaymentStatusId = 2;
                _context.Update(order);
                _context.SaveChanges();
                return Ok(new
                {
                    success = true,
                });

            }
            return BadRequest(new
            {
                success = false,
            });
        }



        //處裡收藏API     
        [HttpGet]
        [Route("Collection")]
        
        public IActionResult InsertColletion([FromQuery]int hotelId)
        {
            
            if (!User.Identity.IsAuthenticated || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) != "Customer")
            {
                return Unauthorized(new
                {
                    success = false,
                });
            }
            int currentUserId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tHCollection = new THcollection
            {
                FCustomerId = currentUserId,
                FHotelId = hotelId,
            };
            try
            {
                _context.THcollections.Add(tHCollection);
                _context.SaveChanges();
                return Ok(new { success =  true});

            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }        
        }


        
        [HttpDelete]
        [Route("Collection")]
        public IActionResult DeleteCollection([FromQuery] int hotelId)
        {
            
            if (!User.Identity.IsAuthenticated || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) != "Customer")
            {
                return Unauthorized(new
                {
                    success = false,
                });
            };

            int currentUserId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tHCollection = new THcollection
            {
                FCustomerId = currentUserId,
                FHotelId = hotelId,
            };
            try
            {
                _context.THcollections.Remove(tHCollection);
                _context.SaveChanges();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }



        #region private function
        private List<HotelCard> getHotelCardList(List<THotel>? tHotel, List<HotelCard> hotelCards)
        {
            foreach (var hotel in tHotel)
            {
                var h = new HotelCard();
                h.HotelId = hotel.FHotelId;
                h.HotelName = hotel.FHotelName;
                h.HotelEngName = hotel.FHotelEngName;
                h.City = hotel.FHotelAddress.Substring(0, 3);
                h.Region = hotel.FHotelAddress.Substring(3, 3);
                h.Rank = hotel.FRank;
                h.Score = hotel.FScore == null ? "0" : hotel.FScore.ToString();

                //評分中文敘述
                if (hotel.FScore != null && (double)hotel.FScore > 4.5)
                {
                    h.ScoreStr = "超讚";
                }
                else if ((hotel.FScore != null && (double)hotel.FScore > 4 && (double)hotel.FScore <= 4.5))
                {
                    h.ScoreStr = "讚";
                }
                else
                {
                    h.ScoreStr = "普通";
                }
                //價格
                h.Price = _context.THroomTypes
                        .Where(r => r.FHotelId == hotel.FHotelId)
                        .Select(r => r.FPrice).Min().ToString("#,0");

                var f = _context.THfacilityRelations
                    .Where(r => r.FHotelId == hotel.FHotelId)
                    .ToList();

                List<string> fList = new List<string>();
                foreach (var i in f)
                {
                    string name = FacilityIdToName(i.FHotelFacilityId);
                    fList.Add(name);
                }
                h.HotelsFacilitiesName = fList;

                var tHImages = _context.THimages
                    .Where(r => r.FHotelId == hotel.FHotelId).Select(x => x.FHotelImage);

               
                h.HotelsImagies = tHImages == null? new List<string> { "noimage.jpg" } : tHImages.ToList();
               
                

                h.CommentCount = _context.THcomments
                                .Where(r => r.FHotelId == hotel.FHotelId)
                                .Count();


                hotelCards.Add(h);
            }
            return hotelCards;
        }

        private async Task<List<THotel>> GetAvailableHotel(string checkinDate, string checkoutDate)
        {
            var avalibleHotel = await _context
                .GetProcedures()
                .GetAvailableRoomsAsync(DateTime.Parse(checkinDate), DateTime.Parse(checkoutDate));

            var hotelIdList = avalibleHotel.Select(x => x.fHotelId).Distinct();

            List<THotel> filter = new List<THotel>();

            foreach (var id in hotelIdList)
            {
                var filtedhotel = _context.THotels.FirstOrDefault(h => h.FHotelId == id);
                if (filtedhotel != null)
                {
                    filter.Add(filtedhotel);
                }
            }
            return filter;
        }

       
        private string? FacilityIdToName(int id)
        {
            string? name = _context.THfacilities.Where(r => r.FHotelFacilityId == id).Select(r => r.FHotelFacilityName).FirstOrDefault(); ;
            return name;
        }

        private InsertMsg InsertOrderToDb(THorder order)
        {
            var insertMsg = new InsertMsg();
            try
            {
                _context.THorders.Add(order);
                _context.SaveChanges();
                string? lastOrderId = _context.THorders.OrderByDescending(hr => hr.FHotelOrderId).Select(hr => hr.FHotelOrderId).FirstOrDefault();

              
                insertMsg.Success = true;
                insertMsg.LastOrderId = lastOrderId;
                return insertMsg;
            }
            catch (Exception ex)
            {
                insertMsg.Success = false;
                insertMsg.Message = $"寫入資料庫發生錯誤: {ex.Message}";
                return insertMsg;
            }
        }
        public class InsertMsg
        {
            public bool Success { get; set; }
            public string? LastOrderId { get; set; }
            public string? Message { get; set; }
        }


        //private async Task<List<THroomType>> GetAvailableRoom(int id, string checkinDate, string checkoutDate)
        //{
        //    var availabelRoom = await _context
        //        .GetProcedures()
        //        .GetAvailableRoomsByHotelIdAsync(id, DateTime.Parse(checkinDate), DateTime.Parse(checkoutDate));

        //    //var hotelIdList = avalibleHotel.Select(x => x.fHotelId).Distinct();

        //    //List<THotel> filter = new List<THotel>();

        //    //foreach (var id in hotelIdList)
        //    //{
        //    //    var filtedhotel = _context.THotels.FirstOrDefault(h => h.FHotelId == id);
        //    //    if (filtedhotel != null)
        //    //    {
        //    //        filter.Add(filtedhotel);
        //    //    }
        //    //}
        //    //return filter;
        //}
        #endregion

    }
}
