using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Contacts.Groups.Forms
{
    public partial class CreateMembership : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string EditUrl = "/ui/admin/contacts/groups/edit";
        private const string SearchUrl = "/ui/admin/contacts/groups/search";

        #endregion

        #region Enums

        private enum SearchTypeEnum
        {
            [Description("By Search Criteria")]
            Criteria,

            [Description("By Email Address List")]
            EmailList,

            [Description("New Users")]
            NewUsers
        }

        #endregion

        #region Properties

        private Guid? GroupIdentifier
        {
            get => (Guid?)ViewState[nameof(GroupIdentifier)];
            set => ViewState[nameof(GroupIdentifier)] = value;
        }

        private List<Guid> SearchResultDataKeys
        {
            get => (List<Guid>)ViewState[nameof(SearchResultDataKeys)];
            set => ViewState[nameof(SearchResultDataKeys)] = value;
        }

        private HashSet<Guid> SearchSelectedUsers
        {
            get => (HashSet<Guid>)ViewState[nameof(SearchSelectedUsers)];
            set => ViewState[nameof(SearchSelectedUsers)] = value;
        }

        private Guid[] SavedIdentifiers
        {
            get => (Guid[])ViewState[nameof(SavedIdentifiers)];
            set => ViewState[nameof(SavedIdentifiers)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchType.AutoPostBack = true;
            SearchType.ValueChanged += SearchType_ValueChanged;

            EmailListSaveButton.Click += EmailListSaveButton_Click;

            CriteriaSearchButton.Click += (s, a) => Search();
            CriteriaClearButton.Click += (s, a) => { CriteriaClear(); Search(); };

            SearchResultPagination.PageChanged += SearchResultPagination_PageChanged;

            SearchResultRepeater.DataBinding += SearchResultRepeater_DataBinding;
            SearchResultRepeater.ItemDataBound += SearchResultRepeater_ItemDataBound;

            SearchResultSaveButton.Click += SearchResultSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanCreate)
                    HttpResponseHelper.Redirect(SearchUrl);

                Open();
            }
            else
            {
                GetCriteriaResultSelections();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var selectedCount = SearchSelectedUsers != null ? SearchSelectedUsers.Count : 0;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(CreateMembership),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

        private void SearchType_ValueChanged(object sender, ComboBoxValueChangedEventArgs e) => OnSearchTypeChanged();

        private void OnSearchTypeChanged()
        {
            var type = SearchType.Value.ToEnum<SearchTypeEnum>();
            var isCriteria = type == SearchTypeEnum.Criteria;
            var isEmailList = type == SearchTypeEnum.EmailList;
            var isNewUsers = type == SearchTypeEnum.NewUsers;

            CriteriaInputs.Visible = isCriteria;
            EmailListSection.Visible = isEmailList;
            SearchResultSection.Visible = isCriteria || isNewUsers;

            if (isCriteria)
            {
                CriteriaClear();
                Search();
            }
            else if (isNewUsers)
            {
                SearchSelectedUsers = SavedIdentifiers.ToHashSet();

                Search();
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

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "UserIdentifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedUsers.Contains(id);

            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedUsers.Count > 0)
            {
                Save(SearchSelectedUsers, RoleType.Value);

                HttpResponseHelper.Redirect(GetCloseUrl());
            }
            else
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
        }

        private void EmailListSaveButton_Click(object sender, EventArgs e)
        {
            if (!CreateControl.ParseContacts(EmailList.Text, out var contacts, out var errors))
            {
                if (errors.Length > 0)
                    ScreenStatus.AddMessage(AlertType.Error, $"Validation errors:<br>{string.Join("<br>", errors)}");
                else if (contacts.Length == 0)
                    ScreenStatus.AddMessage(AlertType.Error, $"No contacts specified");

                return;
            }

            var people = PersonCriteria.Bind(
                x => x.UserIdentifier,
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    EmailsExact = contacts.Select(x => x.Email).ToArray()
                });

            if (people.Length > 0)
                Save(people, RoleType.Value);

            ScreenStatus.AddMessage(AlertType.Success, $"{people.Length:n0} contacts added to the group");
        }

        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var group = Guid.TryParse(Request.QueryString["group"], out Guid groupId)
                ? ServiceLocator.GroupSearch.GetGroup(groupId)
                : null;

            if (group == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            PageHelper.AutoBindHeader(Page, qualifier: $"{group.GroupName} <span class='form-text'>{group.GroupLabel ?? group.GroupType}</span>");

            GroupIdentifier = group.GroupIdentifier;

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SearchType.LoadItems(SearchTypeEnum.NewUsers);

                SavedIdentifiers = CreateControl.SavedIdentifiers;

                CreateControl.SavedIdentifiers = null;
            }
            else
            {
                SearchType.LoadItems(SearchTypeEnum.Criteria, SearchTypeEnum.EmailList);

                if (Organization.Identifier == OrganizationIdentifiers.RCABC)
                {
                    var allowNewContact = Identity.IsGranted(ActionName.Admin_Contacts_Groups_CreateMembership_NewContact);
                    var allowUploadContact = Identity.IsGranted(ActionName.Admin_Contacts_Groups_CreateMembership_UploadContact);
                    var returnUrl = HttpUtility.UrlEncode($"/ui/admin/contacts/groups/create-membership?group={GroupIdentifier}");

                    NewUserCard.Visible = allowNewContact || allowUploadContact;

                    CreateContactLink.Visible = allowNewContact;
                    CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&group={GroupIdentifier}&action=groups_createrole";

                    UploadContactLink.Visible = allowUploadContact;
                    UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&group={GroupIdentifier}&action=groups_createrole";
                }
                else
                {
                    NewUserCard.Visible = false;
                }
            }

            OnSearchTypeChanged();

            CriteriaCloseButton.NavigateUrl = GetCloseUrl();
            EmailListCloseButton.NavigateUrl = GetCloseUrl();
        }

        private void Save(IEnumerable<Guid> userIds, string roleType)
        {
            if (GroupIdentifier == null || !MembershipPermissionHelper.CanModifyMembership(GroupIdentifier.Value))
                return;

            foreach (var userKey in userIds)
            {
                MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = GroupIdentifier.Value,
                    UserIdentifier = userKey,
                    Assigned = DateTimeOffset.UtcNow,
                    MembershipType = roleType
                });
            }
        }

        private void CriteriaClear()
        {
            CriteriaName.Text = null;
            CriteriaEmail.Text = null;

            SearchSelectedUsers = new HashSet<Guid>();
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var count = ServiceLocator.PersonSearch.CountPersons(CreateFilter());
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
            var filter = CreateFilter();
            filter.OrderBy = "User.FullName,User.Email";
            filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User).Select(x => new
            {
                UserIdentifier = x.User.UserIdentifier,
                FullName = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                PersonCode = x.PersonCode
            });
        }

        private void GetCriteriaResultSelections()
        {
            if (!SearchResultRepeater.Visible)
                return;

            foreach (RepeaterItem item in SearchResultRepeater.Items)
            {
                var checkBox = (ICheckBoxControl)item.FindControl("Selected");
                var userId = SearchResultDataKeys[item.ItemIndex];

                if (!checkBox.Checked && SearchSelectedUsers.Contains(userId))
                    SearchSelectedUsers.Remove(userId);
                else if (checkBox.Checked && !SearchSelectedUsers.Contains(userId))
                    SearchSelectedUsers.Add(userId);
            }
        }

        private QPersonFilter CreateFilter()
        {
            var type = SearchType.Value.ToEnum<SearchTypeEnum>();
            var filter = new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            if (type == SearchTypeEnum.Criteria)
            {
                if (CriteriaName.Text.IsNotEmpty())
                    filter.UserNameContains = CriteriaName.Text;

                if (CriteriaEmail.Text.IsNotEmpty())
                    filter.UserEmailContains = CriteriaEmail.Text;
            }
            else if (type == SearchTypeEnum.NewUsers)
            {
                filter.UserIdentifiers = SavedIdentifiers.ToArray();
            }

            return filter;
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={Request.QueryString["group"]}&panel=people"
                : null;
        }

        #endregion

        #region Methods (helpers)

        private string GetCloseUrl() => $"{EditUrl}?contact={GroupIdentifier}&panel=people";

        #endregion
    }
}