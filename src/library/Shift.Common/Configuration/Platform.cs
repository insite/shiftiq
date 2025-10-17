namespace Shift.Common
{
    public class Platform
    {
        public PlatformIntegrity Integrity { get; set; }
        public PlatformSearch Search { get; set; }
        public PlatformMaintenance Maintenance { get; set; }
    }

    public class PlatformIntegrity
    {
        public ExceptionHandlerSettings[] ExceptionHandlers { get; set; }
    }

    public class PlatformSearch
    {
        public PlatformSearchDownload Download { get; set; }
    }

    public class PlatformSearchDownload
    {
        public int MaximumRows { get; set; }
    }

    public class PlatformMaintenance
    {
        public Lockout[] Lockouts { get; set; }
    }
}