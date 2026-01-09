namespace Shift.Common
{
    public class ThrottlingSettings
    {
        public class Settings
        {
            public int TokenLimit { get; set; }
            public int QueueLimit { get; set; }
            public int ReplenishmentPeriodInSeconds { get; set; }
            public int TokensPerPeriod { get; set; }
            public string Status { get; set; }

            public bool IsEnabled => string.Equals(Status, "Enabled");
        }

        public Settings Authenticated { get; set; }
        public Settings Anonymous { get; set; }
    }
}
