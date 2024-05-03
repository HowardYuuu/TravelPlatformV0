using System.Net.Mail;
using System.Net;
using prjTravelPlatformV3.Models;
using System.Net.Mime;
using System.Security.Policy;

namespace prjTravelPlatform_release.Extended.MailSend
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;  
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string receiverEmail, string subject, string message, string? imgPath)
        {
            var SenderEmail = _configuration["SmtpSettings:Email"];
            var pw = _configuration["SmtpSettings:Password"];
            var client = new SmtpClient("smtp.gmail.com", Int32.Parse(_configuration["SmtpSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SenderEmail, pw)
            };

            var mailMessage = new MailMessage();

            // 發信者地址名稱
            mailMessage.From = new MailAddress(SenderEmail, "Trav Vita 旅遊整合平台");

            // 收件人mail
            mailMessage.To.Add(receiverEmail);

            // 主旨及內容
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = message;

            LinkedResource linkedImage1 = new LinkedResource("wwwroot/img/logoF.png");
            linkedImage1.ContentId = "imageLogo";

            LinkedResource linkedImage2 = new LinkedResource("wwwroot/img/Hotel/RoomType/check.png");
            linkedImage2.ContentId = "imageChaeck";

            LinkedResource linkedImage3 = new LinkedResource($"wwwroot/img/Hotel/RoomType/{imgPath}");
            linkedImage3.ContentId = "imageRoomType";


            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedImage1);
            alternateView.LinkedResources.Add(linkedImage2);
            alternateView.LinkedResources.Add(linkedImage3);
            mailMessage.AlternateViews.Add(alternateView);


            return client.SendMailAsync(mailMessage);
        }
        public Task SendEmailAsync(string receiverEmail, string subject, string message)
        {
            var SenderEmail = _configuration["SmtpSettings:Email"];
            var pw = _configuration["SmtpSettings:Password"];
            var client = new SmtpClient("smtp.gmail.com", Int32.Parse(_configuration["SmtpSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SenderEmail, pw)
            };

            var mailMessage = new MailMessage();

            // 發信者地址名稱
            mailMessage.From = new MailAddress(SenderEmail, "Trav Vita 旅遊整合平台");

            // 收件人mail
            mailMessage.To.Add(receiverEmail);

            // 主旨及內容
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = message;

            LinkedResource linkedImage1 = new LinkedResource("wwwroot/img/logoF.png");
            linkedImage1.ContentId = "imageLogo";


            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedImage1);
            
            mailMessage.AlternateViews.Add(alternateView);


            return client.SendMailAsync(mailMessage);
        }

    }
}
