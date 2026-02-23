namespace Shift.Common
{
    public class TokenSettings
    {
        public string Audience { get; set; }
        public string Whitelist { get; set; }
        public int Lifetime { get; set; }

        public int GetClientSecretLifetimeInDays()
            => 90;
    }
}
