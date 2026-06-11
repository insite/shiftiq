using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class HookChanged : Change
    {
        public string Hook { get; set; }

        public HookChanged(string hook)
        {
            Hook = hook;
        }
    }
}
