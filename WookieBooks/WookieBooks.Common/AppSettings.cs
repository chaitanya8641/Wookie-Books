namespace WookieBooks.Common
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
        public string Expires { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
