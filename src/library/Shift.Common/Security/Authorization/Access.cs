using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public enum AccessOperation { Grant, Deny }

    public enum AccessType { Switch, Operation, Http, Authority }

    [Flags]
    public enum SwitchAccess
    {
        Off = 0,
        On = 1
    }

    [Flags]
    public enum OperationAccess
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        Create = 1 << 2,
        Delete = 1 << 3,
        Administrate = 1 << 4,
        Configure = 1 << 5
    }

    [Flags]
    public enum HttpAccess
    {
        None = 0,
        Head = 1 << 0,
        Get = 1 << 1,
        Put = 1 << 2,
        Post = 1 << 3,
        Delete = 1 << 4
    }

    [Flags]
    public enum AuthorityAccess
    {
        None = 0,               // Nobody
        Visitor = 1 << 0,       // Everyone, including unauthenticated users
        Member = 1 << 1,        // Everyone, excluding unauthenticated users
        Trainee = 1 << 2,       // Authenticated newcomers who are onboarding with orientation
        Learner = 1 << 3,       // Authenticated students, employees, contractors, etc.
        Instructor = 1 << 4,    // Authenticated teachers, content authors, etc.
        Validator = 1 << 5,     // Authenticated subject matter experts
        Supervisor = 1 << 6,    // Authenticated low- and medium-level management
        Manager = 1 << 7,       // Authenticated high-level management
        Administrator = 1 << 8, // Authenticated field, office, and system administrators
        Developer = 1 << 9,     // Authenticated programmers and infrastructure administrators (dev/ops)
        Operator = 1 << 10      // Authenticated root-level administrators with full, unrestricted access
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
            var allFlags = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(v => Convert.ToInt32(v) > 0)
                .Aggregate(0, (acc, v) => acc | Convert.ToInt32(v));

            return (T)Enum.ToObject(typeof(T), allFlags);
        }
    }

    public class SwitchAccessHelper : AccessHelper<SwitchAccess>
    {
        public SwitchAccessHelper(SwitchAccess value) : base(value) { }

        protected override SwitchAccess EmptyValue => SwitchAccess.Off;

        protected override Dictionary<SwitchAccess, string> Abbreviations => new Dictionary<SwitchAccess, string>()
        {
            [SwitchAccess.On] = "+"
        };

        public bool On => HasFlag(SwitchAccess.On);
    }

    public class OperationAccessHelper : AccessHelper<OperationAccess>
    {
        public OperationAccessHelper(OperationAccess value) : base(value) { }

        protected override OperationAccess EmptyValue => OperationAccess.None;

        protected override Dictionary<OperationAccess, string> Abbreviations => new Dictionary<OperationAccess, string>()
        {
            [OperationAccess.Read] = "r",
            [OperationAccess.Write] = "w",
            [OperationAccess.Create] = "c",
            [OperationAccess.Delete] = "d",
            [OperationAccess.Administrate] = "a",
            [OperationAccess.Configure] = "f"
        };

        public bool Read => HasFlag(OperationAccess.Read);
        public bool Write => HasFlag(OperationAccess.Write);
        public bool Create => HasFlag(OperationAccess.Create);
        public bool Delete => HasFlag(OperationAccess.Delete);
        public bool Administrate => HasFlag(OperationAccess.Administrate);
        public bool Configure => HasFlag(OperationAccess.Configure);

        // An operation may be a query (to read something) or a command (to write or delete something). It is important
        // to move toward a consistent naming convention for these operations. The following properties are mainly for
        // documentation purposes, but may be useful in writing code also.

        public string[] StandardReadVerbs = new string[] { "Assert", "Collect", "Count", "Download", "Retrieve", "Search", "Export" };
        public string[] LegacyReadVerbs = new string[] { "Read", "Outline", "View" };

        public string[] StandardWriteVerbs = new string[] { "Create", "Import", "Modify" };
        public string[] LegacyWriteVerbs = new string[] { "Edit", "Insert", "Update", "Upload" };

        public string[] StandardDeleteVerbs = new string[] { "Delete", "Purge" };
        public string[] LegacyDeleteVerbs = new string[] { "Remove", "Void" };
    }

    public class HttpAccessHelper : AccessHelper<HttpAccess>
    {
        public HttpAccessHelper(HttpAccess value) : base(value) { }

        protected override HttpAccess EmptyValue => HttpAccess.None;

        protected override Dictionary<HttpAccess, string> Abbreviations => new Dictionary<HttpAccess, string>()
        {
            [HttpAccess.Head] = "h",
            [HttpAccess.Get] = "g",
            [HttpAccess.Put] = "u",
            [HttpAccess.Post] = "p",
            [HttpAccess.Delete] = "d"
        };

        public bool Head => HasFlag(HttpAccess.Head);
        public bool Get => HasFlag(HttpAccess.Get);
        public bool Put => HasFlag(HttpAccess.Put);
        public bool Post => HasFlag(HttpAccess.Post);
        public bool Delete => HasFlag(HttpAccess.Delete);
    }

    public class AuthorityAccessHelper : AccessHelper<AuthorityAccess>
    {
        public AuthorityAccessHelper(AuthorityAccess value) : base(value) { }

        protected override AuthorityAccess EmptyValue => AuthorityAccess.None;

        protected override Dictionary<AuthorityAccess, string> Abbreviations => new Dictionary<AuthorityAccess, string>()
        {
            [AuthorityAccess.Visitor] = "x",
            [AuthorityAccess.Member] = "e",
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

        public bool Visitor => HasFlag(AuthorityAccess.Visitor);
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

    public class Access
    {
        private static readonly Regex FormalPattern = new Regex(
            @"^(?:(switch|operation|http|authority):)?([a-zA-Z,*]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SwitchAccess Switch { get; set; }
        public OperationAccess Operation { get; set; }
        public HttpAccess Http { get; set; }
        public AuthorityAccess Authority { get; set; }

        public void Add(string access)
        {
            var match = FormalPattern.Match(access);

            if (!match.Success)
                return;

            var flags = match.Groups[2].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // If no prefix specified then add all access types.

            if (string.IsNullOrEmpty(match.Groups[1].Value))
            {
                Switch = new SwitchAccessHelper(Switch).Add(flags);
                Operation = new OperationAccessHelper(Operation).Add(flags);
                Http = new HttpAccessHelper(Http).Add(flags);
                Authority = new AuthorityAccessHelper(Authority).Add(flags);
                return;
            }

            var accessType = (AccessType)Enum.Parse(typeof(AccessType), match.Groups[1].Value, ignoreCase: true);

            switch (accessType)
            {
                case AccessType.Switch:
                    Switch = new SwitchAccessHelper(Switch).Add(flags);
                    break;
                case AccessType.Operation:
                    Operation = new OperationAccessHelper(Operation).Add(flags);
                    break;
                case AccessType.Http:
                    Http = new HttpAccessHelper(Http).Add(flags);
                    break;
                case AccessType.Authority:
                    Authority = new AuthorityAccessHelper(Authority).Add(flags);
                    break;
            }
        }

        public void Add(Access access)
        {
            Switch = new SwitchAccessHelper(Switch).Add(access.Switch);
            Operation = new OperationAccessHelper(Operation).Add(access.Operation);
            Http = new HttpAccessHelper(Http).Add(access.Http);
            Authority = new AuthorityAccessHelper(Authority).Add(access.Authority);
        }

        public bool HasAny() =>
            Switch != SwitchAccess.Off ||
            Operation != OperationAccess.None ||
            Http != HttpAccess.None ||
            Authority != AuthorityAccess.None;

        public bool Has(SwitchAccess access) => (Switch & access) != 0;
        public bool Has(OperationAccess access) => (Operation & access) != 0;
        public bool Has(HttpAccess access) => (Http & access) != 0;
        public bool Has(AuthorityAccess access) => (Authority & access) != 0;
    }

    public class AccessControl
    {
        public Access Granted { get; set; }

        public Access Denied { get; set; }

        public AccessControl()
        {
            Granted = new Access();

            Denied = new Access();
        }

        public void Add(AccessControl access)
        {
            Granted.Add(access.Granted);

            Denied.Add(access.Denied);
        }

        public void Add(string access)
        {
            const string pattern = @"^(allow|deny)(?::(.+))?$";

            var match = new Regex(pattern, RegexOptions.IgnoreCase).Match(access);

            if (!match.Success)
                return;

            var allow = StringHelper.Equals(match.Groups[1].Value, "allow");

            var value = match.Groups.Count == 3 ? match.Groups[2].Value : "on";

            if (string.IsNullOrEmpty(value))
                value = "on";

            if (allow)
                Granted.Add(value);
            else
                Denied.Add(value);
        }

        public void Grant(SwitchAccess access)
            => Granted.Switch = new SwitchAccessHelper(Granted.Switch).Add(access);

        public void Deny(SwitchAccess access)
            => Denied.Switch = new SwitchAccessHelper(Denied.Switch).Add(access);

        public void Grant(OperationAccess access)
            => Granted.Operation = new OperationAccessHelper(Granted.Operation).Add(access);

        public void Deny(OperationAccess access)
            => Denied.Operation = new OperationAccessHelper(Denied.Operation).Add(access);

        public bool IsAllowed()
            => Granted.HasAny();

        public bool IsAllowed(SwitchAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsAllowed(OperationAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsAllowed(HttpAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsAllowed(AuthorityAccess access)
            => !IsDenied(access) && IsGranted(access);

        public bool IsGranted(SwitchAccess access)
            => Granted.Switch.HasFlag(access);

        public bool IsDenied()
            => Denied.HasAny();

        public bool IsDenied(SwitchAccess access)
            => Denied.Switch.HasFlag(access);

        public bool IsGranted(OperationAccess access)
            => Granted.Operation.HasFlag(access);

        public bool IsDenied(OperationAccess access)
            => Denied.Operation.HasFlag(access);

        public bool IsGranted(HttpAccess access)
            => Granted.Http.HasFlag(access);

        public bool IsDenied(HttpAccess access)
            => Denied.Http.HasFlag(access);

        public bool IsGranted(AuthorityAccess access)
            => Granted.Authority.HasFlag(access);

        public bool IsDenied(AuthorityAccess access)
            => Denied.Authority.HasFlag(access);
    }
}