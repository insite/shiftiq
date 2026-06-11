using Shift.Common.Timeline.Changes;

namespace InSite.Application.Sites.Read
{
    public class PageChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly IPageSearch _pages;

        public PageChangeProcessor(ICommander commander, IChangeQueue publisher, IPageSearch pages)
        {
            _commander = commander;

            _pages = pages;
        }
    }
}
