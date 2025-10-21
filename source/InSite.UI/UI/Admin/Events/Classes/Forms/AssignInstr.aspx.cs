using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class AssignInstr : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/events/classes/search";

        #endregion

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=class";

        private Guid[] _registeredInstructors;
        private Guid[] RegisteredInstructors
        {
            get
            {
                if (_registeredInstructors == null)
                {
                    _registeredInstructors = ServiceLocator.EventSearch
                        .GetAttendees(EventIdentifier.Value, x => x.Person)
                        .Where(x => x.AttendeeRole == "Instructor")
                        .Select(x => x.UserIdentifier)
                        .ToArray();
                }

                return _registeredInstructors;
            }
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

            CriteriaGroupIdentifier.ItemsRequested += CriteriaGroupIdentifier_ItemsRequested;

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

            var selectedCount = SearchSelectedUsers != null ? SearchSelectedUsers.Count : 0;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(AssignInstr),
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

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "UserIdentifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedUsers.Contains(id);

            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedUsers.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            var addInstructors = SearchSelectedUsers.Except(RegisteredInstructors);

            foreach (var id in addInstructors)
                ServiceLocator.SendCommand(new AddEventAttendee(EventIdentifier.Value, id, "Instructor", false));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value)
                : null;
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedUsers = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;
            }
            else
            {
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Events_Classes_AssignInstructor_NewContact); ;
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Events_Classes_AssignInstructor_UploadContact);
                var returnUrl = HttpUtility.UrlEncode($"/ui/admin/events/classes/assign-instructor?event={EventIdentifier}");

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&event={EventIdentifier}&action=classes_assigninstr";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&event={EventIdentifier}&action=classes_assigninstr";

                CriteriaClear();
            }

            Search();

            SearchResultCloseButton.NavigateUrl = OutlineUrl;
        }

        private void CriteriaClear()
        {
            CriteriaContactName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaGroupType.Value = null;
            CriteriaGroupLabel.Text = null;
            CriteriaGroupIdentifier.Value = null;

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

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User, x => x.EmployerGroup).Select(x => new
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                PersonCode = x.PersonCode,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                EmployerIdentifier = x.EmployerGroup?.GroupIdentifier,
                EmployerName = x.EmployerGroup?.GroupName,
                EmployerOrganizationIdentifier = x.EmployerGroup?.OrganizationIdentifier
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
            var filter = new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifiers = SavedIdentifiers,
                ExcludeUserIdentifiers = RegisteredInstructors,
                UserNameContains = CriteriaContactName.Text,
                UserEmailContains = CriteriaEmail.Text,
                UserMembershipGroupLabelContains = CriteriaGroupLabel.Text
            };

            if (CriteriaGroupType.Value.IsNotEmpty())
            {
                if (CriteriaGroupIdentifier.HasValue)
                    filter.UserMembershipGroupIdentifier = CriteriaGroupIdentifier.Value.Value;
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
                ? $"event={Request.QueryString["event"]}"
                : null;
        }

        #endregion
    }
}