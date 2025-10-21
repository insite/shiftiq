using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class AddStudent : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private string ReturnUrl => Request.QueryString["return"];

        private bool UserCreated => Request["userCreated"] == "1";
        private Guid[] SavedIdentifiers
        {
            get => (Guid[])ViewState[nameof(SavedIdentifiers)];
            set => ViewState[nameof(SavedIdentifiers)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            SaveButton.Click += SaveButton_Click;

            SavedUsersRepeater.DataBinding += SavedUsersRepeater_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SetControlsVisibility();

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event);
            if (gradebook == null || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            if (gradebook.IsLocked)
                HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}");

            var title = gradebook.GradebookTitle;

            if (gradebook.Event != null)
                title += $" <span class='form-text'> for {gradebook.Event.EventTitle} ({GetLocalTime(gradebook.Event.EventScheduledStart)} - {GetLocalTime(gradebook.Event.EventScheduledEnd)})</span>";

            PageHelper.AutoBindHeader(this, null, title);

            RegistrationEventIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            RegistrationEventIdentifier.Filter.EventType = "Class";
            RegistrationEventIdentifier.Value = gradebook.EventIdentifier;

            CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=scores";

            if (UserCreated && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                CreateControl.SavedIdentifiers = null;

                SearchCriteriaSection.Visible = false;
                SearchResultsSection.Visible = false;
                SavedUsersSection.Visible = true;

                SaveButton.Visible = true;

                SavedUsersRepeater.DataBind();
            }
            else
            {
                var returnUrl = $"/ui/admin/records/gradebooks/add-learner?gradebook={GradebookIdentifier}";
                if (!string.IsNullOrEmpty(ReturnUrl))
                    returnUrl += $"&return={ReturnUrl}";

                var returnUrlEncoded = HttpUtility.UrlEncode(returnUrl);

                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrlEncoded}&gradebook={GradebookIdentifier}&action=gradebook_add_students";
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrlEncoded}&gradebook={GradebookIdentifier}&action=gradebook_add_students";

                if (gradebook.EventIdentifier.HasValue)
                    Search();
            }
        }

        private void SetControlsVisibility()
        {
            CreateContactLink.Visible = Identity.IsGranted(ActionName.Admin_Records_Gradebooks_AddStudent_NewContact);
            UploadContactLink.Visible = Identity.IsGranted(ActionName.Admin_Records_Gradebooks_AddStudent_UploadContact);
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            StudentGrid.SelectedContacts.Clear();

            Search();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            FullName.Text = null;
            Email.Text = null;
            RegistrationEventIdentifier.Value = null;

            StudentGrid.SelectedContacts.Clear();

            SearchResultsSection.Visible = false;

            SearchResultsSection.SetTitle("Students", 0);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (SaveSelectedPersons())
                RedirectToGradebook();
        }

        private bool SaveSelectedPersons()
        {
            HashSet<Guid> contacts;

            if (SavedIdentifiers != null)
            {
                contacts = new HashSet<Guid>();

                foreach (RepeaterItem item in SavedUsersRepeater.Items)
                {
                    var isSelectedCheckBox = (ICheckBoxControl)item.FindControl("IsSelected");
                    if (isSelectedCheckBox.Checked)
                    {
                        contacts.Add(SavedIdentifiers[item.ItemIndex]);
                    }
                }
            }
            else
                contacts = StudentGrid.SelectedContacts;

            if (contacts.Count == 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return false;
            }

            foreach (var learner in contacts)
                InSite.Application.Courses.Read.CourseObjectChangeProcessor.EnsureEnrollment(ServiceLocator.SendCommand, ServiceLocator.RecordSearch, GradebookIdentifier, learner, DateTimeOffset.UtcNow);

            return true;
        }

        private void SavedUsersRepeater_DataBinding(object sender, EventArgs e)
        {
            var users = PersonCriteria.Bind(
                x => new
                {
                    x.UserIdentifier,
                    x.User.FullName,
                    x.PersonCode,
                    x.User.Email,
                    x.User.EmailAlternate,
                    EmployerName = x.EmployerGroup.GroupName,
                },
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    IncludeUserIdentifiers = SavedIdentifiers,
                    OrderBy = "FullName"
                });

            SavedUsersSection.SetTitle("People", users.Length);

            SavedUsersRepeater.DataSource = users;
        }

        private void RedirectToGradebook()
        {
            var url = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=scores";
            HttpResponseHelper.Redirect(url);
        }

        private void Search()
        {
            StudentGrid.RegistrationEventIdentifier = RegistrationEventIdentifier.Value;
            StudentGrid.Search(GetFilter(), true);

            SearchResultsSection.SetTitle("Students", StudentGrid.RowCount);
            SearchResultsSection.Visible = true;
            SearchResultsSection.IsSelected = true;

            SaveButton.Visible = StudentGrid.RowCount > 0;

            SearchResultsSection.Visible = true;
            SearchResultsSection.IsSelected = true;
        }

        private QUserFilter GetFilter()
        {
            return new QUserFilter
            {
                OrganizationIdentifiers = new[] { Organization.OrganizationIdentifier },
                FullName = FullName.Text,
                NameFilterType = "Exact",
                EmailOrAlternate = Email.Text,
                RegistrationEventIdentifier = RegistrationEventIdentifier.Value,
                ExcludeGradebookIdentifier = GradebookIdentifier
            };
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=scores"
                : null;
        }
    }
}
