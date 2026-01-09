using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class TypeChanged : Change
    {
        public string Type { get; set; }
        public TypeChanged(string type)
        {
            Type = type;
        }
    }
}
