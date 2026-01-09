using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.UI.Admin.Records.Logbooks.Controls
{
    public partial class AddUsersControl : BaseUserControl
    {
        #region Properties

        public Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        private bool IsUserCreated => Request.QueryString["userCreated"] == "1";

        public string SearchUrl
        {
            get => (string)ViewState[nameof(SearchUrl)];
            set => ViewState[nameof(SearchUrl)] = value;
        }

        public string OutlineUrl
        {
            get => (string)ViewState[nameof(OutlineUrl)];
            set => ViewState[nameof(OutlineUrl)] = value;
        }

        public string ScreenUrl
        {
            get => (string)ViewState[nameof(ScreenUrl)];
            set => ViewState[nameof(ScreenUrl)] = value;
        }

        private List<Guid> SearchResultDataKeys
        {
            get => (List<Guid>)ViewState[nameof(SearchResultDataKeys)];
            set => ViewState[nameof(SearchResultDataKeys)] = value;
        }

        private HashSet<Guid> SearchSelectedEntities
        {
            get => (HashSet<Guid>)ViewState[nameof(SearchSelectedEntities)];
            set => ViewState[nameof(SearchSelectedEntities)] = value;
        }

        private Guid[] SavedIdentifiers
        {
            get => (Guid[])ViewState[nameof(SavedIdentifiers)];
            set => ViewState[nameof(SavedIdentifiers)] = value;
        }

        protected string PreviousContactType
        {
            get => (string)ViewState[nameof(PreviousContactType)];
            set => ViewState[nameof(PreviousContactType)] = value;
        }

        protected bool IsGroup
        {
            get => (bool)ViewState[nameof(IsGroup)];
            set => ViewState[nameof(IsGroup)] = value;
        }

        protected bool IsPerson
        {
            get => (bool)ViewState[nameof(IsPerson)];
            set => ViewState[nameof(IsPerson)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaContactType.AutoPostBack = true;
            CriteriaContactType.ValueChanged += (s, a) => OnCriteriaContactTypeChanged();

            CriteriaEventIdentifier.AutoPostBack = true;
            CriteriaEventIdentifier.ValueChanged += (s, a) => Search();

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

            var selectedCount = SearchSelectedEntities != null ? SearchSelectedEntities.Count : 0;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(AddUsersControl),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

        private void OnCriteriaContactTypeChanged()
        {
            var contactType = CriteriaContactType.Value;

            CriteriaGroupTypeField.Visible = contactType == "Group";
            CriteriaEmailField.Visible = contactType == "Person";
            CriteriaEventIdentifierField.Visible = !IsUserCreated && contactType == "Person";
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

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "Identifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedEntities.Contains(id);

            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedEntities.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            Save();

            HttpResponseHelper.Redirect($"{OutlineUrl}?journalsetup={JournalSetupIdentifier}&panel=users");
        }

        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsUserCreated && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedEntities = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;

                CriteriaContactType.Enabled = false;
            }
            else
            {
                CriteriaEventIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                CriteriaEventIdentifier.Filter.EventType = "Class";
                CriteriaEventIdentifier.Value = journalSetup.EventIdentifier;

                var allowNewContact = Identity.IsGranted(ActionName.Admin_Records_Logbooks_AddUsers_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Records_Logbooks_AddUsers_UploadContact);

                var returnUrl = $"{ScreenUrl}?journalsetup={JournalSetupIdentifier}";
                if (Request.QueryString["return"].IsNotEmpty())
                    returnUrl += $"&return={Request.QueryString["return"]}";

                returnUrl = HttpUtility.UrlEncode(returnUrl);

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&journalsetup={JournalSetupIdentifier}&action=logbook_add_learners";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&journalsetup={JournalSetupIdentifier}&action=logbook_add_learners";

                CriteriaClear();
            }

            CriteriaContactType.Value = "Person";

            OnCriteriaContactTypeChanged();

            Search();

            SearchResultCloseButton.NavigateUrl = $"{OutlineUrl}?journalsetup={JournalSetupIdentifier}&panel=users";
        }

        private void CriteriaClear()
        {
            if (CriteriaContactType.Enabled)
                CriteriaContactType.ClearSelection();

            CriteriaGroupType.Value = null;
            CriteriaName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaEventIdentifier.Value = null;

            OnCriteriaContactTypeChanged();

            SearchSelectedEntities = new HashSet<Guid>();
        }

        private void Save()
        {
            if (IsGroup)
                SaveGroups();
            else
                SavePersons();
        }

        private void SaveGroups()
        {
            foreach (var groupId in SearchSelectedEntities)
            {
                if (ServiceLocator.JournalSearch.ExistsJournalSetupGroup(JournalSetupIdentifier, groupId))
                    continue;

                ServiceLocator.SendCommand(new CreateJournalSetupGroup(JournalSetupIdentifier, groupId));
            }
        }

        private void SavePersons()
        {
            foreach (var userId in SearchSelectedEntities)
            {
                var learner = ServiceLocator.JournalSearch
                    .GetJournalSetupUser(JournalSetupIdentifier, userId, JournalSetupUserRole.Learner);

                if (learner != null)
                    continue;

                ServiceLocator.SendCommand(
                    new AddJournalSetupUser(
                        JournalSetupIdentifier,
                        userId,
                        JournalSetupUserRole.Learner));
            }
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var contactType = CriteriaContactType.Value;

            if (PreviousContactType != contactType)
            {
                PreviousContactType = contactType;
                SearchSelectedEntities = new HashSet<Guid>();
            }

            IsGroup = contactType == "Group";
            IsPerson = contactType == "Person";

            SearchResultHeaderGroup.Visible = IsGroup;
            SearchResultHeaderPerson.Visible = IsPerson;

            int count;
            if (IsGroup)
            {
                EntityName.Text = "Groups";
                count = ServiceLocator.GroupSearch.CountGroups(GetGroupFilter());
            }
            else if (IsPerson)
            {
                EntityName.Text = "People";
                count = ServiceLocator.ContactSearch.Count(GetUserFilter());
            }
            else
                throw ApplicationError.Create("Not expected contact type: " + contactType);

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
            return IsGroup ? GetGroupResultData() : GetUserResultData();
        }

        private object GetGroupResultData()
        {
            var filter = GetGroupFilter();

            filter.Paging = Paging.SetSkipTake(SearchResultPagination.ItemsSkip, SearchResultPagination.ItemsTake);

            var groups = ServiceLocator.GroupSearch.SearchGroupDetails(filter);

            return groups
                .Select(x => new
                {
                    Identifier = x.GroupIdentifier,
                    Code = x.GroupCode,
                    Name = x.GroupName,
                    Email = (string)null,
                    EmailAlternate = (string)null,
                    Size = x.MembershipCount,
                })
                .ToArray();
        }

        private object GetUserResultData()
        {
            var filter = GetUserFilter();
            filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);
            filter.OrderBy = "Name,Email";

            return ServiceLocator.ContactSearch
                .Bind(
                    x => new
                    {
                        Identifier = x.UserIdentifier,
                        Name = x.UserFullName,
                        Email = x.UserEmail,
                        EmailAlternate = x.UserEmailAlternate
                    },
                    filter
                );
        }

        private void GetCriteriaResultSelections()
        {
            if (!SearchResultRepeater.Visible)
                return;

            foreach (RepeaterItem item in SearchResultRepeater.Items)
            {
                var checkBox = (ICheckBoxControl)item.FindControl("Selected");
                var userId = SearchResultDataKeys[item.ItemIndex];

                if (!checkBox.Checked && SearchSelectedEntities.Contains(userId))
                    SearchSelectedEntities.Remove(userId);
                else if (checkBox.Checked && !SearchSelectedEntities.Contains(userId))
                    SearchSelectedEntities.Add(userId);
            }
        }

        private QGroupFilter GetGroupFilter()
        {
            return new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                GroupNameLike = CriteriaName.Text,
                GroupType = CriteriaGroupType.Value,
                ExcludeJournalSetupIdentifier = JournalSetupIdentifier,
            };
        }

        private QUserFilter GetUserFilter()
        {
            return new QUserFilter
            {
                OrganizationIdentifiers = new[] { Organization.OrganizationIdentifier },
                FullName = CriteriaName.Text,
                NameFilterType = "Exact",
                EmailOrAlternate = CriteriaEmail.Text,
                RegistrationEventIdentifier = CriteriaEventIdentifier.Visible ? CriteriaEventIdentifier.Value : null,
                ExcludeLearnerJournalSetupIdentifier = JournalSetupIdentifier,
                UserIdentifiers = SavedIdentifiers.NullIfEmpty()
            };
        }

        protected string GetContactSize(object dataItem)
        {
            return IsGroup
                ? "Person".ToQuantity((int)DataBinder.Eval(dataItem, "Size"))
                : null;
        }

        #endregion
    }
}