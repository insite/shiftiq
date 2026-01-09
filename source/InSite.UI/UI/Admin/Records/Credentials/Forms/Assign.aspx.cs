using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Achievements.Credentials.Forms
{
    public partial class Assign : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? AchievementIdentifier => Guid.TryParse(Request["achievement"], out var id) ? id : (Guid?)null;

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

            NextButton.Click += NextButton_Click;
            NextSavedUsersButton.Click += NextButton_Click;

            CredentialStatus.AutoPostBack = true;
            CredentialStatus.SelectedIndexChanged += CredentialStatus_SelectedIndexChanged;

            SaveButton.Click += SaveButton_Click;
            SavedUsersRepeater.DataBinding += SavedUsersRepeater_DataBinding;

            if (ServiceLocator.Partition.IsE03())
            {
                Necessity.Settings.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
                Priority.Settings.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
            }
            else
            {
                Necessity.Settings.UseCurrentOrganization = true;
                Priority.Settings.UseCurrentOrganization = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var achievement = AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value) : null;
            var organization = achievement != null ? OrganizationSearch.Select(achievement.OrganizationIdentifier) : null;

            if (achievement == null
                    || (
                        achievement.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier
                        && !ServiceLocator.Partition.IsE03()
                    )
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/achievements/search");
                return;
            }

            PageHelper.AutoBindHeader(this, null, achievement.AchievementTitle);

            // Set defaults

            Assigned.Value = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);

            if (UserCreated && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                CreateControl.SavedIdentifiers = null;

                SearchCriteriaSection.Visible = false;
                SearchResultsSection.Visible = false;
                SavedUsersSection.Visible = true;

                SavedUsersRepeater.DataBind();
            }
            else
            {
                var returnUrl = $"/ui/admin/records/credentials/assign?achievement={AchievementIdentifier}";
                if (!string.IsNullOrEmpty(ReturnUrl))
                    returnUrl += $"&return={ReturnUrl}";

                var returnUrlEncoded = HttpUtility.UrlEncode(returnUrl);

                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrlEncoded}&achievement={AchievementIdentifier}&action=credentials_assign";
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrlEncoded}&achievement={AchievementIdentifier}&action=credentials_assign";
            }

            CancelButton1.NavigateUrl = CancelButton2.NavigateUrl = CancelSavedUsersButton.NavigateUrl = GetAchievementUrl();

            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            CreateContactLink.Visible = Identity.IsGranted(ActionName.Admin_Records_Credentials_Assign_NewContact);
            UploadContactLink.Visible = Identity.IsGranted(ActionName.Admin_Records_Credentials_Assign_UploadContact);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            AssignGrid.SelectedContacts.Clear();

            Search();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ContactName.Text = null;
            Email.Text = null;

            AssignGrid.SelectedContacts.Clear();

            Search();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            CredentialPanel.Visible = true;
            CredentialPanel.IsSelected = true;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value);

            ExpirationField.SetExpiration(achievement);
        }

        private void CredentialStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GrantedDateField.Visible = CredentialStatus.SelectedValue == "Granted";
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ICollection<Guid> contacts;

            if (SavedIdentifiers != null)
            {
                contacts = new List<Guid>();

                foreach (RepeaterItem item in SavedUsersRepeater.Items)
                {
                    var isSelectedCheckBox = (ICheckBoxControl)item.FindControl("IsSelected");
                    if (isSelectedCheckBox.Checked)
                    {
                        var user = SavedIdentifiers[item.ItemIndex];
                        contacts.Add(user);
                    }
                }
            }
            else
                contacts = AssignGrid.SelectedContacts;

            if (contacts.Count == 0)
            {
                CreatorStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            SaveChanges(contacts);

            RedirectToAchievement();
        }

        private void Search()
        {
            var count = AssignGrid.LoadData(ContactName.Text, Email.Text);

            SearchResultsSection.SetTitle("People", count);

            SearchResultsSection.Visible = true;
            SearchResultsSection.IsSelected = true;

            SaveButton.Visible = count > 0;
        }

        private void SaveChanges(IEnumerable<Guid> contacts)
        {
            var achievementIdentifier = AchievementIdentifier.Value;
            var expiration = ExpirationField.GetExpiration();
            var commands = new List<Command>();

            foreach (var userIdentifier in contacts)
            {
                var credential = ServiceLocator.AchievementSearch.GetCredential(AchievementIdentifier.Value, userIdentifier);
                var id = ServiceLocator.AchievementSearch.GetCredentialIdentifier(credential?.CredentialIdentifier, AchievementIdentifier.Value, userIdentifier);

                if (credential == null)
                    commands.Add(new CreateCredential(id, Organization.OrganizationIdentifier, achievementIdentifier, userIdentifier, Assigned.Value));

                if (credential == null || IsExpirationChanged(expiration, credential))
                    commands.Add(new ChangeCredentialExpiration(id, expiration));

                if (credential == null || credential.CredentialNecessity != Necessity.Value || credential.CredentialPriority != Priority.Value)
                    commands.Add(new TagCredential(id, Necessity.Value, Priority.Value));

                if (GrantedDateField.Visible && GrantedDate.Value.HasValue)
                {
                    var person = ServiceLocator.ContactSearch.GetPerson(userIdentifier, Organization.OrganizationIdentifier);
                    commands.Add(new GrantCredential(id, GrantedDate.Value.Value, null, null, person?.EmployerGroupIdentifier, person?.EmployerGroupStatus));
                }
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        private bool IsExpirationChanged(Expiration a, VCredential credential)
        {
            var b = new Expiration(credential.CredentialExpirationType, credential.CredentialExpirationFixedDate, credential.CredentialExpirationLifetimeQuantity, credential.CredentialExpirationLifetimeUnit);
            return !a.Equals(b);
        }

        private void RedirectToAchievement()
        {
            HttpResponseHelper.Redirect(GetAchievementUrl());
        }

        private string GetAchievementUrl() =>
            !string.IsNullOrEmpty(ReturnUrl)
                ? ReturnUrl
                : $"/ui/admin/records/achievements/outline?id={AchievementIdentifier}&panel=credentials";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={Request.QueryString["achievement"]}&panel=credentials"
                : null;
        }

    }
}
