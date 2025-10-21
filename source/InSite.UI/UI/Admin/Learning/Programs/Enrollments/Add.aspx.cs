using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Forms;
using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Records.Programs
{
    public partial class AddUser : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/learning/programs/search";

        #endregion

        #region Properties

        private Guid? ProgramIdentifier => Guid.TryParse(Request["program"], out var id) ? id : (Guid?)null;

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

        private string ReturnUrl
        {
            get => (string)ViewState[nameof(ReturnUrl)];
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        private VProgramEnrollment[] ProgramUsers
        {
            get => (VProgramEnrollment[])ViewState[nameof(ProgramUsers)];
            set => ViewState[nameof(ProgramUsers)] = value;
        }

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
                typeof(CreateUserConnection),
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
            if (SearchSelectedUsers.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            var program = ProgramSearch.GetProgram(ProgramIdentifier.Value);
            if (program == null)
                return;

            foreach (var userIdentifier in SearchSelectedUsers)
                SaveUser(program, userIdentifier);

            HttpResponseHelper.Redirect(ReturnUrl);
        }

        private void SaveUser(TProgram program, Guid userIdentifier)
        {
            var tasks = TaskStore.EnrollUserToProgramTasks(Organization.Identifier, program.ProgramIdentifier, userIdentifier);

            if (tasks != null && tasks.Length > 0)
            {
                EnsureCourseEnrollment(userIdentifier, tasks);
                EnsureLogbookEnrollment(userIdentifier, tasks);
            }

            ServiceLocator.ProgramService.CompletionOfProgramAchievement(program.ProgramIdentifier, userIdentifier, program.OrganizationIdentifier);
        }

        #endregion

        #region Methods (open/save)

        private void Open()
        {
            var program = ProgramIdentifier.HasValue ? ProgramSearch.GetProgram(ProgramIdentifier.Value) : null;
            if (program == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: program.ProgramName);

            var returnValue = Request.QueryString["return"];

            ReturnUrl = returnValue.IfNullOrEmpty($"/ui/admin/learning/programs/outline?id={ProgramIdentifier}&panel=users");

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedUsers = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;
            }
            else
            {
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Records_Programs_AddUser_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Records_Programs_AddUser_UploadContact);

                var returnUrl = $"/ui/admin/learning/programs/enrollments/add?program={ProgramIdentifier}";
                if (returnValue.IsNotEmpty())
                    returnUrl += $"&return={returnValue}";

                returnUrl = HttpUtility.UrlEncode(returnUrl);

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&achievement={ProgramIdentifier}&action=programs_addpeople";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&achievement={ProgramIdentifier}&action=programs_addpeople";

                CriteriaClear();
            }

            ProgramUsers = ProgramSearch1
                .GetProgramUsers(new VProgramEnrollmentFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ProgramIdentifier = ProgramIdentifier.Value
                })
                .ToArray();

            Search();

            SearchResultCloseButton.NavigateUrl = ReturnUrl;
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
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                PersonCode = x.PersonCode,
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
                UserNameContains = CriteriaName.Text,
                UserEmailContains = CriteriaEmail.Text,
            };
        }

        #endregion

        #region Helpers

        private static void EnsureLogbookEnrollment(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Logbook"))
                ProgramHelper.EnsureLogbookEnrollment(userIdentifier, task.ObjectIdentifier);
        }

        private static void EnsureCourseEnrollment(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Course"))
                ProgramHelper.EnsureCourseEnrollment(userIdentifier, task.ObjectIdentifier);
        }

        protected string GetRoles(string name)
        {
            return "Learner";
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={Request.QueryString["program"]}&panel=users"
                : null;
        }

        #endregion
    }
}