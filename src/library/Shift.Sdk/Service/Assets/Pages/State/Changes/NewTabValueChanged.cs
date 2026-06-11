using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class NewTabValueChanged : Change
    {
        public bool IsNewTab { get; set; }
        public NewTabValueChanged(bool isNewTab)
        {
            IsNewTab = isNewTab;
        }
    }
}
