using prjTravelPlatformV3.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Visa
{
    public class VOrderViewModel
    {
        
        public int FId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "未選擇商品")]
        [DisplayName("商品名稱")]
        public int FProductId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "未選填購買會員")]
        [DisplayName("會員姓名")]
        public int FCustomerId { get; set; }

        [DisplayName("商品價格")]
        public decimal? FPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未新增旅客")]
        [DisplayName("購買數量")]
        public int FQuantity { get; set; }

        [Required(ErrorMessage = "未選擇預計出國日")]
        [DisplayName("預計出國日")]
        public string FDepartureDate { get; set; }

        [DisplayName("自取或配送地址")]
        public string? FForPickupOrDeliveryAddress { get; set; }

        [DisplayName("是否已提醒面試")]
        public bool? FInterviewReminder { get; set; }

        [DisplayName("評價")]
        public int? FEvaluate { get; set; }

        [DisplayName("備註")]
        public string? FMemo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇訂單狀態")]
        [DisplayName("訂單狀態")]
        public int? FStatusId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇使用優惠")]
        [DisplayName("使用優惠")]
        public int? FCouponId { get; set; }


        [DisplayName("旅客資料")]
        public virtual ICollection<TVtravelerInfo> TVtravelerInfos { get; set; } = new List<TVtravelerInfo>();

        private string[]? _travelerInfos { get; set; }

        [Required(ErrorMessage = "請寫入旅客資訊")]
        public string[]? travelerInfos
        {
            get { return _travelerInfos; }
            set
            {
                _travelerInfos = value;
                TVtravelerInfos = conv(value);
            }
        }
        private ICollection<TVtravelerInfo>? conv(string[]? travelers)
        {
            if (travelers ==  null)
            {
                return null;
            }
            int chunkSize = 4;
            IEnumerable<string[]> travelersChunks = travelers.Select((value,index) => new { Index = index, Value = value})
                .GroupBy(x => x.Index / chunkSize)
                .Select(grp => grp.Select(x => x.Value).ToArray());
            List<TVtravelerInfo> list = new List<TVtravelerInfo>();
            foreach (var traveler in travelersChunks)
            {
                list.Add(new TVtravelerInfo
                {
                    FOrderId = Convert.ToInt32(traveler[0]),
                    FName = traveler[1],
                    FGender = Convert.ToBoolean(traveler[2]),
                    FBirthDate = traveler[3]
                });
            }
            return list;
        }
    }
}
