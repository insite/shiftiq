using System;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.People
{
    public partial class Archive : AdminBasePage, IHasParentLinkParameters
    {
        private QPerson _person = null;

        private bool _isPersonLoaded = false;

        private QPerson Person
        {
            get
            {
                if (!_isPersonLoaded)
                {
                    _person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Key, x => x.User);
                    _isPersonLoaded = true;
                }

                return _person;
            }
        }

        private Guid? UserIdentifier
            => Guid.TryParse(Request["user"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeactivateContact.AutoPostBack = true;
            DeactivateContact.CheckedChanged += (x, y) =>
            {
                if (DeactivateContact.Checked)
                    RevokeAccess.Checked = true;
            };

            ArchiveButton.Click += ArchiveButton_Click;
            UnarchiveButton.Click += UnarchiveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Person == null || Person.User.AccountCloaked.HasValue && !User.IsCloaked)
                RedirectToSearch();

            BindModelToControls();

            PageHelper.AutoBindHeader(this, null, Person.User.FullName);
        }

        protected void BindModelToControls()
        {
            ArchiveOptions.Visible = ArchiveConfirm.Visible = ArchiveButton.Visible = !Person.IsArchived;
            UnarchiveConfirm.Visible = UnarchiveButton.Visible = Person.IsArchived;

            PersonDetail.BindPerson(Person, User.TimeZone);

            RemoveFromOrganizations.Visible = ServiceLocator.Partition.IsE03();

            CancelButton.NavigateUrl = $"/ui/admin/contacts/people/edit?contact={UserIdentifier}";
        }

        private void ArchiveButton_Click(object sender, EventArgs e)
        {
            if (Person != null)
            {
                ArchiveConfirmed(Person.PersonIdentifier, Person.UserIdentifier, Person.OrganizationIdentifier, Person.EmployerGroupIdentifier);
            }

            RedirectToSearch();
        }

        private void UnarchiveButton_Click(object sender, EventArgs e)
        {
            if (Person != null)
            {
                UnarchiveConfirmed(Person.PersonIdentifier, Person.UserIdentifier, Person.OrganizationIdentifier);
            }

            RedirectToSearch();
        }

        public void ArchiveConfirmed(Guid personId, Guid userId, Guid organizationId, Guid? employerId)
        {
            var helper = new ArchiveHelper(
                User.FullName,
                DeactivateContact.Checked,
                RemoveFromGroups.Checked,
                DisableNotifications.Checked,
                RevokeAccess.Checked,
                RemoveFromOrganizations.Checked && RemoveFromOrganizations.Visible
                );

            helper.Archive(personId, userId, organizationId, employerId);

            RedirectToEdit();
        }

        public void UnarchiveConfirmed(Guid personId, Guid userId, Guid organizationId)
        {
            var helper = new ArchiveHelper(
                User.FullName
                );

            helper.Unarchive(personId);

            RedirectToEdit();
        }

        private void RedirectToEdit()
            => HttpResponseHelper.Redirect(CancelButton.NavigateUrl);

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect("/ui/admin/contacts/people/search");

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"contact={UserIdentifier}" : null;
    }
}