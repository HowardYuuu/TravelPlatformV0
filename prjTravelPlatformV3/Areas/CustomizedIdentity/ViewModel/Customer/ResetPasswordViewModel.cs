using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.ViewModel.Customer
{
    public class ResetPasswordViewModel
    {
        public string? token { get; set; }

        [Required(ErrorMessage ="密碼欄位為必填")]
        public string? Password { get; set; }
        public int UserId { get; set; }

    }
}
