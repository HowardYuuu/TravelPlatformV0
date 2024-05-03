using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace prjTravelPlatform_release.Areas.Customer.ViewModel.Airline
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("suk19960912@gmail.com", "Trav Vita 旅遊平台");  //寄件者
            mail.To.Add(email);
            mail.Subject = subject;  //主旨
            mail.Body = htmlMessage;  //內文
            mail.IsBodyHtml = true; //內容是否為HTML
            mail.Priority = MailPriority.Normal;
            //建立Smtp物件，並設定Gmail的smtp主機及Port號
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("suk19960912@gmail.com", "kvhhjcsxplxgxvma");  //設定寄件信箱的帳密
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}
