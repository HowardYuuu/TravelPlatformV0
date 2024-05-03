using Microsoft.IdentityModel.JsonWebTokens;
using prjTravelPlatform_release.Areas.Customer.SendMailService;
using prjTravelPlatform_release.Extended.MailSend;
using prjTravelPlatformV3.Models;
using System.Security.Claims;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity
{
    public class PasswordResetService
    {
        private readonly TokenManager _tokenManager;
        //private readonly IEmailSender _emailService;
        private readonly MailService _sendMailService;

        public PasswordResetService(TokenManager tokenManager, MailService sendMailService)
        {
            _tokenManager = tokenManager;
            _sendMailService = sendMailService;          
        }

        public async Task SendPasswordResetEmail(string userEmail, string resetPasswordUrl)
        {
            // 生成重置密码令牌，设置过期时间为 30 分钟
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = _tokenManager.CreateToken(claims);

            var resetPasswordLink = $"{resetPasswordUrl}?email={userEmail}&token={token}";

            string message = MailContentFormat.ForgetPwdStr(resetPasswordLink);
            // 發送郵件給用戶
            await _sendMailService.SendMailAsync(userEmail, "重設密碼通知", message);
        }   
    }
}
