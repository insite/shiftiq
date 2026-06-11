using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationLogbookSettingsModified : Change
    {
        public LogbookSettings Logbooks { get; set; }

        public OrganizationLogbookSettingsModified(LogbookSettings logbooks)
        {
            Logbooks = logbooks;
        }
    }
}
