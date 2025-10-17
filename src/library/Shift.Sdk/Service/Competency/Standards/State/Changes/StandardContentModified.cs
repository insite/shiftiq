using Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Standards
{
    public class StandardContentModified : Change
    {
        public ContentContainer Content { get; }

        public StandardContentModified(ContentContainer content)
        {
            Content = content;
        }
    }
}
