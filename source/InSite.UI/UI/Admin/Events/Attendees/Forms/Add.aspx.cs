using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Events.Attendees.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/events/exams/search";

        #endregion

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request.QueryString["event"], out Guid value) ? value : (Guid?)null;

        private string OutlineUrl =>
            $"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=contacts";

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

        private Guid[] _attendeedUsers;
        private Guid[] AttendeedUsers
        {
            get
            {
                if (_attendeedUsers == null)
                    _attendeedUsers = ServiceLocator.EventSearch
                        .GetAttendees(EventIdentifier.Value)
                        .Select(x => x.UserIdentifier)
                        .ToArray();

                return _attendeedUsers;
            }
        }
        #endregion

        #region Fields

        private DateTime? _processStartedOn;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

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

                CriteriaRoleSelector.ListFilter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                CriteriaRoleSelector.ListFilter.GroupType = GroupTypes.Team;
                CriteriaRoleSelector.ListFilter.GroupLabel = GroupTypes.Role;

                CriteriaEnableRoleFilter.ValueAsBoolean = false;

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
                typeof(Add),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

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
            if (!Page.IsValid)
                return;

            if (SearchSelectedUsers.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            if (Save())
                HttpResponseHelper.Redirect(OutlineUrl);
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value)
                : null;
            if (@event == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedUsers = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;
            }
            else
            {
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Events_Attendees_Add_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Events_Attendees_Add_UploadContact);
                var returnUrl = HttpUtility.UrlEncode($"/ui/admin/events/attendees/add?event={EventIdentifier}");

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&event={EventIdentifier}&action=attendees_add";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&event={EventIdentifier}&action=attendees_add";

                CriteriaClear();
            }

            Search();

            SearchResultCloseButton.NavigateUrl = OutlineUrl;
        }

        private void CriteriaClear()
        {
            CriteriaRoleSelector.ValueAsGuid = null;
            CriteriaRoleSelectorView.IsActive = true;
            CriteriaRoleText.Text = null;

            CriteriaContactName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaContactCode.Text = null;

            SearchSelectedUsers = new HashSet<Guid>();
        }

        #endregion

        #region Methods (save)

        private bool Save()
        {
            var role = !CriteriaRoleSelectorView.IsActive
                ? CriteriaRoleText.Text
                : CriteriaRoleSelector.ValueAsGuid.HasValue
                    ? ServiceLocator.GroupSearch.GetGroup(CriteriaRoleSelector.ValueAsGuid.Value)?.GroupName
                    : null;

            if (string.IsNullOrWhiteSpace(role))
                role = "Role Not Defined";

            ProgressCallback("Prepare data", 0, 1, false);

            if (SearchSelectedUsers.Count == 0)
                return true;

            var problems = FindProblems(role, SearchSelectedUsers);
            if (problems.IsNotEmpty())
            {
                ScreenStatus.AddMessage(AlertType.Error, string.Join("<br />", problems));
                return false;
            }

            var contactsForSave = new HashSet<Guid>();

            foreach (var contact in SearchSelectedUsers)
                AddContactForSave(contact);

            try
            {
                var progressMax = contactsForSave.Count;
                var progressIndex = 0;
                var validate = !Identity.IsOperator;

                foreach (var contact in contactsForSave)
                {
                    ServiceLocator.SendCommand(new AddEventAttendee(EventIdentifier.Value, contact, role, validate));

                    ProgressCallback("Adding contact", ++progressIndex, progressMax, true);
                }
            }
            catch (ExamCandidateNotAllowedException ex)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    $"You cannot add an Exam Candidate to this event after {ex.Cutoff.Format(User.TimeZone)}.");
            }

            return true;

            void AddContactForSave(Guid id)
            {
                if (!contactsForSave.Contains(id))
                    contactsForSave.Add(id);
            }
        }

        private List<string> FindProblems(string role, ICollection<Guid> newContactIdentifiers)
        {
            if (!string.Equals(role, "Exam Candidate", StringComparison.OrdinalIgnoreCase))
                return null;

            var list = new List<string>();

            var newContacts = PersonCriteria.Bind(
                x => new { x.PersonCode, x.User.Email, x.User.FullName },
                new PersonFilter { IncludeUserIdentifiers = newContactIdentifiers.ToArray() });

            var existingContacts = ServiceLocator.EventSearch.GetAttendees(EventIdentifier.Value, x => x.Person);

            foreach (var newContact in newContacts)
            {
                if (newContact.PersonCode.IsEmpty())
                {
                    list.Add($"You cannot add an Exam Candidate without a unique Contact Code ({newContact.FullName}).");
                    continue;
                }

                var existingContact = existingContacts.FirstOrDefault(x => x.Person.PersonCode == newContact.PersonCode);
                if (existingContact != null)
                    list.Add($"You cannot add two different Exam Candidates ({existingContact.Person.UserFullName} " +
                        $"and {newContact.FullName}) with the same Contact Code ({newContact.PersonCode}).");
            }

            return list;
        }

        private void ProgressCallback(string status, int currentPosition, int positionMax, bool enableTimeRemaining)
        {
            SaveProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;

                context.Variables["status"] = status;

                if (enableTimeRemaining)
                {
                    if (!_processStartedOn.HasValue)
                        _processStartedOn = DateTime.UtcNow;

                    context.Variables["time_remaining"] = string.Format(
                        "{0:hh}:{0:mm}:{0:ss}s",
                        Clock.TimeRemaining(positionMax, currentPosition, _processStartedOn.Value));
                }
                else
                {
                    context.Variables["time_remaining"] = "00:00:00s";
                }
            });
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

            ValidateContactCodes();
        }

        private void ValidateContactCodes()
        {
            var codes = GetCriteriaContactCodes();
            if (codes.Length == 0)
                return;

            var foundCodes = new HashSet<string>(
                ServiceLocator.PersonSearch.GetPersonCodes(Organization.OrganizationIdentifier, codes),
                StringComparer.OrdinalIgnoreCase);

            var notFoundCodes = codes.Where(x => !foundCodes.Contains(x));
            if (notFoundCodes.Any())
                ScreenStatus.AddMessage(AlertType.Warning, "Contacts not found in your search results: " + string.Join(", ", notFoundCodes));
        }

        private object GetCriteriaResultData()
        {
            var filter = CreateFilter();
            filter.OrderBy = "User.FullName,User.Email";
            filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User.Memberships.Select(y => y.Group)).Select(x => new
            {
                UserIdentifier = x.User.UserIdentifier,
                FullName = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                PersonCode = x.PersonCode,
                Roles = x.User.Memberships
                    .Where(y => y.Group != null && y.Group.OrganizationIdentifier == Organization.OrganizationIdentifier && y.Group.GroupType == GroupTypes.Team)
                    .Select(y => y.Group.GroupName)
                    .ToList()
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
            return new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifiers = SavedIdentifiers,
                ExcludeUserIdentifiers = AttendeedUsers,
                UserMembershipGroupIdentifier = CriteriaEnableRoleFilter.ValueAsBoolean.Value ? CriteriaRoleSelector.ValueAsGuid : null,
                UserNameContains = CriteriaContactName.Text,
                UserEmailContains = CriteriaEmail.Text,
                PersonCodes = GetCriteriaContactCodes()
            };
        }

        #endregion

        #region Helpers

        private string[] GetCriteriaContactCodes()
        {
            return CriteriaContactCode.Text.IsEmpty()
                ? new string[0]
                : StringHelper.Split(CriteriaContactCode.Text);
        }

        protected static string GetRoles(object dataItem)
        {
            var roles = (List<string>)DataBinder.Eval(dataItem, "Roles");
            if (roles.Count == 0)
                return string.Empty;

            roles.Sort();

            return "<small><ul class='ps-3 my-0'><li class='my-0'>" + string.Join("</li><li class='mb-0 mt-1'>", roles) + "</li></ul></small>";
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/exams/outline")
                ? $"event={EventIdentifier}&panel=contacts"
                : null;
        }

        #endregion
    }
}