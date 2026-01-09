using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class BankContentChanged : Change
    {
        public ContentExamBank Content { get; set; }

        public BankContentChanged(ContentExamBank content)
        {
            Content = content;
        }
    }
}
