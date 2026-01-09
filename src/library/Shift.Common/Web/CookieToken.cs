using System;
using System.Globalization;

using Shift.Common;

namespace Shift.Contract
{
    [Serializable]
    public class CookieToken : ISystemRoles
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private static TimeSpan Lifetime = TimeSpan.Zero;

        private static readonly DateTimeStyles Styles = DateTimeStyles.None;

        private const int DefaultLifetimeInMinutes = 60;

        private const int EventDurationInMinutes = 15;

        private DateTimeOffset _created;

        private DateTimeOffset _modified;

        public string AuthenticationSource { get; set; }

        public string Created
        {
            get => ConvertDateToString(_created);
            set => _created = ConvertStringToDate(value);
        }

        public string CurrentBrowser { get; set; }

        public string CurrentBrowserVersion { get; set; }

        public string CurrentOrganization { get; set; }

        public Guid ID { get; set; }

        public string ImpersonatorOrganization { get; set; }

        public string ImpersonatorUser { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsAuthenticated => UserIdentifier.HasValue && UserIdentifier != Guid.Empty;

        public bool IsDeveloper { get; set; }

        public bool IsOperator { get; set; }
        public bool IsLearner { get; set; }

        public string Language { get; set; }

        public string Modified
        {
            get => ConvertDateToString(_modified);
            set => _modified = ConvertStringToDate(value);
        }

        public string OrganizationCode { get; set; }

        public Guid? OrganizationIdentifier { get; set; }

        public string Session { get; set; }

        public string TimeZoneId { get; set; }

        public string UserEmail { get; set; }

        public Guid? UserIdentifier { get; set; }

        public string[] UserRoles { get; set; }

        public string ValidationKey { get; set; }

        public string Environment { get; set; }

        public CookieToken()
        {
            ID = Guid.NewGuid();

            Created = DateTimeOffset.UtcNow.ToString("O");

            Lifetime = TimeSpan.FromMinutes(DefaultLifetimeInMinutes);

            Modified = Created;
        }

        public bool IsActive()
            => !IsExpired() && _modified.AddMinutes(EventDurationInMinutes) > DateTimeOffset.UtcNow;

        public bool IsExpired()
            => _modified.Add(Lifetime) <= DateTimeOffset.UtcNow;

        public bool TargetsEnvironment(string environment)
            => StringHelper.Equals(environment, Environment);

        public DateTimeOffset ParseCreated()
            => _created;

        private string ConvertDateToString(DateTimeOffset iso)
            => iso.ToString("O");

        private DateTimeOffset ConvertStringToDate(string iso)
        {
            if (DateTimeOffset.TryParseExact(iso, "O", Culture, Styles, out DateTimeOffset parsed))
                return parsed;

            throw new FormatException($"Invalid ISO 8601 date format: '{iso}'. Expected format: 'O'.");
        }
    }
}