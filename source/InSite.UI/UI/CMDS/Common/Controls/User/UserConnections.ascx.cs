using System;
using System.Web.UI.WebControls;

using InSite.Application.Users.Write;
using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Custom.CMDS.Admin.People.Controls
{
    public partial class UserConnections : BaseUserControl
    {
        private Guid UserIdentifier
        {
            get => (Guid)(ViewState[nameof(UserIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RelationshipType.AutoPostBack = true;
            RelationshipType.SelectedIndexChanged += (s, a) => RelationshipTypeSelected();
            AddRelationship.Click += AddRelationship_Click;

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var leader = RelationshipType.Items.FindByValue("Leader");

            if (leader != null)
                leader.Enabled = !(Organization.Toolkits?.Contacts?.DisableLeaderRelationshipCreation ?? false);

            RelationshipTypeSelected();

            BindHelp();
        }

        public void LoadData(Guid userId, string userName, PersonFinderSecurityInfoWrapper finderSecurityInfo)
        {
            UserIdentifier = userId;

            ToUserName.Text = userName;

            RelationshipTypeSelected();

            PersonIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!finderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
            {
                PersonIdentifier.Filter.ParentUserIdentifier = User.UserIdentifier;
                PersonIdentifier.Filter.DepartmentsForParentId = User.UserIdentifier;
            }

            PersonIdentifier.Filter.ExcludeUserIdentifier = UserIdentifier;

            BindGrid();
        }

        public void SetSelectionEnabled(bool isLeaderEnabled, bool isManagerEnabled, bool isSupervisorEnabled, bool isValidatorEnabled)
        {
            var items = RelationshipType.Items;

            if (!isLeaderEnabled)
                items.Remove(items.FindByValue(UserConnectionType.Leader));

            if (!isManagerEnabled)
                items.Remove(items.FindByValue(UserConnectionType.Manager));

            if (!isSupervisorEnabled)
                items.Remove(items.FindByValue(UserConnectionType.Supervisor));

            if (!isValidatorEnabled)
                items.Remove(items.FindByValue(UserConnectionType.Validator));

            if (items.Count > 1)
                RelationshipType.SelectedIndex = 1;
        }

        private void RelationshipTypeSelected()
        {
            PersonIdentifier.Value = null;
            PersonIdentifier.Filter.RelationCategory = RelationshipType.SelectedValue.ToEnum(RelationCategory.Manager);
        }

        private void BindHelp()
        {
            string html;

            try
            {
                html = GetEmbededHelpContent("#reporting-lines").NullIfEmpty();
            }
            catch (InvalidOperationException ioex)
            {
                html = null;

                OnAlert(AlertType.Error, "Error loading embedded help content. " + ioex.Message);
            }

            ReportingLineHelp.Visible = html != null;

            if (ReportingLineHelp.Visible)
                ReportingLineHelp.InnerHtml = html;
        }

        private void AddRelationship_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            AddManagerToPerson();

            BindGrid();

            PersonIdentifier.Value = null;
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            var descriptions = GetRelationCategory();
            var relationships = UserConnectionSearch.SelectCmdsDetails(UserIdentifier, Organization.Identifier, descriptions);

            Grid.DataSource = relationships;
            GridColumn.Visible = relationships.Count > 0;
        }

        protected void OnRelationshipChecked(object sender, EventArgs e)
        {
            if (sender is CheckBox cb)
            {
                var row = (GridViewRow)cb.NamingContainer;

                var fromUserIdentifier = Grid.GetDataKey<Guid>(row);
                var toUserIdentifier = UserIdentifier;

                var c = UserConnectionSearch.Select(fromUserIdentifier, toUserIdentifier);

                if (c == null)
                    c = CreateUserConnection(fromUserIdentifier, toUserIdentifier);

                BindUserConnection(c, cb);

                if (c.IsManager || c.IsSupervisor || c.IsValidator)
                    ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
                else
                    UserConnectionStore.Delete(c);
            }

            BindGrid();
        }

        private void AddManagerToPerson()
        {
            var personId = PersonIdentifier.Value;

            if (!personId.HasValue)
                return;

            var fromUserIdentifier = personId.Value;
            var toUserIdentifier = UserIdentifier;
            var c = CreateUserConnection(fromUserIdentifier, toUserIdentifier);

            ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
        }

        private UserConnection CreateUserConnection(Guid from, Guid to)
        {
            var relationCategory = RelationshipType.SelectedValue;

            return new UserConnection
            {
                FromUserIdentifier = from,
                ToUserIdentifier = to,
                IsLeader = relationCategory == "Leader",
                IsManager = relationCategory == "Manager",
                IsSupervisor = relationCategory == "Supervisor",
                IsValidator = relationCategory == "Validator"
            };
        }

        private void BindGrid()
        {
            GridColumn.Visible = true;
            Grid.DataBind();
        }

        private static void BindUserConnection(UserConnection relation, CheckBox box)
        {
            switch (box.Text)
            {
                case "Leader":
                    relation.IsLeader = box.Checked;
                    break;
                case "Manager":
                    relation.IsManager = box.Checked;
                    break;
                case "Supervisor":
                    relation.IsSupervisor = box.Checked;
                    break;
                case "Validator":
                    relation.IsValidator = box.Checked;
                    break;
            }
        }

        private string[] GetRelationCategory()
        {
            var relationCategory = new string[RelationshipType.Items.Count];

            for (var i = 0; i < RelationshipType.Items.Count; i++)
                relationCategory[i] = RelationshipType.Items[i].Value;

            return relationCategory;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e.Row.RowType))
                return;

            CmdsUserConnection item = (CmdsUserConnection)e.Row.DataItem;

            var isLeader = (CheckBox)e.Row.FindControl("IsLeader");
            var isManager = (CheckBox)e.Row.FindControl("IsManager");
            var isSupervisor = (CheckBox)e.Row.FindControl("IsSupervisor");
            var isValidator = (CheckBox)e.Row.FindControl("IsValidator");

            isLeader.Checked = item.IsLeader;
            isManager.Checked = item.IsManager;
            isManager.Checked = item.IsManager;
            isSupervisor.Checked = item.IsSupervisor;
            isValidator.Checked = item.IsValidator;

            isLeader.CssClass = CheckboxColor(item.CanBeLeader);
            isManager.CssClass = CheckboxColor(item.CanBeManager);
            isSupervisor.CssClass = CheckboxColor(item.CanBeSupervisor);
            isValidator.CssClass = CheckboxColor(item.CanBeValidator);

            isLeader.Enabled = CheckboxEnabled(item.CanBeLeader, CmdsRole.Leaders);
            isManager.Enabled = CheckboxEnabled(item.CanBeManager, CmdsRole.Managers);
            isSupervisor.Enabled = CheckboxEnabled(item.CanBeSupervisor, CmdsRole.Supervisors);
            isValidator.Enabled = CheckboxEnabled(item.CanBeValidator, CmdsRole.Validators);
        }

        private string CheckboxColor(bool condition)
        {
            return condition ? "" : "text-danger";
        }

        private bool CheckboxEnabled(bool condition, string role)
        {
            return condition
                && (Identity.IsInRole(role)
                 || Identity.IsInRole(CmdsRole.Programmers)
                 || Identity.IsInRole(CmdsRole.SystemAdministrators)
                 || Identity.IsInRole(CmdsRole.FieldAdministrators)
                 );
        }
    }
}