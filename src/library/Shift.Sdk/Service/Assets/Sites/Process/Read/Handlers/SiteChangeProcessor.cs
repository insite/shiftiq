using Shift.Common.Timeline.Changes;

namespace InSite.Application.Sites.Read
{
    public class SiteChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly ISiteSearch _sites;

        public SiteChangeProcessor(ICommander commander, IChangeQueue publisher, ISiteSearch sites)
        {
            _commander = commander;

            _sites = sites;
        }
    }
}
