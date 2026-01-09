using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class MembershipGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Fields

        private Guid[] _organizationList;
        private ReturnUrl _returnUrl;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;

            Grid.RowDataBound += Grid_ItemDataBound;
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            CommandsSection.Visible = AllowEdit;

            Grid.Columns.FindByName("Commands").Visible = AllowEdit;

            base.OnPreRender(e);
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        public virtual bool AllowEdit
        {
            get => ViewState[nameof(AllowEdit)] == null || (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        protected Guid UserIdentifier
        {
            get => (Guid?)ViewState[nameof(UserIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        private Guid[] UserOrganizations
        {
            get
            {
                if (_organizationList == null)
                {
                    _organizationList = CurrentSessionState.Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray();
                    if (_organizationList.Length == 0)
                        _organizationList = new[] { Organization.OrganizationIdentifier };
                }

                return _organizationList;
            }
        }

        public string ReturnQuery
        {
            get => (string)ViewState[nameof(ReturnQuery)];
            set => ViewState[nameof(ReturnQuery)] = value;
        }

        #endregion

        #region Public methods

        public void LoadData(Guid userIdentifier)
        {
            UserIdentifier = userIdentifier;

            _returnUrl = !ReturnQuery.IsEmpty() ? new ReturnUrl(ReturnQuery) : null;

            var user = UserSearch.Select(userIdentifier);

            Search(new NullFilter());

            var createUrl = $"/ui/admin/contacts/people/create-membership?user={user.UserIdentifier}";

            AddButton.NavigateUrl = _returnUrl == null
                ? createUrl
                : _returnUrl.GetRedirectUrl(createUrl);
        }

        public void Clear()
        {
            Clear(new NullFilter());
        }

        #endregion

        #region Event handlers

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(MembershipGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = e.Row.DataItem;
            var name = DataBinder.Eval(row, "GroupName") as string;
            var thumbprint = (Guid)DataBinder.Eval(row, "GroupIdentifier");
            var organizationIdentifier = (Guid)DataBinder.Eval(row, "OrganizationIdentifier");
            var organizationCode = (string)DataBinder.Eval(row, "OrganizationCode");
            var groupParentId = (Guid?)DataBinder.Eval(row, "GroupParentIdentifier");

            if (name.HasValue())
            {
                var separatorIndex = name.IndexOf('/', 1);
                if (separatorIndex != -1)
                    name = name.Substring(separatorIndex, name.Length - separatorIndex);
            }

            var allowEditGroup = Identity.IsActionAuthorized("ui/admin/contacts/groups/edit") && Organization.OrganizationIdentifier == organizationIdentifier;

            var groupEditorLink = (HyperLink)e.Row.FindControl("GroupEditorLink");
            groupEditorLink.Visible = allowEditGroup;
            if (groupEditorLink.Visible)
            {
                groupEditorLink.Text = name;
                groupEditorLink.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={thumbprint}";
            }

            var groupNameLiteral = e.Row.FindControl("GroupName");
            groupNameLiteral.Visible = !allowEditGroup;
            if (groupNameLiteral.Visible) ((ITextControl)groupNameLiteral).Text = name;

            var organizationNameLiteral = e.Row.FindControl("OrganizationName");
            organizationNameLiteral.Visible = !organizationCode.Equals(Organization.Code, StringComparison.OrdinalIgnoreCase);
            if (organizationNameLiteral.Visible) ((ITextControl)organizationNameLiteral).Text = $"<span class='form-text'>[{organizationCode}]</span>";

            var groupParentLiteral = e.Row.FindControl("GroupParent");
            if (groupParentLiteral.Visible = groupParentId.HasValue)
            {
                var groupParentType = (string)DataBinder.Eval(row, "GroupParentType");
                var groupParentName = (string)DataBinder.Eval(row, "GroupParentName");

                ((ITextControl)groupParentLiteral).Text = $"<span class='form-text'>{groupParentType} {groupParentName}</span>";
            }

            var editButton = (IconLink)e.Row.FindControl("EditButton");
            var deleteButton = (IconLink)e.Row.FindControl("DeleteButton");

            editButton.Visible = deleteButton.Visible = Organization.OrganizationIdentifier == organizationIdentifier;

            var membership = (Guid)DataBinder.Eval(row, "MembershipIdentifier");
            var history = (IconLink)e.Row.FindControl("HistoryLink");
            var returnUrl = $"/ui/admin/contacts/people/edit?contact={UserIdentifier}&panel=groups";
            history.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={membership}&returnURL=" + HttpUtility.UrlEncode(returnUrl);
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter filter)
        {
            return MembershipSearch.Count(FilterQuery());
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            return MembershipSearch.Bind(x => new
            {
                MembershipIdentifier = x.MembershipIdentifier,
                GroupName = x.Group.GroupName,
                GroupIdentifier = x.Group.GroupIdentifier,
                GroupType = x.Group.GroupType,
                GroupParentIdentifier = (Guid?)x.Group.Parent.GroupIdentifier,
                GroupParentType = x.Group.Parent.GroupType,
                GroupParentName = x.Group.Parent.GroupName,
                RoleType = x.MembershipType,
                OrganizationIdentifier = x.Group.Organization.OrganizationIdentifier,
                OrganizationCode = x.Group.Organization.OrganizationCode,
                Assigned = x.Assigned,
                MembershipExpiry = x.MembershipExpiry
            }, FilterQuery(), filter.Paging, "GroupType, GroupName").ToSearchResult();
        }

        private Expression<Func<Membership, bool>> FilterQuery()
        {
            Expression<Func<Membership, bool>> where = x => x.UserIdentifier == UserIdentifier && UserOrganizations.Contains(x.Group.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(FilterTextBox.Text))
            {
                var keyword = FilterTextBox.Text;
                where = where.And(x => x.Group.GroupName.Contains(keyword));
            }

            return where.Expand();
        }

        #endregion

        #region Methods (helpers)

        protected string GetEditUrl()
        {
            var url = $"/ui/admin/contacts/people/edit-membership?from={Eval("GroupIdentifier")}&to={UserIdentifier}";

            if (_returnUrl != null)
                url = _returnUrl.GetRedirectUrl(url);

            return url;
        }

        protected string GetDeleteUrl()
        {
            var url = $"/ui/admin/contacts/people/delete-membership?from={Eval("GroupIdentifier")}&to={UserIdentifier}";

            if (_returnUrl != null)
                url = _returnUrl.GetRedirectUrl(url);

            return url;
        }

        #endregion
    }
}