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

        private static readonly string[] ShiftPrefixes = { "local-", "dev-", "sandbox-" };
        private static readonly string[] CmdsPrefixes = { "work-", "test-", "demo-", "live-" };
        private static readonly string[] ShiftBareSubdomains = { "local", "dev", "sandbox" };
        private static readonly string[] CmdsBareSubdomains = { "work", "test", "demo", "live" };

        /// <summary>
        /// Prefixes valid for the currently active convention. Used by parsers that must
        /// reject tenant subdomains whose codes happen to collide with a sibling
        /// convention's environment names (e.g. tenant <c>demo</c> on a Shift partition).
        /// </summary>
        public static IReadOnlyList<string> ActivePrefixes
            => string.Equals(Current, Cmds, System.StringComparison.OrdinalIgnoreCase)
                ? CmdsPrefixes
                : ShiftPrefixes;

        /// <summary>
        /// Bare environment subdomains valid for the currently active convention.
        /// </summary>
        public static IReadOnlyList<string> ActiveBareSubdomains
            => string.Equals(Current, Cmds, System.StringComparison.OrdinalIgnoreCase)
                ? CmdsBareSubdomains
                : ShiftBareSubdomains;
    }
}
