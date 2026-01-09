using System;
using System.ComponentModel;

using InSite.Application.Contacts.Write;
using InSite.Application.People.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Accounts.Users.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<UserFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveBulkButton.Click += SaveBulkButton_Click;
        }

        private void SaveBulkButton_Click(object sender, EventArgs e)
        {
            bool isGranted;
            if (UserAccess.SelectedValue == "Access Granted")
                isGranted = true;
            else if (UserAccess.SelectedValue == "Access Revoked")
                isGranted = false;
            else
            {
                BulkUpdateStatus.AddMessage(AlertType.Error, "No bulk action selected");
                return;
            }

            var count = isGranted
                ? GrantAccess()
                : RevokeAccess();

            UserAccess.SelectedValue = null;

            var action = isGranted ? "Access <b>granted</b> to " : "Access <b>revoked</b> from ";

            BulkUpdateStatus.AddMessage(AlertType.Success, action + Shift.Common.Humanizer.ToQuantity(count, "user account") + " in current organization.");

            if (count > 0)
                Search(Filter);
        }

        private int GrantAccess()
        {
            var persons = UserSearch.SelectPersons(Filter, Organization.Identifier);
            var users = UserSearch.Select(Filter);
            var count = 0;

            foreach (var user in users)
            {
                var person = persons.Find(x => x.UserIdentifier == user.UserIdentifier);
                if (person != null && person.UserAccessGranted.HasValue && person.AccessRevoked == null)
                    continue;

                Guid personId;

                if (person == null)
                {
                    var newPerson = UserFactory.CreatePerson(Organization.Identifier);
                    newPerson.PersonIdentifier = UniqueIdentifier.Create();
                    newPerson.UserIdentifier = user.UserIdentifier;

                    var commands = PersonCommandCreator.Create(null, newPerson);
                    ServiceLocator.SendCommands(commands);

                    personId = newPerson.PersonIdentifier;
                }
                else
                    personId = person.PersonIdentifier;

                ServiceLocator.SendCommand(new GrantPersonAccess(personId, DateTimeOffset.UtcNow, User.FullName));

                count++;
            }

            return count;
        }

        private int RevokeAccess()
        {
            var persons = UserSearch.SelectPersons(Filter, Organization.Identifier);
            var count = 0;

            foreach (var person in persons)
            {
                if (person.AccessRevoked.HasValue && person.UserAccessGranted == null)
                    continue;

                ServiceLocator.SendCommand(new RevokePersonAccess(person.PersonIdentifier, DateTimeOffset.Now, User.FullName));

                count++;
            }

            return count;
        }

        #region Searching

        protected override int SelectCount(UserFilter filter)
        {
            var count = UserSearch.Count(filter);

            BulkUpdateButtonPanel.Visible = count > 0;

            return count;
        }

        protected override IListSource SelectData(UserFilter filter)
        {
            return UserSearch.SelectSearchResults(filter);
        }

        #endregion

        #region Helper methods

        protected string Format(DateTime? value)
        {
            return !value.HasValue
                ? null
                : TimeZones.Format((DateTimeOffset)value, User.TimeZone);
        }

        #endregion
    }
}