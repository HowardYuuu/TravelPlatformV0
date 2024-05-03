using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Google.Apis.Util;
using MailKit.Net.Smtp;
using prjTravelPlatform_release.Areas.Customer.Model.MailKit;
//using System.Net.Mail;
using System.Net.Mime;
using MimeKit.Utils;

namespace prjTravelPlatform_release.Areas.Customer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailKitController : ControllerBase
    {
        [HttpGet("SendEmail")]
        public async Task<IActionResult> GetEmail([FromQuery] string? CustomerName, string? EmailAddress, string? Subject, string? Body, string? ImgPath)
        {
            try
            {
                #region OAuth驗證
                const string GMailAccount = "msit55ATravelPlatform@gmail.com";

                var clientSecrets = new ClientSecrets
                {
                    ClientId = "454163442968-r9r8j96rp7d54dbqt5csfsj5b2tsf8ro.apps.googleusercontent.com",
                    ClientSecret = "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"
                };

                var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    DataStore = new FileDataStore("CredentialCacheFolder", false),
                    Scopes = new[] { "https://mail.google.com/" },
                    ClientSecrets = clientSecrets
                });

                var codeReceiver = new LocalServerCodeReceiver();
                var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

                var credential = await authCode.AuthorizeAsync(GMailAccount, CancellationToken.None);

                if (credential.Token.IsExpired(SystemClock.Default))
                    await credential.RefreshTokenAsync(CancellationToken.None);

                var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);
                #endregion

                #region 信件內容
                var message = new MimeMessage();
                //寄件者名稱及信箱(信箱是測試帳號)
                message.From.Add(new MailboxAddress("Trav Vita旅遊平台", "msit55ATravelPlatform@gmail.com"));
                //收件者名稱，收件者信箱
                message.To.Add(new MailboxAddress(CustomerName, EmailAddress));
                //信件標題
                message.Subject = Subject;


                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = Body;

                // 添加内联图片
                var img1 = bodyBuilder.LinkedResources.Add(@"wwwroot/img/logoF.png");
                img1.ContentId = "imageLogo";

                var img2 = bodyBuilder.LinkedResources.Add(@"wwwroot/img/Hotel/RoomType/check.png");
                img2.ContentId = "imageChaeck";

                var img3 = bodyBuilder.LinkedResources.Add($"wwwroot/img/Hotel/RoomType/{ImgPath}");
                img3.ContentId = "imageRoomType";

                // 设置消息的正文
                message.Body = bodyBuilder.ToMessageBody();
                             
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587);
                    await client.AuthenticateAsync(oauth2);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                #endregion

                return Ok("ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("SendMailForgetPwd")]
        public async Task<IActionResult> GetEmail2([FromQuery] string? EmailAddress, string? Subject, string? Body)
        {
            try
            {
                #region OAuth驗證
                const string GMailAccount = "msit55ATravelPlatform@gmail.com";

                var clientSecrets = new ClientSecrets
                {
                    ClientId = "454163442968-r9r8j96rp7d54dbqt5csfsj5b2tsf8ro.apps.googleusercontent.com",
                    ClientSecret = "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"
                };

                var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    DataStore = new FileDataStore("CredentialCacheFolder", false),
                    Scopes = new[] { "https://mail.google.com/" },
                    ClientSecrets = clientSecrets
                });

                var codeReceiver = new LocalServerCodeReceiver();
                var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

                var credential = await authCode.AuthorizeAsync(GMailAccount, CancellationToken.None);

                if (credential.Token.IsExpired(SystemClock.Default))
                    await credential.RefreshTokenAsync(CancellationToken.None);

                var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);
                #endregion

                #region 信件內容
                var message = new MimeMessage();
                //寄件者名稱及信箱(信箱是測試帳號)
                message.From.Add(new MailboxAddress("Trav Vita旅遊平台", "msit55ATravelPlatform@gmail.com"));
                //收件者名稱，收件者信箱
                message.To.Add(new MailboxAddress("", EmailAddress));
                //信件標題
                message.Subject = Subject;


                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = Body;

                // 添加内联图片
                var img1 = bodyBuilder.LinkedResources.Add(@"wwwroot/img/logoF.png");
                img1.ContentId = "imageLogo";

                // 设置消息的正文
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587);
                    await client.AuthenticateAsync(oauth2);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                #endregion

                return Ok("ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
