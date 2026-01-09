using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Memberships.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class CreateRole : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string EditUrl = "/ui/admin/contacts/people/edit";
        private const string SearchUrl = "/ui/admin/contacts/people/search";

        #endregion

        #region Properties

        private Guid? UserIdentifier =>
            Guid.TryParse(Request["user"], out Guid value) ? value : (Guid?)null;

        private List<Guid> SearchResultDataKeys
        {
            get => (List<Guid>)ViewState[nameof(SearchResultDataKeys)];
            set => ViewState[nameof(SearchResultDataKeys)] = value;
        }

        private HashSet<Guid> SearchSelectedGroups
        {
            get => (HashSet<Guid>)ViewState[nameof(SearchSelectedGroups)];
            set => ViewState[nameof(SearchSelectedGroups)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaSearchButton.Click += (s, a) => Search();
            CriteriaClearButton.Click += (s, a) => { CriteriaClear(); Search(); };

            ReasonSubtype.AutoPostBack = true;
            ReasonSubtype.ValueChanged += ReasonSubtype_ValueChanged;

            SearchResultPagination.PageChanged += SearchResultPagination_PageChanged;

            SearchResultRepeater.DataBinding += SearchResultRepeater_DataBinding;
            SearchResultRepeater.ItemDataBound += SearchResultRepeater_ItemDataBound;

            SearchResultSaveButton.Click += SearchResultSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
            else
                GetCriteriaResultSelections();
        }

        private void Open()
        {
            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            var user = UserIdentifier.HasValue
                ? UserSearch.Select(UserIdentifier.Value)
                : null;
            if (user == null || user.IsCloaked && !User.IsCloaked)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: user.FullName);

            CriteriaClear();
            Search();

            SearchResultCloseButton.NavigateUrl = GetReturnUrl();
        }

        #endregion

        #region Event handlers

        private void ReasonSubtype_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var isEnabled = e.NewValue.IsNotEmpty();

            ReasonFieldsContainer.Visible = isEnabled;

            if (isEnabled && e.OldValue.IsEmpty())
            {
                var nowDate = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, User.TimeZone);

                ReasonEffectiveDate.Value = nowDate;
                ReasonExpiryDate.Value = nowDate.AddDays(180);
            }
        }

        private void SearchResultPagination_PageChanged(object sender, Pagination.PageChangedEventArgs e)
        {
            SearchResultRepeater.DataBind();
        }

        private void SearchResultRepeater_DataBinding(object sender, EventArgs e)
        {
            SearchResultDataKeys = new List<Guid>();

            SearchResultRepeater.DataSource = GetCriteriaResultData();
        }

        private void SearchResultRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "GroupIdentifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedGroups.Contains(id);

            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (SearchSelectedGroups.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected groups");
                return;
            }

            var userId = UserIdentifier.Value;
            var addReason = ReasonSubtype.Value.IsNotEmpty();

            foreach (var groupId in SearchSelectedGroups)
            {
                if (!MembershipPermissionHelper.CanModifyMembership(groupId))
                    continue;

                var membershipId = MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = groupId,
                    UserIdentifier = userId,
                    MembershipType = RoleType.Value,
                    Assigned = DateTimeOffset.UtcNow
                });

                if (!addReason)
                    continue;

                var isExists = ServiceLocator.MembershipReasonSearch.Exists(new QMembershipReasonFilter
                {
                    MembershipIdentifier = membershipId
                });

                if (!isExists)
                {
                    ServiceLocator.SendCommand(new AddMembershipReason(
                        membershipId,
                        UniqueIdentifier.Create(),
                        "Referral",
                        ReasonSubtype.Value,
                        ReasonEffectiveDate.Value.Value,
                        ReasonExpiryDate.Value,
                        ReasonPersonOccupation.Text));
                }
            }

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var count = ServiceLocator.GroupSearch.CountGroups(GetFilter());
            var hasData = count > 0;

            SearchResultPagination.ItemsCount = count;
            SearchResultPagination.PageIndex = 0;
            SearchResultFooter.Visible = SearchResultPagination.PageCount > 1;
            SearchResultCount.InnerText = count.ToString("n0");

            SearchResultUpdatePanel.Visible = hasData;
            SearchResultCount.Visible = hasData;
            SearchNoResultContainer.Visible = !hasData;

            SearchResultRepeater.DataBind();
        }

        private object GetCriteriaResultData()
        {
            var filter = GetFilter();
            filter.Paging = Paging.SetSkipTake(SearchResultPagination.ItemsSkip, SearchResultPagination.ItemsTake);

            return ServiceLocator.GroupSearch.GetGroups(filter);
        }

        private void GetCriteriaResultSelections()
        {
            if (!SearchResultRepeater.Visible)
                return;

            foreach (RepeaterItem item in SearchResultRepeater.Items)
            {
                var checkBox = (ICheckBoxControl)item.FindControl("Selected");
                var userId = SearchResultDataKeys[item.ItemIndex];

                if (!checkBox.Checked)
                    SearchSelectedGroups.Remove(userId);
                else if (checkBox.Checked)
                    SearchSelectedGroups.Add(userId);
            }
        }

        private QGroupFilter GetFilter()
        {
            return new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                GroupType = CriteriaGroupType.Value,
                Keyword = CriteriaKeyword.Text,
                OnlyOperatorCanAddUser = !MembershipPermissionHelper.CanModifyAdminMemberships() ? false : (bool?)null
            };
        }

        #endregion

        #region Methods (helpers)

        private void CriteriaClear()
        {
            CriteriaGroupType.Value = null;
            CriteriaKeyword.Text = null;

            SearchSelectedGroups = new HashSet<Guid>();
        }

        protected override string GetReturnUrl()
        {
            var returnUrl = new ReturnUrl();

            return returnUrl.GetReturnUrl()
                ?? EditUrl + $"?contact={UserIdentifier}&panel=groups";
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={UserIdentifier}"
                : null;
        }

        #endregion
    }
}