namespace Shift.Common
{
    public class SecuritySettings
    {
        public string Autologin { get; set; }
        public string Domain { get; set; }
        public string Secret { get; set; }
        public SentinelsSettings Sentinels { get; set; }
        public TokenSettings Token { get; set; }
        public CookieSettings Cookie { get; set; }
    }
}
