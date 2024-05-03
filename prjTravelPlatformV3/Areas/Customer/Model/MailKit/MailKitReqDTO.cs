namespace prjTravelPlatform_release.Areas.Customer.Model.MailKit
{
    public class MailKitReqDTO
    {
        public string? CustomerName { get; set; }
        public string? EmailAddress { get; set;}
        public string? Subject { get; set;}
        public string? Body { get; set;}
        public List<string>? Imgs { get; set; }
    }
}
