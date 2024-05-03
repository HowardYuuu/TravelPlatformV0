namespace prjTravelPlatform_release.Extended.MailSend
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string receiverEmail, string subject, string message, string? imgPath);
        Task SendEmailAsync(string receiverEmail, string subject, string message);

    }
}
