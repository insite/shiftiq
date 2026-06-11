using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationLocalizationModified : Change
    {
        public string[] Languages { get; set; }
        public string TimeZone { get; set; }

        public OrganizationLocalizationModified(string[] languages, string timeZone)
        {
            Languages = languages;
            TimeZone = timeZone;
        }
    }
}
