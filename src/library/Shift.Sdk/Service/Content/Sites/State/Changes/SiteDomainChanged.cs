using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteDomainChanged : Change
    {
        public string Domain { get; set; }
        public SiteDomainChanged(string domain)
        {
            Domain = domain;
        }
    }
}
