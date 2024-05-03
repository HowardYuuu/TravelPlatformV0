using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "姓名為必填")]
        [DisplayName("員工名稱")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "帳號為必填")]
        [DisplayName("帳號")]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "密碼為必填")]
        [DisplayName("密碼")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "電話為必填")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "長度不符")]
        [DisplayName("電話")]
        public string? Phone { get; set; }

        [DisplayName("身分證字號")]
        public string? IdentityNumber { get; set; }

        [DisplayName("性別")]
        public string? Gender { get; set; }

        [DisplayName("出生日期")]
        public string? Birth { get; set; }

        [DisplayName("電子信箱")]
        public string? Email { get; set; }

        [DisplayName("住址")]
        public string? Address { get; set; }

        [DisplayName("狀態")]
        public bool? Status { get; set; }

        [DisplayName("職位")]
        public int? StaffId { get; set; }
    }
}

