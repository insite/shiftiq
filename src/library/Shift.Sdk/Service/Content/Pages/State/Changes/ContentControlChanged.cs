using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Sites.Pages
{
    public class ContentControlChanged : Change
    {
        public string ContentControl { get; set; }

        public ContentControlChanged(string contentControl)
        {
            if (contentControl.IsEmpty())
                ContentControl = null;
            else if (contentControl.StartsWith("InSite.UI.Layouts.Bootstrap5.Controls."))
                ContentControl = "InSite.UI.Layout.Common.Controls." + contentControl.Substring(38);
            else if (contentControl.StartsWith("InSite.UI.Layout.Bootstrap5.Controls."))
                ContentControl = "InSite.UI.Layout.Common.Controls." + contentControl.Substring(37);
            else
                ContentControl = contentControl;
        }
    }
}
