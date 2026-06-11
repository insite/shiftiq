using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Contacts.Read;
using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.UI.Admin.Records.Logbooks.Validators
{
    public partial class Assign : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/records/logbooks/search";

        #endregion

        #region Properties

        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        private string OutlineUrl =>
            $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=setup";

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

        private string SelectedContactType
        {
            get => (string)ViewState[nameof(SelectedContactType)] ?? "Person";
            set => ViewState[nameof(SelectedContactType)] = value;
        }

        protected bool IsGroup => SelectedContactType == "Group";
        protected bool IsPerson => SelectedContactType == "Person";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaContactType.AutoPostBack = true;
            CriteriaContactType.ValueChanged += (s, a) => OnCriteriaContactTypeChanged();

            CriteriaGroupIdentifier.AutoPostBack = true;
            CriteriaGroupIdentifier.ItemsRequested += CriteriaGroupIdentifier_ItemsRequested;
            CriteriaGroupIdentifier.ValueChanged += (s, a) => Search();

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

                CriteriaGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

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
                typeof(Assign),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

        private void OnCriteriaContactTypeChanged()
        {
            var contactType = CriteriaContactType.Value;

            GroupCriteriaPanel.Visible = contactType == "Group";
            PersonCriteriaPanel.Visible = contactType == "Person";
        }

        private void CriteriaGroupIdentifier_ItemsRequested(object sender, EventArgs e)
        {
            CriteriaGroupIdentifier.Filter.GroupType = CriteriaGroupType.Value;
            CriteriaGroupIdentifier.Filter.GroupLabel = CriteriaGroupLabel.Text;
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

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone));

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedEntities = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;

                CriteriaContactType.Enabled = false;
            }
            else
            {
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Records_Logbooks_AddUsers_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Records_Logbooks_AddUsers_UploadContact);
                var returnUrl = HttpUtility.UrlEncode($"/ui/admin/records/logbooks/validators/assign?journalsetup={JournalSetupIdentifier}");

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&journalsetup={JournalSetupIdentifier}&action=logbook_add_validators";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&journalsetup={JournalSetupIdentifier}&action=logbook_add_validators";

                CriteriaClear();
            }

            CriteriaContactType.Value = "Person";

            OnCriteriaContactTypeChanged();

            Search();

            SearchResultCloseButton.NavigateUrl = OutlineUrl;
        }

        private void CriteriaClear()
        {
            if (CriteriaContactType.Enabled)
                CriteriaContactType.ClearSelection();

            CriteriaName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaGroupType.Value = null;
            CriteriaGroupLabel.Text = null;
            CriteriaGroupIdentifier.Value = null;

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
                if (ServiceLocator.JournalSearch.ExistsJournalSetupGroup(JournalSetupIdentifier, groupId, JournalSetupUserRole.Validator))
                    continue;

                ServiceLocator.SendCommand(new CreateJournalSetupGroup(JournalSetupIdentifier, groupId, JournalSetupUserRole.Validator));
            }
        }

        private void SavePersons()
        {
            foreach (var userId in SearchSelectedEntities)
            {
                var validator = ServiceLocator.JournalSearch
                    .GetJournalSetupUser(JournalSetupIdentifier, userId, JournalSetupUserRole.Validator);

                if (validator == null)
                    ServiceLocator.SendCommand(new AddJournalSetupUser(JournalSetupIdentifier, userId, JournalSetupUserRole.Validator));
            }
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            if (SelectedContactType != CriteriaContactType.Value)
            {
                SelectedContactType = CriteriaContactType.Value;
                SearchSelectedEntities = new HashSet<Guid>();
            }

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
                count = ServiceLocator.PersonSearch.CountPersons(GetPersonFilter());
            }
            else
                throw ApplicationError.Create("Not expected contact type: " + SelectedContactType);

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
            return IsGroup ? GetGroupResultData() : GetPersonResultData();
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
                    EmployerIdentifier = (Guid?)null,
                    EmployerName = (string)null,
                    Size = x.MembershipCount,
                })
                .ToArray();
        }

        private object GetPersonResultData()
        {
            var filter = GetPersonFilter();
            filter.OrderBy = "User.FullName,User.Email";
            filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User, x => x.EmployerGroup).Select(x => new
            {
                Identifier = x.UserIdentifier,
                Code = x.PersonCode,
                Name = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                EmployerIdentifier = x.EmployerGroup?.GroupIdentifier,
                EmployerName = x.EmployerGroup?.GroupName,
                Size = (int?)null
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
                GroupNameLike = Group_GroupName.Text,
                GroupType = Group_GroupType.Value,
                ExcludeValidatorJournalSetupIdentifier = JournalSetupIdentifier,
            };
        }

        private QPersonFilter GetPersonFilter()
        {
            var filter = new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifiers = SavedIdentifiers,
                ExcludeValidatorJournalSetupIdentifier = JournalSetupIdentifier,
                UserNameContains = CriteriaName.Text,
                UserEmailContains = CriteriaEmail.Text,
                UserMembershipGroupLabelContains = CriteriaGroupLabel.Text
            };

            if (CriteriaGroupType.Value.IsNotEmpty())
            {
                if (CriteriaGroupIdentifier.HasValue)
                    filter.UserMembershipGroupIdentifier = CriteriaGroupIdentifier.Value;
                else
                    filter.UserMembershipGroupTypeExact = CriteriaGroupType.Value;
            }

            return filter;
        }

        protected string GetContactSize(object dataItem)
        {
            return IsGroup
                ? "Person".ToQuantity((int)DataBinder.Eval(dataItem, "Size"))
                : null;
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=setup"
                : null;
        }

        #endregion
    }
}
