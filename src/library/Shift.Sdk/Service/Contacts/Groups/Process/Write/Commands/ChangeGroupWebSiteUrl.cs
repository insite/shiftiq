using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupWebSiteUrl : Command
    {
        public string Url { get; }

        public ChangeGroupWebSiteUrl(Guid group, string url)
        {
            AggregateIdentifier = group;
            Url = url.NullIfEmpty();
        }
    }

    public class ChangeGroupSocialMediaUrl : Command
    {
        public string Type { get; }
        public string Url { get; }

        public ChangeGroupSocialMediaUrl(Guid group, string type, string url)
        {
            AggregateIdentifier = group;
            Type = type;
            Url = url.NullIfEmpty();
        }
    }
}
