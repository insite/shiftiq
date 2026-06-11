using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupIndustryChanged : Change
    {
        public string Industry { get; }
        public string IndustryComment { get; }

        public GroupIndustryChanged(string industry, string industryComment)
        {
            Industry = industry;
            IndustryComment = industryComment;
        }
    }
}
