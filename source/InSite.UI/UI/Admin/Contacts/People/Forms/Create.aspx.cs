using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Contacts.People.Controls;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class Create : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Classes

        public class Contact
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public Guid? UserIdentifier { get; set; }
        }

        #endregion

        #region Constants

        private const string EditUrl = "/ui/admin/contacts/people/edit";
        private const string SearchUrl = "/ui/admin/contacts/people/search";
        public const string RelativePath = "/ui/admin/contacts/people/create";

        #endregion

        #region Properties

        public static Guid[] SavedIdentifiers
        {
            get => (Guid[])HttpContext.Current.Session[_savedIdentifiersKey];
            set => HttpContext.Current.Session[_savedIdentifiersKey] = value;
        }

        private static readonly string _savedIdentifiersKey = typeof(Create).FullName + "." + nameof(SavedIdentifiers);

        private string ReturnUrl => Request["return"];

        private List<Guid> RolesIdentifiers => (List<Guid>)(ViewState[nameof(RolesIdentifiers)]
            ?? (ViewState[nameof(RolesIdentifiers)] = new List<Guid>()));

        #endregion

        #region Inialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            OnePersonUniquePersonCode.ServerValidate += OnePersonUniquePersonCode_ServerValidate;

            OnePersonRolesRepeater.DataBinding += OnePersonRolesRepeater_DataBinding;
            OnePersonRolesRepeater.ItemDataBound += OnePersonRolesRepeater_ItemDataBound;

            OnePersonGenerateEmailButton.Click += OnePersonGenerateEmailButton_Click;

            OnePersonEmployerGroupIdentifier.AutoPostBack = true;
            OnePersonEmployerGroupIdentifier.ValueChanged += (s, a) => OnOnePersonEmployerSelected();

            OnePersonIsUserAccessGranted.AutoPostBack = true;
            OnePersonIsUserAccessGranted.CheckedChanged += (s, a) => OnOnePersonIsUserAccessGranted();

            OnePersonDuplicateOption.SelectedIndexChanged += (s, a) => OnOnePersonDuplicateOptionChanged();

            OnePersonDuplicateContinueButton.Click += OnePersonDuplicateContinueButton_Click;
            OnePersonDuplicateCloseButton.Click += (s, a) => OnePersonDuplicateWindow.Visible = false;

            MultiplePersonConfirmButton.Click += (s, a) => ConfirmMultiplePerson();
            MultiplePersonCancelButton.Click += (s, a) => MultiplePersonConfirmWindow.Visible = false;
            MultiplePersonCloseButton.Click += (s, a) => MultiplePersonConfirmWindow.Visible = false;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            Open();
        }

        #endregion

        #region Event handlers

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                CreateMultiView.SetActiveView(ViewOnePerson);
            else if (value == CreationTypeEnum.Bulk)
                CreateMultiView.SetActiveView(ViewMultiplePerson);
            else
                CreateMultiView.ActiveViewIndex = -1;
        }

        private void OnePersonUniquePersonCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Value))
                return;

            var persons = ServiceLocator.PersonSearch.GetPersonsByPersonCodes(new[] { args.Value }, Organization.Key);

            args.IsValid = persons.Count == 0;
        }

        private void OnePersonGenerateEmailButton_Click(object sender, EventArgs e)
        {
            var domain = ServiceLocator.AppSettings.Application.EmailDomain;
            OnePersonEmail.Text = UserSearch.CreateUniqueEmailsForOrganization(Organization.OrganizationCode, domain, 1)[0];
            OnePersonEmailDisabled.Checked = true;
        }

        private QGroup OnOnePersonEmployerSelected()
        {
            var employerId = OnePersonEmployerGroupIdentifier.Value;
            var employer = employerId.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(employerId.Value)
                : null;

            var status = TCollectionItemCache.GetName(employer?.GroupStatusItemIdentifier);

            OnePersonEmployerBadge.Visible = status.IsNotEmpty();
            OnePersonEmployerBadge.InnerText = status;

            return employer;
        }

        private void OnOnePersonIsUserAccessGranted()
        {
            var isAccessGranted = OnePersonIsUserAccessGranted.Checked;

            OnePersonSignInFieldGroup.Visible = isAccessGranted;

            if (isAccessGranted)
                OnePersonPassword.Text = RandomStringGenerator.CreateUserPassword();
        }

        private void OnePersonRolesRepeater_DataBinding(object sender, EventArgs e)
        {
            RolesIdentifiers.Clear();
        }

        private void OnePersonRolesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (IsContentItem(e))
                RolesIdentifiers.Add(((QGroup)e.Item.DataItem).GroupIdentifier);
        }

        private void OnOnePersonDuplicateOptionChanged()
        {
            var value = OnePersonDuplicateOption.SelectedValue;

            OnePersonDuplicateCancelDescription.Visible = value == "Cancel";
            OnePersonDuplicateConnectDescription.Visible = value == "Connect";
            OnePersonDuplicateCreateDescription.Visible = value == "Create";
            OnePersonDuplicateContinueButton.Visible = value.IsNotEmpty();
        }

        private void OnePersonDuplicateContinueButton_Click(object sender, EventArgs e)
        {
            switch (OnePersonDuplicateOption.SelectedValue)
            {
                case "Cancel":
                    HttpResponseHelper.Redirect(RelativePath);
                    break;
                case "Connect":
                    ConnectOnePerson();
                    break;
                case "Create":
                    SaveOnePerson(true);
                    break;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        #endregion

        #region Methods (save/load)

        private void Open()
        {
            SetNotification(ScreenNotification);
            SetDefaultInputValues();

            CreationTypePanel.Visible = true;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Bulk);

            var allowGenerateEmail = CurrentSessionState.Identity.IsGranted(ActionName.Admin_Contacts_People_Email_Generate);

            OnePersonGenerateEmailButton.Visible = allowGenerateEmail;

            if (allowGenerateEmail)
                OnePersonEmail.Attributes["style"] = "width:calc(100% - 170px);";

            OnePersonEmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            OnCreationTypeSelected();

            CancelButton.NavigateUrl = ReturnUrl.IsNotEmpty()
                ? ReturnUrl
                : SearchUrl;
        }

        private void SetDefaultInputValues()
        {
            OnePersonTimeZone.Value = CurrentSessionState.Identity.Organization.TimeZone.Id;
            OnePersonCheckDuplicates.Checked = true;
            OnePersonIsUserAccessGranted.Checked = false;

            OnOnePersonIsUserAccessGranted();

            var filter = new QGroupFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, GroupType = GroupTypes.Role };
            var allGroups = ServiceLocator.GroupSearch.GetGroups(filter);

            var visibleGroups = MembershipPermissionHelper.CanModifyAdminMemberships()
                ? allGroups
                : allGroups.Where(x => !x.OnlyOperatorCanAddUser).ToList();

            OnePersonRolesRepeater.DataSource = visibleGroups;
            OnePersonRolesRepeater.DataBind();
        }

        public static void SetNotification(Alert alert)
        {
            var request = HttpContext.Current.Request;

            switch (request["action"])
            {
                case "credentials_assign":
                    AddAchievementNotification("You're creating person(s) to assign credential for {0}");
                    break;
                case "registrations_add":
                    AddEventNotification("You're creating person(s) for {0}");
                    break;
                case "classes_assigninstr":
                    AddEventNotification("You're creating Instructor(s) for {0}");
                    break;
                case "candidates_add":
                    AddEventNotification("You're creating candidate(s) for {0}");
                    break;
                case "attendees_add":
                    AddEventNotification("You're creating attendee(s) for {0}");
                    break;
                case "groups_createrole":
                    AddGroupNotification("You're creating person(s) that will be assigned to the {0} group");
                    break;
                case "people_createuserconnection":
                    AddUserNotification("You're creating person(s) to connect to {0}");
                    break;
                case "subscribers_add":
                    AddMessageNotification("You're creating person(s) as subscriber(s) of {0}");
                    break;
                case "progressions_add":
                    AddResourceNotification("You're creating people for {0} progression");
                    break;
                case "gradebook_add_students":
                    AddGradebookNotification("You're creating student(s) for {0} gradebook");
                    break;
                case "logbook_add_learners":
                    AddJournalSetupNotification("You're creating learner(s) for {0} logbook");
                    break;
                case "logbook_add_validators":
                    AddJournalSetupNotification("You're creating validator(s) for {0} logbook");
                    break;
            }

            #region Set Notification

            void AddAchievementNotification(string format)
            {
                var entity = Guid.TryParse(request["achievement"], out var identifier)
                    ? ServiceLocator.AchievementSearch.GetAchievement(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.AchievementTitle.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.AchievementTitle));
            }

            void AddEventNotification(string format)
            {
                var entity = Guid.TryParse(request["event"], out var identifier)
                    ? ServiceLocator.EventSearch.GetEvent(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.EventTitle.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.EventTitle));
            }

            void AddGroupNotification(string format)
            {
                var entity = Guid.TryParse(request["group"], out var identifier)
                    ? ServiceLocator.GroupSearch.GetGroup(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.GroupName.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.GroupName));
            }

            void AddUserNotification(string format)
            {
                var entity = Guid.TryParse(request["user"], out var identifier)
                    ? UserSearch.Bind(identifier, x => new
                    {
                        x.FullName,
                        IsPerson = x.Persons.Any(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                    })
                    : null;

                if (entity != null && entity.FullName.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.FullName));
            }

            void AddMessageNotification(string format)
            {
                var entity = Guid.TryParse(request["message"], out var identifier)
                    ? ServiceLocator.MessageSearch.GetMessage(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.MessageName.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.MessageName));
            }

            void AddResourceNotification(string format)
            {
                var title = Guid.TryParse(request["resource"], out var identifier)
                    ? ServiceLocator.ContentSearch.GetTitleText(identifier)
                    : null;

                if (title.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, title));
            }

            void AddGradebookNotification(string format)
            {
                var entity = Guid.TryParse(request["gradebook"], out var identifier)
                    ? ServiceLocator.RecordSearch.GetGradebook(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.GradebookTitle.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.GradebookTitle));
            }

            void AddJournalSetupNotification(string format)
            {
                var entity = Guid.TryParse(request["journalsetup"], out var identifier)
                    ? ServiceLocator.JournalSearch.GetJournalSetup(identifier)
                    : null;

                if (entity != null && entity.OrganizationIdentifier == Organization.OrganizationIdentifier && entity.JournalSetupName.IsNotEmpty())
                    alert.AddMessage(AlertType.Information, string.Format(format, entity.JournalSetupName));
            }

            #endregion
        }

        private void Save()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOnePerson(false);
            else if (value == CreationTypeEnum.Bulk)
                SaveMultiplePerson();
            else
                throw new NotImplementedException();
        }

        #endregion

        #region Methods (save one)

        private void SaveOnePerson(bool isDuplicateMode)
        {
            var user = UserFactory.Create();
            var person = UserFactory.CreatePerson(Organization.Identifier);

            GetOnePersonInputValues(user, person);

            var employer = person.EmployerGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(person.EmployerGroupIdentifier.Value)
                : null;
            if (employer != null)
            {
                person.Region = employer.GroupRegion;

                var employerAddress = Edit.GetEmployerAddress(employer.GroupIdentifier);
                if (employerAddress != null)
                    person.WorkAddress = employerAddress;
            }

            var isOrphan = ServiceLocator.UserSearch.IsOrphan(user.Email) ?? false;
            if (isOrphan)
            {
                person.UserIdentifier = UserSearch.BindFirst(x => x.UserIdentifier, new UserFilter { EmailExact = user.Email });
                user.UserIdentifier = person.UserIdentifier;
                PersonStore.Insert(person);
            }
            else if (!(isDuplicateMode || !OnePersonCheckDuplicates.Checked || SaveOnePersonWithCheck(user)))
            {
                return;
            }
            else if (UserSearch.IsEmailDuplicate(user.UserIdentifier, user.Email))
            {
                user.EmailAlternate = user.Email;
                person.EmailAlternateEnabled = true;
                user.Email = UserSearch.CreateUniqueEmailFromDuplicate(user.Email);
                person.EmailEnabled = false;
            }

            if (!isOrphan)
                UserStore.Insert(user, person);

            if (OnePersonIsUserAccessGranted.Checked)
            {
                PersonHelper.SendAccountCreated(Organization.OrganizationIdentifier, Organization.LegalName, user, person);
                PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);
            }

            AddUserToRoles(user.UserIdentifier);

            if (employer != null)
                EditTabDetails.AddEmployeeMembership(employer.GroupIdentifier, person.UserIdentifier);

            RedirectAfterSave(user.UserIdentifier);
        }

        private bool SaveOnePersonWithCheck(QUser person)
        {
            OnePersonDuplicateOption.SelectedIndex = -1;

            OnOnePersonDuplicateOptionChanged();

            if (UserSearch.IsEmailDuplicate(person.UserIdentifier, person.Email))
            {
                var anyPerson = PersonSearch.SelectByEmail(null, person.Email)
                    ?? throw new ArgumentNullException($"The user {person.Email} is not assigned to any organization.");

                var filter = new PersonFilter
                {
                    OrganizationIdentifier = anyPerson.OrganizationIdentifier,
                    EmailContains = person.Email
                };

                OnePersonDuplicateWindow.Visible = true;

                OnePersonDuplicateGrid.DisablePaging();
                OnePersonDuplicateGrid.LoadData(filter, new[] { "Name", "Email", "City" }, null);

                if (!OnePersonDuplicateGrid.HasRows)
                {
                    OnePersonDuplicateWindow.Visible = false;
                    return true;
                }

                OnePersonDuplicateMessage.Attributes["class"] = "alert alert-danger";
                OnePersonDuplicateMessage.InnerHtml = $"There is another existing user account with the same email address.";

                OnePersonDuplicateDuplicateEmail.Text = UserSearch.CreateUniqueEmailFromDuplicate(person.Email);
                OnePersonDuplicateDuplicateEmailAlternate.Text = person.Email;
            }
            else
            {
                var filter = new PersonFilter
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    NameFilterType = "Similar",
                    OrganizationIdentifier = Organization.OrganizationIdentifier
                };

                OnePersonDuplicateWindow.Visible = true;

                OnePersonDuplicateGrid.DisablePaging();
                OnePersonDuplicateGrid.LoadData(filter, new[] { "Name", "Email", "City" }, null);

                if (!OnePersonDuplicateGrid.HasRows)
                {
                    OnePersonDuplicateWindow.Visible = false;
                    return true;
                }

                filter.NameFilterType = "ExactName";
                var exactDuplicateCount = PersonCriteria.Count(filter);

                filter.NameFilterType = "Similar";
                var similarDuplicateCount = PersonCriteria.Count(filter);

                var text1 = exactDuplicateCount == 1
                    ? "is <strong>1</strong> person"
                    : $"are <strong>{exactDuplicateCount}</strong> people";

                var text2 = similarDuplicateCount == 1
                    ? "is <strong>1</strong> person"
                    : $"are <strong>{similarDuplicateCount}</strong> people";

                OnePersonDuplicateMessage.Attributes["class"] = "alert alert-warning";
                OnePersonDuplicateMessage.InnerHtml = $"There {text2} in the system who {(similarDuplicateCount == 1 ? "has" : "have")}"
                    + $" a name with similar spelling or pronounciation. Of these, there {text1} with the same spelling.";

                OnePersonDuplicateAddNewInstruction.Visible = exactDuplicateCount != 0;
            }

            return false;
        }

        private void ConnectOnePerson()
        {
            var user = UserSearch.SelectByEmail(OnePersonEmail.Text);
            if (user == null)
            {
                SaveOnePerson(true);
                return;
            }

            var person = UserFactory.CreatePerson(Organization.Identifier);

            GetOnePersonInputValues(new QUser(), person);

            person.UserIdentifier = user.UserIdentifier;

            PersonStore.Insert(person);

            AddUserToRoles(user.UserIdentifier);

            RedirectAfterSave(user.UserIdentifier);
        }

        private void GetOnePersonInputValues(QUser user, QPerson person)
        {
            user.FirstName = OnePersonFirstName.Text;
            user.MiddleName = OnePersonMiddleName.Text;
            user.LastName = OnePersonLastName.Text;
            user.Email = OnePersonEmail.Text;
            user.TimeZone = Organization.TimeZone.Id;
            person.EmailEnabled = true;

            var personCode = OnePersonCode.Text.NullIfWhiteSpace();
            if (personCode.IsNotEmpty())
                person.PersonCode = personCode;

            if (OnePersonIsUserAccessGranted.Checked)
            {
                user.SetPassword(OnePersonPassword.Text);
                user.SetDefaultPassword(OnePersonPassword.Text);
                person.UserAccessGranted = DateTimeOffset.UtcNow;
                person.UserAccessGrantedBy = User.FullName;
            }

            person.EmployerGroupIdentifier = OnePersonEmployerGroupIdentifier.Value;
            person.JobTitle = OnePersonJobTitle.Text;
            person.Phone = Shift.Common.Phone.Format(Phone.Text);
            person.Birthdate = OnePersonBirthdate.Value;
            person.Language = MultilingualString.DefaultLanguage;

            person.HomeAddress = new QPersonAddress();
            person.ShippingAddress = new QPersonAddress();
            person.BillingAddress = new QPersonAddress();
            person.WorkAddress = new QPersonAddress();
        }

        private void AddUserToRoles(Guid userIdentifier)
        {
            for (var i = 0; i < OnePersonRolesRepeater.Items.Count; i++)
            {
                var item = OnePersonRolesRepeater.Items[i];

                var checkbox = (ICheckBoxControl)item.FindControl("IsSelected");
                if (!checkbox.Checked || !MembershipPermissionHelper.CanModifyMembership(RolesIdentifiers[i]))
                    continue;

                var membership = new Membership
                {
                    UserIdentifier = userIdentifier,
                    GroupIdentifier = RolesIdentifiers[i],
                    Assigned = DateTimeOffset.UtcNow
                };

                MembershipHelper.Save(membership);
            }
        }

        #endregion

        #region Methods (save multiple)

        private void SaveMultiplePerson()
        {
            var contacts = GetContacts();
            if (contacts == null)
                return;

            var hasErrors = false;

            foreach (var contact in contacts)
            {
                if (string.IsNullOrWhiteSpace(contact.FirstName))
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"Missing First Name for {contact.Email}");
                    hasErrors = true;
                }

                if (string.IsNullOrWhiteSpace(contact.LastName))
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"Missing Last Name for {contact.Email}");
                    hasErrors = true;
                }
            }

            if (hasErrors)
                return;

            var existRecords = new List<string>();
            var newRecords = new List<string>();

            foreach (var contact in contacts)
            {
                var line = $"<i>{contact.FirstName} {contact.LastName}</i> <strong>{contact.Email}</strong>";

                if (contact.UserIdentifier.HasValue)
                    existRecords.Add(line);
                else
                    newRecords.Add(line);
            }

            var confirmHtml = string.Empty;

            if (existRecords.Count > 0)
            {
                var title = (newRecords.Count > 0 ? "Some of" : "All")
                    + " the email addresses you entered are already registered in the system";

                confirmHtml +=
                    $"<div class='alert alert-warning' role=\"alert\">" +
                    $"<p>{title}:</p><ul><li>{string.Join("</li><li>", existRecords)}</li></ul>" +
                    $"</div>";
            }

            var hasNewRecords = newRecords.Count > 0;

            if (hasNewRecords)
            {
                confirmHtml +=
                    $"<div class='alert alert-success' role=\"alert\">" +
                    $"<p>Here is the list of people who will be added to the system after you confirm the changes:</p>" +
                    $"<ul><li>{string.Join("</li><li>", newRecords)}</li></ul>" +
                    $"</div>";
            }

            MultiplePersonConfirmMessage.InnerHtml = confirmHtml;

            MultiplePersonConfirmWindow.Visible = true;
            MultiplePersonConfirmCommands.Visible = hasNewRecords;
            MultiplePersonOtherCommands.Visible = !hasNewRecords;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Create),
                "show_comfirm",
                $"modalManager.show('{MultiplePersonConfirmWindow.ClientID}');",
                true);
        }

        private void ConfirmMultiplePerson()
        {
            var contacts = GetContacts();
            if (contacts == null)
                return;

            var result = new List<Guid>();
            foreach (var contact in contacts)
            {
                if (contact.UserIdentifier == null)
                    contact.UserIdentifier = CreateContact(contact).UserIdentifier;

                result.Add(contact.UserIdentifier.Value);
            }

            RedirectAfterSave(result.ToArray());
        }

        private static QUser CreateContact(Contact contact)
        {
            var user = UserFactory.Create();
            var person = UserFactory.CreatePerson(Organization.Identifier);

            user.Email = contact.Email;
            person.EmailEnabled = true;
            person.Language = MultilingualString.DefaultLanguage;
            user.FirstName = contact.FirstName;
            user.LastName = contact.LastName;

            UserStore.Insert(user, person);

            return user;
        }

        #endregion

        #region Methods (parse email list)

        private Contact[] GetContacts()
        {
            if (!ParseContacts(MultiplePersonEmailAddressList.Text, out var contacts, out var errors))
            {
                if (errors.Length > 0)
                    ScreenStatus.AddMessage(AlertType.Error, $"Validation errors:<br>{string.Join("<br>", errors)}");
                else if (contacts.Length == 0)
                    ScreenStatus.AddMessage(AlertType.Error, $"No contacts specified");

                return null;
            }

            foreach (var contact in contacts)
            {
                var person = ServiceLocator.PersonSearch.GetPerson(Organization.OrganizationIdentifier, contact.Email);
                if (person != null)
                    contact.UserIdentifier = person.UserIdentifier;
            }

            return contacts;
        }

        public static bool ParseContacts(string text, out Contact[] contacts, out string[] errors)
        {
            var contactsList = new List<Contact>();
            var errorsList = new List<string>();

            using (var reader = new StringReader(text))
            {
                string line;
                var lineNumber = 1;

                while ((line = reader.ReadLine()) != null)
                {
                    line = StringHelper.TrimAndClean(line);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var contact = ParseLine(line);

                        if (contact == null)
                            errorsList.Add($"Line {lineNumber} has incorrect format");
                        else if (!EmailAddress.IsValidAddress(contact.Email))
                            errorsList.Add($"Line {lineNumber} has incorrect email address");
                        else
                            contactsList.Add(contact);
                    }

                    lineNumber++;
                }
            }

            errors = errorsList.ToArray();
            contacts = contactsList.ToArray();

            return errors.Length == 0 && contacts.Length > 0;
        }

        private static Contact ParseLine(string line)
        {
            var parts = Split(line);

            if (parts == null)
            {
                if (EmailAddress.IsValidAddress(line))
                    return new Contact { Email = line };
                return null;
            }

            var name = parts[0];
            var nameIndex = name.IndexOf(' ');

            if (nameIndex < 0)
                return null;

            return new Contact
            {
                FirstName = name.Substring(0, nameIndex).Trim(),
                LastName = name.Substring(nameIndex + 1).Trim(),
                Email = parts[1].Trim(new[] { ' ', '<', '>' })
            };
        }

        private static string[] Split(string line)
        {
            var parts = Split(line, ',');

            if (parts == null)
                parts = Split(line, ' ');

            if (parts == null)
                parts = Split(line, '\t');

            return parts;
        }

        private static string[] Split(string line, char splitChar)
        {
            var index = line.LastIndexOf(splitChar);

            if (index >= 0 && line.IndexOf('@', index) >= 0)
                return new string[] { line.Substring(0, index).Trim(), line.Substring(index + 1).Trim() };

            index = line.IndexOf(splitChar);

            return index >= 0 && line.IndexOf('@', 0, index) >= 0
                ? new string[] { line.Substring(index + 1).Trim(), line.Substring(0, index).Trim() }
                : null;
        }

        #endregion

        #region Methods (helpers)

        private void RedirectAfterSave(params Guid[] ids)
        {
            if (ReturnUrl.HasValue())
            {
                SavedIdentifiers = ids;

                var url = HttpResponseHelper.BuildUrl(ReturnUrl, "userCreated=1");

                HttpResponseHelper.Redirect(url);
            }
            else if (ids.Length > 0)
            {
                var url = HttpResponseHelper.BuildUrl(EditUrl, $"contact={ids[0]}&status=saved");

                HttpResponseHelper.Redirect(url);
            }
            else
            {
                HttpResponseHelper.Redirect("/ui/admin/contacts/people/search");
            }
        }

        #endregion

        #region IOverrideWebRouteParent

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);

        #endregion
    }
}
