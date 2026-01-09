using System;
using System.Linq;
using System.Text;

using InSite.Admin.Accounts.Organizations.Controls;
using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class ApproveAccess : AdminBasePage, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/contacts/people/edit";
        private const string SearchUrl = "/ui/admin/contacts/people/search";

        private Guid? UserIdentifier
            => Guid.TryParse(Request["user"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AttachGroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += (s, a) => OnEmployerChanged();

            RegisterWithGroup.AutoPostBack = true;
            RegisterWithGroup.SelectedIndexChanged += (s, a) => OnRegisterWithGroupChanged();

            GrantButton.Click += GrantButton_Click;
            DenyButton.Click += DenyButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                RedirectToSearch();

            var person = UserIdentifier.HasValue
                ? ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier, x => x.User.Memberships)
                : null;

            if (person == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(Page, qualifier: $"{person.User.FullName} <span class='form-text'>{person.User.Email}</span>");

            ScreenStatus.AddMessage(AlertType.Information, "Please review this user's request for access to your organization's account.");

            InSite.UI.Admin.Contacts.People.Controls.PersonInfo.BindPerson(
                person, person?.User, User.TimeZone,
                PersonName, PersonLink, PersonEmail, PersonCode, PersonBirthdate);

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;
            EmployerGroupIdentifier.Value = person.EmployerGroupIdentifier;
            OnEmployerChanged();

            var organization = OrganizationSearch.Select(Organization.Identifier);
            OrganizationInfo.BindOrganization(organization);

            if (person != null)
            {
                IsAdministrator.Checked = person.IsAdministrator;
                IsLearner.Checked = person.IsLearner;
            }

            ConfirmPersonName.Text = person.User.FullName;
            ConfirmPersonNameFirst.Text = person.User.FirstName;
            OrganizationName.Text = Organization.CompanyName;

            RegisterWithGroup.SelectedValue = RegisterWithGroup.Items[0].Value;

            OnRegisterWithGroupChanged();

            AttachGroupIdentifier.Filter.ExcludeGroupIdentifiers = person.User.Memberships.Select(x => x.GroupIdentifier).ToArray();
        }

        private void OnEmployerChanged()
        {
            var employerId = EmployerGroupIdentifier.Value;
            var employer = employerId.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(employerId.Value)
                : null;

            var status = employer != null
                ? TCollectionItemCache.GetName(employer.GroupStatusItemIdentifier)
                : null;

            EmployerBadge.Visible = status.IsNotEmpty();
            EmployerBadge.InnerText = status;
        }

        private void OnRegisterWithGroupChanged()
        {
            AttachGroupIdentifier.Value = null;
            CreateGroupType.Value = null;
            CreateGroupName.Text = null;

            AttachGroupContainer.Visible = RegisterWithGroup.SelectedValue == "Attach";
            CreateGroupContainer.Visible = RegisterWithGroup.SelectedValue == "Create";
        }

        private void GrantButton_Click(object sender, EventArgs e)
        {
            var user = UserSearch.Select(UserIdentifier.Value);

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier);
            if (person == null)
            {
                person = Web.Data.UserFactory.CreatePerson(Organization.Identifier, User.Identifier);
                person.UserIdentifier = UserIdentifier.Value;
            }

            person.EmployerGroupIdentifier = EmployerGroupIdentifier.Value;

            person.UserAccessGranted = DateTimeOffset.UtcNow;
            person.UserAccessGrantedBy = User.FullName;
            person.AccessRevoked = null;
            person.AccessRevokedBy = null;

            person.IsAdministrator = IsAdministrator.Checked;
            person.IsLearner = IsLearner.Checked;

            if (person.PersonIdentifier == Guid.Empty)
                PersonStore.Insert(person);
            else
                PersonStore.Update(person);

            Guid? groupId = null;

            if (RegisterWithGroup.SelectedValue == "Attach")
            {
                groupId = AttachGroupIdentifier.Value.HasValue && MembershipPermissionHelper.CanModifyMembership(AttachGroupIdentifier.Value.Value)
                    ? AttachGroupIdentifier.Value.Value
                    : (Guid?)null;
            }
            else if (RegisterWithGroup.SelectedValue == "Create")
            {
                groupId = UniqueIdentifier.Create();

                ServiceLocator.SendCommand(new CreateGroup(
                    groupId.Value,
                    Organization.Identifier,
                    CreateGroupType.Value,
                    CreateGroupName.Text));
            }

            if (groupId.HasValue)
            {
                MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = groupId.Value,
                    UserIdentifier = person.UserIdentifier,
                    Assigned = DateTimeOffset.UtcNow
                });
            }

            ServiceLocator.AlertMailer.Send(Organization.Identifier, User.UserIdentifier, UserIdentifier, new AlertApplicationAccessGranted
            {
                ApproverEmail = User.Email,
                ApproverName = User.FullName,
                Organization = Organization.LegalName,
                UserAccess = WriteUserAccessList(),
                UserFirstName = user.FirstName
            });

            HttpResponseHelper.Redirect(EditUrl + $"?contact={user.UserIdentifier}");
        }

        private void DenyButton_Click(object sender, EventArgs e)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier);
            if (person == null)
                return;

            person.UserAccessGranted = null;
            person.UserAccessGrantedBy = null;
            person.AccessRevoked = DateTimeOffset.UtcNow;
            person.AccessRevokedBy = User.FullName;

            PersonStore.Update(person);

            var user = UserSearch.Select(UserIdentifier.Value);

            HttpResponseHelper.Redirect(EditUrl + $"?contact={user.UserIdentifier}");
        }

        private string WriteUserAccessList()
        {
            var sb = new StringBuilder();
            if (IsAdministrator.Checked)
                sb.AppendLine("- Access to Admin Tools (Administrator)");
            if (IsLearner.Checked)
                sb.AppendLine("- Include in Reports (Member)");
            return sb.ToString();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect(SearchUrl);

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"contact={UserIdentifier}" : null;
    }
}