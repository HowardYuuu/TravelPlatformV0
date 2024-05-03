namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Airline
{
    public class PassengerViewModel
    {
        public string Type { get; set; } // 乘客類型，成人、孩童、幼兒
        public string LastName { get; set; } // 姓氏
        public string FirstName { get; set; } // 名字
        public string NationalId { get; set; } // 身分證字號
        public Boolean Gender { get; set; } // 性别，true代表男性，false代表女性
        public DateTime Birth { get; set; } // 出生日期
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
