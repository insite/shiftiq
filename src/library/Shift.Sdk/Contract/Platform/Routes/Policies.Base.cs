namespace Shift.Contract
{
    public static partial class Policies
    {
        public static class Me
        {
            public const string Context = "Orchestration.Me.Context"; // previously: react/site-settings
        }

        public static partial class Platform
        {
            public static partial class Routes
            {
                public const string Settings = "Platform.Routes.Settings"; // previously: react/page-settings
            }
        }

        public static partial class Security
        {
            public static class Cookies
            {
                public const string Decode = "security/cookies/decode";
                public const string Generate = "security/cookies/generate";
                public const string Introspect = "Security.Cookies.Introspect";
                public const string Validate = "security/cookies/validate";
            }

            public static class Tokens
            {
                public const string Generate = "security/tokens/generate";
                public const string Introspect = "security/tokens/introspect";
                public const string Validate = "security/tokens/validate";
            }
        }

        public static partial class Timeline
        {
            public static partial class Commands
            {
                public const string Send = "Timeline.Commands.Send";
            }

            public const string Queries = "Timeline.Queries";
        }
    }
}