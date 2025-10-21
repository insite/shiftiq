using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;
using InSite.Persistence;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class UserOrganizationList : BaseUserControl
    {
        private Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        public int ItemsCount
        {
            get => (int)(ViewState[nameof(ItemsCount)] ?? 0);
            set => ViewState[nameof(ItemsCount)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrganizationCombo.AutoPostBack = true;
            OrganizationCombo.ValueChanged += OrganizationCombo_ValueChanged;

            Repeater.ItemCommand += Repeater_ItemCommand;
            Repeater.ItemCreated += Repeater_ItemCreated;
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var administrator = (ICheckBoxControl)e.Item.FindControl("PersonIsAdministrator");
            var developer = (ICheckBoxControl)e.Item.FindControl("PersonIsDeveloper");
            var @operator = (ICheckBoxControl)e.Item.FindControl("PersonIsOperator");
            var learner = (ICheckBoxControl)e.Item.FindControl("PersonIsLearner");
            var access = (ICheckBoxControl)e.Item.FindControl("PersonIsGrantedAccess");

            administrator.CheckedChanged += Administrator_CheckedChanged;
            developer.CheckedChanged += Developer_CheckedChanged;
            @operator.CheckedChanged += Operator_CheckChanged;
            learner.CheckedChanged += Learner_CheckedChanged;
            access.CheckedChanged += Access_CheckedChanged;
        }

        private void Administrator_CheckedChanged(object sender, EventArgs e)
        {
            var box = (System.Web.UI.WebControls.CheckBox)sender;
            var organizationId = Guid.Parse(((ITextControl)box.Parent.FindControl("OrganizationIdentifier")).Text);
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, organizationId, x => x.User);

            ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.IsAdministrator, box.Checked));

            BindCounts(GetOrganizations());

            if (box.Checked)
            {
                var organization = OrganizationSearch.Select(organizationId);
                OrganizationAccessGrantedToAdminTools(organization, person);
            }
        }

        private void Developer_CheckedChanged(object sender, EventArgs e)
        {
            var box = (System.Web.UI.WebControls.CheckBox)sender;
            var organizationId = Guid.Parse(((ITextControl)box.Parent.FindControl("OrganizationIdentifier")).Text);

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, organizationId);

            ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.IsDeveloper, box.Checked));

            BindCounts(GetOrganizations());
        }

        private void Operator_CheckChanged(object sender, EventArgs e)
        {
            var box = (System.Web.UI.WebControls.CheckBox)sender;
            var organizationId = Guid.Parse(((ITextControl)box.Parent.FindControl("OrganizationIdentifier")).Text);
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, organizationId, x => x.User);

            ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.IsOperator, box.Checked));

            BindCounts(GetOrganizations());

            if (box.Checked)
            {
                var organization = OrganizationSearch.Select(organizationId);
                OrganizationAccessGrantedToAdminTools(organization, person);
            }
        }

        private void Learner_CheckedChanged(object sender, EventArgs e)
        {
            var box = (System.Web.UI.WebControls.CheckBox)sender;
            var organizationId = Guid.Parse(((ITextControl)box.Parent.FindControl("OrganizationIdentifier")).Text);

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, organizationId);

            ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.IsLearner, box.Checked));

            BindCounts(GetOrganizations());
        }

        private void Access_CheckedChanged(object sender, EventArgs e)
        {
            var box = (System.Web.UI.WebControls.CheckBox)sender;
            var organizationId = Guid.Parse(((ITextControl)box.Parent.FindControl("OrganizationIdentifier")).Text);
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, organizationId);

            if (box.Checked)
                ServiceLocator.SendCommand(new GrantPersonAccess(person.PersonIdentifier, DateTimeOffset.UtcNow, User.FullName));
            else
                ServiceLocator.SendCommand(new RevokePersonAccess(person.PersonIdentifier, DateTimeOffset.UtcNow, User.FullName));

            BindCounts(GetOrganizations());
        }

        private void OrganizationCombo_ValueChanged(object sender, EventArgs e)
        {
            if (OrganizationCombo.HasValue)
            {
                var organizationId = OrganizationCombo.Value.Value;

                if (!ServiceLocator.PersonSearch.IsPersonExist(UserIdentifier, organizationId))
                {
                    var person = PersonFactory.Create(UserIdentifier, organizationId, null, false, null);
                    person.EmailEnabled = true;

                    PersonStore.Insert(person);
                }

                BindCounts(GetOrganizations());

                LoadData(UserIdentifier);
            }

            OrganizationCombo.Value = null;
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
                DeletePerson(e.CommandArgument);
        }

        private void DeletePerson(object organizationId)
        {
            if (organizationId == null)
                return;

            if (!Guid.TryParse(organizationId.ToString(), out Guid id))
                return;

            var organization = OrganizationSearch.Select(id);
            if (organization == null)
                return;

            if (ServiceLocator.PersonSearch.IsPersonExist(UserIdentifier, organization.Identifier))
                PersonStore.Delete(UserIdentifier, organization.Identifier);

            LoadData(UserIdentifier);
        }

        public void LoadData(Guid user)
        {
            UserIdentifier = user;
            BindRepeater();
        }

        private void BindRepeater()
        {
            var identity = CurrentSessionState.Identity;
            string[] availableOrganizationCodes = null;

            var organizations = GetOrganizations();
            BindCounts(organizations);

            availableOrganizationCodes = identity.Organizations.Where(x => x.AccountClosed == null).Select(x => x.OrganizationCode).ToArray();

            organizations = organizations.Where(x => availableOrganizationCodes.Contains(x.OrganizationCode))
                .OrderBy(x => x.OrganizationName)
                .ToList();

            var dataCount = organizations.Count;

            Repeater.Visible = dataCount > 0;
            Repeater.DataSource = organizations;
            Repeater.DataBind();

            ItemsCount = dataCount;

            OrganizationCount.Value = dataCount.ToString();

            OrganizationCombo.Value = null;
            OrganizationCombo.Filter.IsClosed = false;

            if (availableOrganizationCodes != null)
                OrganizationCombo.Filter.IncludeOrganizationCode = availableOrganizationCodes;

            OrganizationCombo.Filter.ExcludeOrganizationCode = organizations.Select(x => x.OrganizationCode).ToArray();
            OrganizationCombo.Visible = true;
        }

        private List<PersonOrganizationListDataItem> GetOrganizations()
        {
            return ServiceLocator.PersonSearch.GetPersonsForOrganizationList(new QPersonFilter
            {
                UserIdentifier = UserIdentifier,
                OrderBy = "Organization.CompanyName"
            });
        }

        private void BindCounts(IEnumerable<PersonOrganizationListDataItem> organizations)
        {
            var adminCount = organizations.Count(x => x.PersonIsAdministrator);
            var developerCount = organizations.Count(x => x.PersonIsDeveloper);
            var operatorCount = organizations.Count(x => x.PersonIsOperator);
            var learnerCount = organizations.Count(x => x.PersonIsLearner);
            var accessCount = organizations.Count(x => x.PersonIsGrantedAccess);

            AdministratorCount.Text = $"<span class='badge bg-danger'>{adminCount}</span>";
            DeveloperCount.Text = $"<span class='badge bg-warning'>{developerCount}</span>";
            OperatorCount.Text = $"<span class='badge bg-dark'>{operatorCount}</span>";
            LearnerCount.Text = $"<span class='badge bg-info'>{learnerCount}</span>";
            AccessCount.Text = $"<span class='badge bg-success'>{accessCount}</span>";
        }

        private void OrganizationAccessGrantedToAdminTools(OrganizationState organization, QPerson person)
            => ServiceLocator.AlertMailer.Send(organization.OrganizationIdentifier, User.UserIdentifier, person.UserIdentifier, new AlertApplicationAccessGranted
            {
                ApproverEmail = User.Email,
                ApproverName = User.FullName,
                Organization = organization.LegalName,
                UserAccess = WriteUserAccessList(person),
                UserFirstName = person.User?.FirstName
            });

        private string WriteUserAccessList(QPerson person)
        {
            var sb = new StringBuilder();

            if (person.IsAdministrator)
                sb.AppendLine("- Administrator");

            if (person.IsDeveloper)
                sb.AppendLine("- Developer");

            if (person.IsLearner)
                sb.AppendLine("- Learner");

            if (person.IsOperator)
                sb.AppendLine("- Operator");

            return sb.ToString();
        }
    }
}