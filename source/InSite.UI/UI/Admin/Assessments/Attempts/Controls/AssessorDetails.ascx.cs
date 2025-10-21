using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class AssessorDetails : BaseUserControl
    {
        public string Title
        {
            get => ControlTitle.InnerText;
            set => ControlTitle.InnerText = value;
        }

        public void Bind(Guid userId)
        {
            var user = UserSearch.Select(userId);

            Bind(user);
        }

        public void Bind(User user)
        {
            var hasPerson = false;

            if (user == null)
            {
                user = new User
                {
                    UserIdentifier = UserIdentifiers.Someone,
                    FullName = "Someone"
                };
            }
            else
                hasPerson = ServiceLocator.PersonSearch.IsPersonExist(user.UserIdentifier, Organization.Identifier);

            UserName.Text = user.FullName;
            UserName.Visible = !hasPerson;

            UserLink.InnerText = user.FullName;
            UserLink.HRef = $"/ui/admin/contacts/people/edit?contact={user.UserIdentifier}";
            UserLink.Visible = hasPerson;

            UserEmail.Text = $"<a href='mailto:{user.Email}'>{user.Email}</a>";
            UserEmail.Visible = user.Email.IsNotEmpty();
        }
    }
}