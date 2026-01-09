using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupIndustry : Command
    {
        public string Industry { get; }
        public string IndustryComment { get; }

        public ChangeGroupIndustry(Guid group, string industry, string industryComment)
        {
            AggregateIdentifier = group;
            Industry = industry;
            IndustryComment = industryComment;
        }
    }
}
