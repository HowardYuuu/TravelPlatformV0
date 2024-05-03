using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Discount
{
    public class CouponViewModel
    {
        public int CouponId { get; set; }

        [Required(ErrorMessage = "折扣碼為必填")]
        [DisplayName("折扣碼")]
        public string? CouponCode { get; set; }

        [Required(ErrorMessage = "名稱為必填")]
        [DisplayName("優惠卷名稱")]
        public string? CouponName { get; set; }

        [Required(ErrorMessage = "金額門檻為必填")]
        [DisplayName("金額門檻")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "啟用日期為必選")]
        [DisplayName("啟用日期")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "結束日期為必選")]
        [DisplayName("結束日期")]
        public DateTime? EndDate { get; set; }

        [DisplayName("使用條件")]
        public string? Rule { get; set; }

        [DisplayName("商品類型")]
        public string? ProductType { get; set; }

        [DisplayName("折扣值")]
        public decimal? Discount { get; set; }

        [DisplayName("備註")]
        public string? Note { get; set; }

        [DisplayName("啟用狀態")]
        public bool? Enable { get; set; }
    }
}
