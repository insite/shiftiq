using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class ReferralGrid : BaseUserControl
    {
        #region Properties

        public virtual bool AllowEdit
        {
            get => ViewState[nameof(AllowEdit)] == null || (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        private QMembershipReasonFilter Filter
        {
            get => (QMembershipReasonFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        public string ReturnQuery
        {
            get => (string)ViewState[nameof(ReturnQuery)];
            set => ViewState[nameof(ReturnQuery)] = value;
        }

        private ReturnUrl ReturnUrl
        {
            get
            {
                if (!_returnUrlInited)
                {
                    _returnUrl = !ReturnQuery.IsEmpty() ? new ReturnUrl(ReturnQuery) : null;
                    _returnUrlInited = true;
                }

                return _returnUrl;
            }
        }

        public int RowCount => Grid.VirtualItemCount;

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;
        private bool _returnUrlInited;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowDataBound += Grid_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            CommandsSection.Visible = AllowEdit;

            Grid.Columns.FindByName("Commands").Visible = AllowEdit;

            base.OnPreRender(e);
        }

        #endregion

        #region Public methods

        public void LoadData(Guid userId)
        {
            Filter = new QMembershipReasonFilter
            {
                UserIdentifier = userId,
                GroupOrganizationIdentifiers = CurrentSessionState.Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray(),
                OrderBy = "Membership.Group.GroupType,Membership.Group.GroupName"
            };

            if (Filter.GroupOrganizationIdentifiers.IsEmpty())
                Filter.GroupOrganizationIdentifiers = new[] { Organization.OrganizationIdentifier };

            ResetGrid();
            UpdateVisibility();

            AddButton.NavigateUrl = GetRedirectUrl($"/ui/admin/contacts/memberships/reasons/create?user={userId}");
        }

        #endregion

        #region Event handlers

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            Filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            Grid.DataSource = ServiceLocator.MembershipReasonSearch.SelectForReferralGrid(Filter);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Filter.GroupName = FilterTextBox.Text.Trim().NullIfEmpty();

            ResetGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ReferralGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var data = (ReferralGridDataItem)e.Row.DataItem;

            var isCurrentOrg = data.GroupOrganizationIdentifier == Organization.Identifier;
            var hasParent = data.GroupParentIdentifier.HasValue;
            var allowEditGroup = isCurrentOrg && Identity.IsActionAuthorized("ui/admin/contacts/groups/edit");

            var groupName = data.GroupName;
            if (groupName.IsNotEmpty())
            {
                var separatorIndex = groupName.IndexOf('/', 1);
                if (separatorIndex != -1)
                    groupName = groupName.Substring(separatorIndex, groupName.Length - separatorIndex);
            }

            SetLink("GroupEditorLink", groupName, $"/ui/admin/contacts/groups/edit?contact={data.GroupIdentifier}", allowEditGroup);
            SetLiteral("GroupName", !allowEditGroup, () => groupName);
            SetLiteral("OrganizationName", !isCurrentOrg, () => $"<span class='form-text'>[{data.GroupOrganizationCode}]</span>");
            SetLiteral("GroupParent", hasParent, () => $"<span class='form-text'>{data.GroupParentType} {data.GroupParentName}</span>");

            var editButton = (IconLink)e.Row.FindControl("EditButton");
            var deleteButton = (IconLink)e.Row.FindControl("DeleteButton");

            editButton.Visible = deleteButton.Visible = isCurrentOrg;

            var history = (IconLink)e.Row.FindControl("HistoryLink");
            var returnUrl = $"/ui/admin/contacts/people/edit?contact={data.UserIdentifier}&panel=referrals";
            history.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={data.MembershipIdentifier}&returnURL=" + HttpUtility.UrlEncode(returnUrl);

            void SetLiteral(string id, bool visible, Func<string> getText)
            {
                var literal = (System.Web.UI.WebControls.Literal)e.Row.FindControl(id);
                if (visible)
                    literal.Text = getText();
                literal.Visible = visible;
            }

            void SetLink(string id, string text, string href, bool visible)
            {
                var link = (HyperLink)e.Row.FindControl(id);
                link.Visible = visible;
                link.Text = text;
                link.NavigateUrl = href;
            }
        }

        #endregion

        #region Methods (grid)

        private void ResetGrid()
        {
            Grid.PageIndex = 0;
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            Grid.VirtualItemCount = ServiceLocator.MembershipReasonSearch.Count(Filter);
            Grid.DataBind();
        }

        private void UpdateVisibility()
        {
            var hasData = Grid.VirtualItemCount > 0;

            FilterField.Visible = hasData || Filter.GroupName.IsNotEmpty();
            EmptyGrid.Visible = !hasData;
            Grid.Visible = hasData;
        }

        #endregion

        #region Methods (helpers)

        protected string GetEditUrl() =>
            GetRedirectUrl($"/ui/admin/contacts/memberships/reasons/edit?reason={Eval("ReasonIdentifier")}");

        protected string GetDeleteUrl() =>
            GetRedirectUrl($"/ui/admin/contacts/memberships/reasons/delete?reason={Eval("ReasonIdentifier")}");

        private string GetRedirectUrl(string url)
        {
            var retUrl = ReturnUrl;
            return retUrl == null ? url : retUrl.GetRedirectUrl(url);
        }

        #endregion
    }
}