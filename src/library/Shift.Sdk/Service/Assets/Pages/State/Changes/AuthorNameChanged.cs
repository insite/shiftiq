using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class AuthorNameChanged : Change
    {
        public string AuthorName { get; set; }
        public AuthorNameChanged(string authorName)
        {
            AuthorName = authorName;
        }
    }
}
