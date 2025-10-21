using System;
using System.Linq;

using InSite.Common;
using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Events.Classes.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class RegisterEmployee : PortalBasePage
    {
        protected Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

        protected bool IsFull
        {
            get => (bool)ViewState[nameof(IsFull)];
            set => ViewState[nameof(IsFull)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NextButton.Click += NextButton_Click;
            SearchButton.Click += SearchButton_Click;
            PersonCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Organization.Toolkits.Events?.AllowUsersRegisterEmployees != true)
                NavigateToOutline();

            if (!IsPostBack)
                LoadData();

            RenderBreadcrumb($"?event={EventIdentifier}");
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            BindOrganizationSettingsToControls();

            if (EventIdentifier == null
                || TGroupPermissionSearch.IsAccessDenied(EventIdentifier.Value, Identity)
                )
            {
                NavigateToSearch();
            }

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations, x => x.Achievement);

            var checkResult = @event != null ? RegistrationHelper.CheckClass(@event) : CheckClassResult.ClassClosed;

            if (@event == null
                || @event.OrganizationIdentifier != Organization.OrganizationIdentifier
                || checkResult != CheckClassResult.ClassOpen && checkResult != CheckClassResult.ClassFull
                )
            {
                NavigateToOutline();
            }

            var title = $"{@event.EventTitle}";
            var subtitle = $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";

            if (@event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date)
                subtitle += $" - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PageTitle.InnerText = title;
            PageSubtitle.InnerHtml = subtitle;

            var requirement = Organization.Toolkits.Events.RegisterEmployeesSearchRequirement;

            if (requirement == RegisterEmployeesSearchRequirement.NameAndEmailOrCode)
            {
                PersonCodeColumn.Visible = true;
                FieldsRequiredText.Text = "Your search must include last name and either email or tradeworker #";
            }
            else if (requirement == RegisterEmployeesSearchRequirement.NameAndEmail)
            {
                PersonCodeColumn.Visible = false;
                FieldsRequiredText.Text = "Your search must include last name and email";
            }
            else if (requirement == RegisterEmployeesSearchRequirement.Email)
            {
                PersonCodeColumn.Visible = false;
                PersonNameColumn.Visible = false;
                FieldsRequiredText.Text = "Your search must include email";
            }
            else
            {
                PersonCodeColumn.Visible = false;
                FieldsRequiredText.Text = "Your search must include last name or email";
            }

            SearchHint.Text = FieldsRequiredText.Text;

            IsFull = checkResult == CheckClassResult.ClassFull;

            if (!IsFull
                && @event.AllowMultipleRegistrations
                && RegisterLinkIsVisible()
                )
            {
                MainPanel.Visible = false;
                CloseButton2.Visible = false;
            }
            else
                ShowOneEmployeePanel();

            RegisterLink.NavigateUrl = IsFull
                ? $"/ui/portal/events/classes/add-to-waiting-list?event={EventIdentifier}"
                : $"/ui/portal/events/classes/register?event={EventIdentifier}&registeremployee=1";

            CloseButton1.NavigateUrl =
            CloseButton2.NavigateUrl = $"/ui/portal/events/classes/outline?event={EventIdentifier}";
        }

        public static bool RegisterLinkIsVisible()
        {
            return (Identity.Organization.Toolkits.Events?.AllowUserAccountCreationDuringRegistration) ?? false;
        }

        private void BindOrganizationSettingsToControls()
        {
            RegisterLink.Visible = RegisterLinkIsVisible();
            RegistrationProcessMultiple.Visible = RegisterLink.Visible;
        }

        private void ShowOneEmployeePanel()
        {
            ChooseModePanel.Visible = false;
            MainPanel.Visible = true;
            CloseButton2.Visible = true;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (RegistrationProcessMultiple.Checked)
            {
                var url = $"/ui/portal/events/classes/register-multiple?event={EventIdentifier}";
                HttpResponseHelper.Redirect(url);
            }

            ShowOneEmployeePanel();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            var requirement = Organization.Toolkits.Events.RegisterEmployeesSearchRequirement;

            var hasCriteriaCode = !string.IsNullOrEmpty(PersonCode.Text);
            var hasCriteriaEmail = !string.IsNullOrEmpty(Email.Text);
            var hasCriteriaName = !string.IsNullOrEmpty(LastName.Text);

            if (!IsSearchRequirementSatisfied(requirement, hasCriteriaCode, hasCriteriaEmail, hasCriteriaName))
            {
                FieldsRequired.Visible = true;
                RequirementFailure1.Visible = false;
                RequirementFailure2.Visible = false;
                RequirementFailure3.Visible = false;
                PersonPanel.Visible = false;
                return;
            }

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);

            var users = new User[0];

            if (requirement == RegisterEmployeesSearchRequirement.NameAndEmailOrCode && hasCriteriaCode)
                users = FindUsersByPersonCode();
            else if (requirement == RegisterEmployeesSearchRequirement.NameOrEmail)
                users = FindUsersByNameOrEmail();
            else if (requirement == RegisterEmployeesSearchRequirement.Email)
                users = FindUsersByEmail();
            else
                users = FindUsersByNameAndEmail();

            var data = users
                .Select(x =>
                {
                    var registration = @event
                        .Registrations
                        .FirstOrDefault(y => y.CandidateIdentifier == x.UserIdentifier);

                    return new
                    {
                        x.UserIdentifier,
                        x.FullName,
                        x.Email,
                        Status = registration?.ApprovalStatus
                    };
                })
                .ToList();

            FieldsRequired.Visible = false;

            bool hasMatches = data.Count > 0;

            RequirementFailure1.Visible = !hasMatches && requirement == RegisterEmployeesSearchRequirement.NameAndEmail;
            RequirementFailure2.Visible = !hasMatches && requirement == RegisterEmployeesSearchRequirement.NameAndEmailOrCode;
            RequirementFailure3.Visible = !hasMatches && requirement == RegisterEmployeesSearchRequirement.NameOrEmail;
            RequirementFailure4.Visible = !hasMatches && requirement == RegisterEmployeesSearchRequirement.Email;

            PersonPanel.Visible = hasMatches;

            if (hasMatches)
            {
                PersonRepeater.DataSource = data;
                PersonRepeater.DataBind();
            }
        }

        private bool IsSearchRequirementSatisfied(
            RegisterEmployeesSearchRequirement requirement,
            bool hasCriteriaCode, bool hasCriteriaEmail, bool hasCriteriaName)
        {
            switch (requirement)
            {
                case RegisterEmployeesSearchRequirement.NameAndEmail:
                    return hasCriteriaName && hasCriteriaEmail;

                case RegisterEmployeesSearchRequirement.NameAndEmailOrCode:
                    return hasCriteriaName && (hasCriteriaEmail || hasCriteriaCode);

                case RegisterEmployeesSearchRequirement.NameOrEmail:
                    return hasCriteriaName || hasCriteriaEmail;

                case RegisterEmployeesSearchRequirement.Email:
                    return hasCriteriaEmail;

                default:
                    return false;
            }
        }

        private User[] FindUsersByPersonCode()
        {
            return PersonCriteria.Bind(x => x.User, new PersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CodeExact = PersonCode.Text,
                LastName = LastName.Text,
                NameFilterType = "ExactName",
                OrderBy = "FullName"
            });
        }

        private User[] FindUsersByNameAndEmail()
        {
            return PersonCriteria.Bind(x => x.User, new PersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                EmailExact = Email.Text,
                LastName = LastName.Text,
                NameFilterType = "ExactName",
                OrderBy = "FullName"
            });
        }

        private User[] FindUsersByEmail()
        {
            return PersonCriteria.Bind(x => x.User, new PersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                EmailContains = Email.Text,
                OrderBy = "FullName"
            });
        }

        private User[] FindUsersByNameOrEmail()
        {
            return PersonCriteria.Bind(x => x.User, new PersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                EmailExact = Email.Text,
                LastName = LastName.Text,
                NameFilterType = "Exact",
                OrderBy = "FullName"
            });
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/portal/events/classes/search", true);

        private void NavigateToOutline()
            => HttpResponseHelper.Redirect($"/ui/portal/events/classes/outline?event={EventIdentifier}", true);

        protected static string GetEmptyMessage(string text)
            => LabelHelper.GetLabelContentText(text);
    }
}