namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Airline
{
    public class OrderViewModel
    {
        public List<PassengerViewModel> Passengers { get; set; } // 乘客信息列表
        public int OutgoingScheduleId { get; set; } // 出發航班ID
        public int ReturnScheduleId { get; set; } // 返回航班ID
        public int PaymentId { get; set; }  //會員ID
        public int CustomerId { get; set; }  //會員ID
        public string CustomerEmail { get; set; }  //會員Email
    }
}
