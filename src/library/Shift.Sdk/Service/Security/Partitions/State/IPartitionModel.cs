using System;

namespace InSite.Domain
{
    public interface IPartitionModel
    {
        string Host { get; set; }
        string Email { get; set; }

        string WhitelistDomains { get; set; }
        string WhitelistEmails { get; set; }

        Guid Identifier { get; set; }
        string Name { get; set; }
        string Slug { get; set; }

        int Number { get; set; }

        bool IsE01();
        bool IsE02();
        bool IsE03();
        bool IsE04();
        bool IsE07();

        string GetPlatformSlug();
        string GetPlatformName();

        int DatabaseMonitorLargeCommandSize { get; set; }
        bool DatabaseMonitorIncludeStackTrace { get; set; }
    }

    public class PartitionModel : IPartitionModel
    {
        public string Host { get; set; }
        public string Email { get; set; }

        public string WhitelistDomains { get; set; }
        public string WhitelistEmails { get; set; }

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public int Number { get; set; }

        public bool IsE01() => string.Compare(Slug, "E01", true) == 0;
        public bool IsE02() => string.Compare(Slug, "E02", true) == 0;
        public bool IsE03() => string.Compare(Slug, "E03", true) == 0;
        public bool IsE04() => string.Compare(Slug, "E04", true) == 0;
        public bool IsE07() => string.Compare(Slug, "E07", true) == 0;

        public string GetPlatformSlug() => IsE03() ? "cmds" : "shift";
        public string GetPlatformName() => IsE03() ? "CMDS" : "Shift iQ";

        public int DatabaseMonitorLargeCommandSize { get; set; }
        public bool DatabaseMonitorIncludeStackTrace { get; set; }
    }
}