using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public enum AccessOperation { Deny, Grant }

    public enum AccessType { Authority, Data, Feature }

    [Flags]
    public enum AuthorityAccess
    {
        Unspecified = 0,          // This does NOT represent access granted to nobody, or access denied to everyone
        Guest = 1 << 0,         // Everyone, including unauthenticated users
        Member = 1 << 1,        // Everyone, excluding unauthenticated users
        Trainee = 1 << 2,       // Authenticated newcomers who are onboarding with orientation
        Learner = 1 << 3,       // Authenticated students, employees, contractors, etc.
        Instructor = 1 << 4,    // Authenticated teachers, content authors, etc.
        Validator = 1 << 5,     // Authenticated subject matter experts
        Supervisor = 1 << 6,    // Authenticated low- and medium-level management
        Manager = 1 << 7,       // Authenticated high-level management
        Administrator = 1 << 8, // Authenticated field, office, and system administrators
        Developer = 1 << 9,     // Authenticated programmers and infrastructure administrators (dev/ops)
        Operator = 1 << 10      // Authenticated root-level platform administrators with full, unrestricted access
    }

    [Flags]
    public enum DataAccess
    {
        Unspecified = 0,
        Read = 1 << 0,
        Update = 1 << 1,
        Create = 1 << 2,
        Delete = 1 << 3,
        Administrate = 1 << 4,
        Configure = 1 << 5
    }

    [Flags]
    public enum FeatureAccess
    {
        Unspecified = 0,
        Trial = 1 << 0, // Feature is visible in UI/menus and available for trial/evaluation
        Use = 1 << 1    // Feature is fully enabled and can be executed/invoked/used
    }

    /// <summary>
    /// Generic base class for access control helpers.
    /// </summary>
    public abstract class AccessHelper<T> where T : struct, Enum
    {
        public T Value { get; set; }

        protected AccessHelper(T value) => Value = value;

        protected abstract T EmptyValue { get; }

        protected abstract Dictionary<T, string> Abbreviations { get; }

        public bool IsEmpty => EqualityComparer<T>.Default.Equals(Value, EmptyValue);

        public string Abbreviate(T value) => Abbreviations.TryGetValue(value, out var abbrev) ? abbrev : "-";

        public string Abbreviate()
        {
            if (!HasAny())
                return "-";

            var result = new StringBuilder();

            foreach (var kvp in Abbreviations.Where(kvp => HasFlag(kvp.Key)))
                result.Append(kvp.Value);

            return result.ToString();
        }

        public string Describe()
        {
            if (!HasAny())
                return Value.ToString();

            var flagNames = Abbreviations.Keys.Where(HasFlag).Select(v => v.ToString());

            return string.Join(", ", flagNames);
        }

        public bool HasAll()
        {
            var allFlags = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(v => Convert.ToInt32(v) > 0)
                .Aggregate(0, (acc, v) => acc | Convert.ToInt32(v));

            return Convert.ToInt32(Value) == allFlags;
        }

        public bool HasAll(string value) => HasFlag(Parse(value));

        /// <summary>
        /// Returns true only if all overlapping flags are set.
        /// </summary>
        public bool HasFlag(T value) => Value.HasFlag(value);

        public bool HasAny() => Convert.ToInt32(Value) > 0;

        /// <summary>
        /// Returns true if any overlapping flags are set.
        /// </summary>
        public bool HasAny(T value)
        {
            var accessInt = Convert.ToInt32(value);

            var valueInt = Convert.ToInt32(Value);

            return (accessInt & valueInt) != 0;
        }

        public T Add(string[] values)
        {
            foreach (var value in values)
                Value = Add(Parse(value));

            return Value;
        }

        public T Add(T value)
        {
            var combined = Convert.ToInt32(Value) | Convert.ToInt32(value);

            Value = (T)Enum.ToObject(typeof(T), combined);

            return Value;
        }

        protected T Parse(string value)
        {
            if (value == "*" || value.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ParseAll();

            return Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : EmptyValue;
        }

        protected T ParseAll()
        {
            if (typeof(T) == typeof(FeatureAccess))
                return (T)Enum.ToObject(typeof(T), FeatureAccess.Use);

            var allFlags = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(v => Convert.ToInt32(v) > 0)
                .Aggregate(0, (acc, v) => acc | Convert.ToInt32(v));

            return (T)Enum.ToObject(typeof(T), allFlags);
        }
    }

    public class AuthorityAccessHelper : AccessHelper<AuthorityAccess>
    {
        public AuthorityAccessHelper(AuthorityAccess value) : base(value) { }

        protected override AuthorityAccess EmptyValue => AuthorityAccess.Unspecified;

        protected override Dictionary<AuthorityAccess, string> Abbreviations => new Dictionary<AuthorityAccess, string>()
        {
            [AuthorityAccess.Guest] = "g",
            [AuthorityAccess.Member] = "b",
            [AuthorityAccess.Trainee] = "t",
            [AuthorityAccess.Learner] = "l",
            [AuthorityAccess.Instructor] = "i",
            [AuthorityAccess.Validator] = "v",
            [AuthorityAccess.Supervisor] = "s",
            [AuthorityAccess.Manager] = "m",
            [AuthorityAccess.Administrator] = "a",
            [AuthorityAccess.Developer] = "d",
            [AuthorityAccess.Operator] = "o"
        };

        public bool Guest => HasFlag(AuthorityAccess.Guest);
        public bool Member => HasFlag(AuthorityAccess.Member);
        public bool Trainee => HasFlag(AuthorityAccess.Trainee);
        public bool Learner => HasFlag(AuthorityAccess.Learner);
        public bool Instructor => HasFlag(AuthorityAccess.Instructor);
        public bool Validator => HasFlag(AuthorityAccess.Validator);
        public bool Supervisor => HasFlag(AuthorityAccess.Supervisor);
        public bool Manager => HasFlag(AuthorityAccess.Manager);
        public bool Administrator => HasFlag(AuthorityAccess.Administrator);
        public bool Developer => HasFlag(AuthorityAccess.Developer);
        public bool Operator => HasFlag(AuthorityAccess.Operator);
    }

    public class DataAccessHelper : AccessHelper<DataAccess>
    {
        public DataAccessHelper(DataAccess value) : base(value) { }

        protected override DataAccess EmptyValue => DataAccess.Unspecified;

        protected override Dictionary<DataAccess, string> Abbreviations => new Dictionary<DataAccess, string>()
        {
            [DataAccess.Read] = "r",
            [DataAccess.Update] = "u",
            [DataAccess.Create] = "c",
            [DataAccess.Delete] = "d",
            [DataAccess.Administrate] = "a",
            [DataAccess.Configure] = "f"
        };

        public bool Read => HasFlag(DataAccess.Read);
        public bool Update => HasFlag(DataAccess.Update);
        public bool Create => HasFlag(DataAccess.Create);
        public bool Delete => HasFlag(DataAccess.Delete);
        public bool Administrate => HasFlag(DataAccess.Administrate);
        public bool Configure => HasFlag(DataAccess.Configure);

        // An operation may be a query (to read something) or a command (to update or delete something). It is important
        // to move toward a consistent naming convention for these operations. The following properties are mainly for
        // documentation purposes, but may be useful in writing code also.

        public static string[] StandardCreateVerbs = new string[] { "create", "import" };
        public static string[] LegacyCreateVerbs = new string[] { "insert", "upload" };

        public static string[] StandardReadVerbs = new string[] { "assert", "collect", "count", "download", "retrieve", "search", "export" };
        public static string[] LegacyReadVerbs = new string[] { "read", "outline", "view" };

        public static string[] StandardUpdateVerbs = new string[] { "update" };
        public static string[] LegacyUpdateVerbs = new string[] { "edit", "modify" };

        public static string[] StandardDeleteVerbs = new string[] { "delete", "purge" };
        public static string[] LegacyDeleteVerbs = new string[] { "remove", "void" };

        public static bool IsRecognized(string verb)
        {
            var allVerbs = StandardCreateVerbs
                .Union(LegacyCreateVerbs)
                .Union(StandardReadVerbs)
                .Union(LegacyReadVerbs)
                .Union(StandardUpdateVerbs)
                .Union(LegacyUpdateVerbs)
                .Union(StandardDeleteVerbs)
                .Union(LegacyDeleteVerbs);

            return allVerbs.Contains(verb);
        }
    }

    public class FeatureAccessHelper : AccessHelper<FeatureAccess>
    {
        public FeatureAccessHelper(FeatureAccess value) : base(value) { }

        protected override FeatureAccess EmptyValue => FeatureAccess.Unspecified;

        protected override Dictionary<FeatureAccess, string> Abbreviations => new Dictionary<FeatureAccess, string>()
        {
            [FeatureAccess.Trial] = "t",
            [FeatureAccess.Use] = "u"
        };

        public bool Trial => HasFlag(FeatureAccess.Trial);

        public bool Use => HasFlag(FeatureAccess.Use);
    }

    public class AccessSet
    {
        private static readonly Regex AccessPattern = new Regex(
            @"^(?:(authority|data|feature):)?([a-zA-Z,*]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AuthorityAccess Authority { get; set; }

        public DataAccess Data { get; set; }

        public FeatureAccess Feature { get; set; }

        public void Add(string access)
        {
            if (string.IsNullOrWhiteSpace(access))
                return;

            // Split on spaces to handle multiple access type segments
            var segments = access.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                var match = AccessPattern.Match(segment);
                if (!match.Success)
                    continue;

                var flags = match.Groups[2].Value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // If no prefix specified, add to all access types
                if (string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    Feature = new FeatureAccessHelper(Feature).Add(flags);
                    Data = new DataAccessHelper(Data).Add(flags);
                    Authority = new AuthorityAccessHelper(Authority).Add(flags);
                    continue;
                }

                var prefix = match.Groups[1].Value.ToLowerInvariant();
                switch (prefix)
                {
                    case "authority":
                        Authority = new AuthorityAccessHelper(Authority).Add(flags);
                        break;

                    case "data":
                        Data = new DataAccessHelper(Data).Add(flags);
                        break;

                    case "feature":
                        Feature = new FeatureAccessHelper(Feature).Add(flags);
                        break;
                }
            }
        }

        public void Add(AccessSet access)
        {
            // If no access control rules are explicitly specified, then assume a visible usable feature by default.

            if (!access.HasAny())
                access.Feature = FeatureAccess.Trial | FeatureAccess.Use;

            Feature = new FeatureAccessHelper(Feature).Add(access.Feature);

            Data = new DataAccessHelper(Data).Add(access.Data);

            Authority = new AuthorityAccessHelper(Authority).Add(access.Authority);
        }

        public bool HasAny() =>
               Authority != AuthorityAccess.Unspecified
            || Data != DataAccess.Unspecified
            || Feature != FeatureAccess.Unspecified;

        public bool Has(AuthorityAccess access) => (Authority & access) != 0;

        public bool Has(DataAccess access) => (Data & access) != 0;

        public bool Has(FeatureAccess access) => (Feature & access) != 0;

        public override string ToString()
        {
            var parts = new List<string>(4);

            if (Authority > 0)
                parts.Add("authority:" + FormatFlags(Authority));

            if (Data > 0)
                parts.Add("data:" + FormatFlags(Data));

            if (Feature > 0)
                parts.Add("feature:" + FormatFlags(Feature));

            return string.Join(" ", parts);
        }

        private string FormatFlags<T>(T flags) where T : Enum
        {
            return flags.ToString().ToLowerInvariant().Replace(" ", "");
        }
    }

    public class AccessControl
    {
        public AccessSet Granted { get; set; }

        public AccessSet Denied { get; set; }

        public AccessControl()
        {
            Granted = new AccessSet();

            Denied = new AccessSet();
        }

        public void Add(AccessControl access)
        {
            Granted.Add(access.Granted);

            Denied.Add(access.Denied);
        }

        public void Add(AccessSet access, AccessOperation operation)
        {
            if (operation == AccessOperation.Grant)
                Granted.Add(access);

            if (operation == AccessOperation.Deny)
                Denied.Add(access);
        }

        public void Add(string access)
        {
            const string pattern = @"^(allow|deny)(?::(.+))?$";

            var match = new Regex(pattern, RegexOptions.IgnoreCase).Match(access);

            if (!match.Success)
                return;

            var allow = StringHelper.Equals(match.Groups[1].Value, "allow");

            var value = match.Groups.Count == 3 ? match.Groups[2].Value : "";

            if (string.IsNullOrEmpty(value))
                value = "feature:trial,use";

            if (allow)
                Granted.Add(value);
            else
                Denied.Add(value);
        }

        public void Deny(FeatureAccess access)
            => Denied.Feature = new FeatureAccessHelper(Denied.Feature).Add(access);

        public void Grant(DataAccess access)
            => Granted.Data = new DataAccessHelper(Granted.Data).Add(access);

        public void Deny(DataAccess access)
            => Denied.Data = new DataAccessHelper(Denied.Data).Add(access);

        public void Grant(FeatureAccess access)
            => Granted.Feature = new FeatureAccessHelper(Granted.Feature).Add(access);

        public bool IsAllowed()
            => Granted.HasAny();

        public bool IsAllowed(AuthorityAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsAllowed(DataAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsAllowed(FeatureAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsDenied()
            => Denied.HasAny();

        public bool IsDenied(AuthorityAccess access)
            => Denied.Authority.HasFlag(access);

        public bool IsDenied(DataAccess access)
            => Denied.Data.HasFlag(access);

        public bool IsDenied(FeatureAccess access)
            => Denied.Feature.HasFlag(access);

        public bool IsGranted(FeatureAccess access)
            => Granted.Feature.HasFlag(access);

        public bool IsGranted(AuthorityAccess access)
            => Granted.Authority.HasFlag(access);

        public bool IsGranted(DataAccess access)
            => Granted.Data.HasFlag(access);

        public bool ShouldSerializeGranted() => Granted?.HasAny() == true;

        public bool ShouldSerializeDenied() => Denied?.HasAny() == true;
    }
}