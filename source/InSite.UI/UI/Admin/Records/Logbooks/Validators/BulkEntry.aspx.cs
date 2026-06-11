using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Contacts.Read;
using InSite.Application.Journals.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Validators
{
    public partial class BulkEntry : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        private string OutlineUrl => $"/ui/admin/records/logbooks/validators/outline?journalsetup={JournalSetupIdentifier}";

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

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaGroupIdentifier.AutoPostBack = true;
            CriteriaGroupIdentifier.ItemsRequested += CriteriaGroupIdentifier_ItemsRequested;
            CriteriaGroupIdentifier.ValueChanged += (s, a) => Search();

            CriteriaSearchButton.Click += (s, a) => Search();
            CriteriaClearButton.Click += (s, a) => { CriteriaClear(); Search(); };

            SearchResultPagination.PageChanged += SearchResultPagination_PageChanged;

            SearchResultRepeater.DataBinding += SearchResultRepeater_DataBinding;
            SearchResultRepeater.ItemDataBound += SearchResultRepeater_ItemDataBound;

            NextButton.Click += NextButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanCreate)
                    CreateAccessDeniedException();

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

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedEntities.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            FieldsTab.Visible = true;
            FieldsTab.IsSelected = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (SearchSelectedEntities.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                SearchTab.IsSelected = true;
                return;
            }

            var count = Save();

            var url = $"/ui/admin/records/logbooks/validators/outline?journalsetup={JournalSetupIdentifier}&bulk-added-entries={count}";

            HttpResponseHelper.Redirect(url);
        }


        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null)
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");

            if (journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || !ServiceLocator.JournalSearch.IsLogbookValidator(JournalSetupIdentifier, User.Identifier)
                || Organization.Toolkits.Logbooks?.LogbookBulkEntry != true
                )
            {
                CreateAccessDeniedException();
            }

            PageHelper.AutoBindHeader(Page, qualifier: LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone));

            var fieldCount = Fields.LoadData(JournalSetupIdentifier, null);

            if (fieldCount == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "This logbook doesn't contain any field");
                Nav.Visible = false;
                return;
            }

            CriteriaClear();

            Search();

            CloseButton.NavigateUrl = OutlineUrl;
            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void CriteriaClear()
        {
            CriteriaName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaGroupType.Value = null;
            CriteriaGroupLabel.Text = null;
            CriteriaGroupIdentifier.Value = null;

            SearchSelectedEntities = new HashSet<Guid>();
        }

        private int Save()
        {
            foreach (var userId in SearchSelectedEntities)
            {
                var commands = new List<Command>();

                var journalId = GetOrCreateJournalId(userId, commands);

                var experienceId = UniqueIdentifier.Create();
                commands.Add(new AddExperience(journalId, experienceId));

                Fields.GetChanges(JournalSetupIdentifier, journalId, experienceId, commands);

                ServiceLocator.SendCommands(commands);
            }

            return SearchSelectedEntities.Count;
        }

        private Guid GetOrCreateJournalId(Guid userId, List<Command> commands)
        {
            var journal = ServiceLocator.JournalSearch.GetJournal(JournalSetupIdentifier, userId);
            if (journal != null)
                return journal.JournalIdentifier;

            if (ServiceLocator.JournalSearch.GetJournalSetupUser(JournalSetupIdentifier, userId, JournalSetupUserRole.Learner) == null)
                commands.Add(new AddJournalSetupUser(JournalSetupIdentifier, userId, JournalSetupUserRole.Learner));

            var journalId = UniqueIdentifier.Create();
            commands.Add(new CreateJournal(journalId, JournalSetupIdentifier, userId));

            return journalId;
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var count = ServiceLocator.PersonSearch.CountPersons(GetPersonFilter());

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

        private QPersonFilter GetPersonFilter()
        {
            var filter = new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
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
