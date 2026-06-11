using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class PlatformCustomized : Change
    {
        public PlatformCustomization PlatformCustomization { get; set; }

        public PlatformCustomized(PlatformCustomization customization)
        {
            PlatformCustomization = customization;
        }
    }
}