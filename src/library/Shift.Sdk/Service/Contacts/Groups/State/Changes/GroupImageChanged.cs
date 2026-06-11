using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupImageChanged : Change
    {
        public string Image { get; }

        public GroupImageChanged(string image)
        {
            Image = image;
        }
    }
}
