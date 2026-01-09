using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteTypeChanged : Change
    {
        public string Type { get; set; }

        public SiteTypeChanged(string type)
        {
            Type = type;
        }
    }
}
