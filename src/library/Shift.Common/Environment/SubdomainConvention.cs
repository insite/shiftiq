using System.Collections.Generic;

namespace Shift.Common
{
    /// <summary>
    /// Names a host-prefix scheme for tenant subdomains. Each deployment serves a single convention: most partitions 
    /// use <see cref="Shift"/>; <c>E03</c> (CMDS) uses <see cref="Cmds"/>.
    /// </summary>
    public static class SubdomainConvention
    {
        public const string Shift = "Shift";
        public const string Cmds = "Cmds";

        /// <summary>
        /// Convention active for the current deployment. Set during application startup
        /// from the partition configuration. Defaults to <see cref="Shift"/>.
        /// </summary>
        public static string Current { get; set; } = Shift;

        public static string GetName(string convention, EnvironmentName environment)
        {
            if (string.Equals(convention, Cmds, System.StringComparison.OrdinalIgnoreCase))
            {
                switch (environment)
                {
                    case EnvironmentName.Production: return "Live";
                    case EnvironmentName.Sandbox: return "Demo";
                    case EnvironmentName.Development: return "Test";
                    case EnvironmentName.Local:
                    default: return "Work";
                }
            }

            return environment.ToString();
        }

        public static string GetPrefix(EnvironmentName environment)
            => GetPrefix(Current, environment);

        public static string GetPrefix(string convention, EnvironmentName environment)
        {
            if (string.Equals(convention, Cmds, System.StringComparison.OrdinalIgnoreCase))
            {
                switch (environment)
                {
                    case EnvironmentName.Production: return "live-";
                    case EnvironmentName.Sandbox: return "demo-";
                    case EnvironmentName.Development: return "test-";
                    case EnvironmentName.Local:
                    default: return "work-";
                }
            }

            switch (environment)
            {
                case EnvironmentName.Production: return string.Empty;
                case EnvironmentName.Sandbox: return "sandbox-";
                case EnvironmentName.Development: return "dev-";
                case EnvironmentName.Local:
                default: return "local-";
            }
        }

        /// <summary>
        /// Every non-empty prefix used by any convention. Used by inbound host parsers
        /// that must accept hosts authored before a convention switch.
        /// </summary>
        public static IReadOnlyList<string> KnownPrefixes { get; } = new[]
        {
            "local-", "dev-", "sandbox-",
            "work-", "test-", "demo-", "live-"
        };

        public static IReadOnlyList<string> KnownBareSubdomains { get; } = new[]
        {
            "local", "dev", "sandbox",
            "work", "test", "demo", "live"
        };
    }
}
