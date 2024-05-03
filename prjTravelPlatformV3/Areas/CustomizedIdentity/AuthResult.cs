namespace prjTravelPlatform_release.Areas.CustomizedIdentity
{
    public class AuthResult
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool? Success { get; set; }
        public List<string>? Errors { get; set; }
        public int Exp { get; set; }
    }
}
