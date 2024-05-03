using prjTravelPlatformV3.Models;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Discounts
{
    public class CusCouponViewModel
    {
        public List<TDcCouponList>? Coupon {  get; set; }
        public List<TDcCusCouponQty>? CusCouQty { get; set;}
        public List<VDcusCouponView>? VCouponQty { get; set; }
    }
}
