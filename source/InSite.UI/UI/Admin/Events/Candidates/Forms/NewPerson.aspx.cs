using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Candidates.Forms
{
    public partial class NewPerson : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/exams/search";

        [Serializable]
        private class Contact
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public Guid? UserIdentifier { get; set; }
            public bool InGroup { get; set; }
        }

        private Guid? RegistrationIdentifier
            => Guid.TryParse(Request["registration"], out var result) ? result : (Guid?)null;

        private Guid? SchoolIdentifier
            => Guid.TryParse(Request["school"], out var result) ? result : (Guid?)null;

        private string EditUrl =>
            $"/ui/admin/registrations/exams/edit?registration={RegistrationIdentifier}&panel=schools";

        private Contact[] Contacts
        {
            get => (Contact[])ViewState[nameof(Contacts)];
            set => ViewState[nameof(Contacts)] = value;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
           => $"registration={RegistrationIdentifier}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InputsValidateButton.Click += InputsValidateButton_Click;

            ConfirmButton.Click += ConfirmButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate || !RegistrationIdentifier.HasValue)
                HttpResponseHelper.Redirect(SearchUrl);

            var school = SchoolIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(SchoolIdentifier.Value) : null;
            if (school == null || school.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: school.GroupName);

            InputsSchoolName.Text = school.GroupName;

            var schoolMembers = MembershipSearch.Bind(x => x.User, x => x.GroupIdentifier == school.GroupIdentifier, nameof(User.FullName));

            InputsNoExistingContacts.Visible = schoolMembers.Length == 0;

            InputsExistingContacts.Visible = schoolMembers.Length > 0;
            InputsExistingContacts.DataSource = schoolMembers;
            InputsExistingContacts.DataBind();

            InputsCancelButton.NavigateUrl = EditUrl;
            ConfirmCancelButton.NavigateUrl = EditUrl;
        }

        private void InputsValidateButton_Click(object sender, EventArgs e)
        {
            ConfirmTab.Visible = false;

            if (!Page.IsValid)
                return;

            Contacts = GetContacts();
            if (Contacts == null)
                return;

            var changes = new List<string>();

            foreach (var contact in Contacts)
            {
                if (contact.InGroup)
                    changes.Add($"Person <span style='font-style:italic'>{contact.FirstName} {contact.LastName}</span> <strong>{contact.Email}</strong> already assigned to School");
                else if (contact.UserIdentifier.HasValue)
                    changes.Add($"Assign existing person <span style='font-style:italic'>{contact.FirstName} {contact.LastName}</span> <strong>{contact.Email}</strong> to School");
                else
                    changes.Add($"Add new person <span style='font-style:italic'>{contact.FirstName} {contact.LastName}</span> <strong>{contact.Email}</strong> and assign to School");
            }

            ConfirmTab.Visible = true;
            ConfirmTab.IsSelected = true;

            ConfirmChanges.DataSource = changes;
            ConfirmChanges.DataBind();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var usersToAssign = new List<Guid>();

            foreach (var contact in Contacts)
            {
                if (contact.InGroup)
                    continue;

                if (contact.UserIdentifier == null)
                    contact.UserIdentifier = CreateContact(contact).UserIdentifier;

                usersToAssign.Add(contact.UserIdentifier.Value);
            }

            foreach (var user in usersToAssign)
            {
                if (MembershipPermissionHelper.CanModifyMembership(SchoolIdentifier.Value))
                    MembershipStore.Save(MembershipFactory.Create(user, SchoolIdentifier.Value, Organization.Identifier));
            }

            HttpResponseHelper.Redirect(EditUrl);

            QUser CreateContact(Contact contact)
            {
                var user = UserFactory.Create();
                var person = UserFactory.CreatePerson(Organization.Identifier);

                user.Email = contact.Email;
                person.EmailEnabled = true;
                user.FirstName = contact.FirstName;
                user.LastName = contact.LastName;

                UserStore.Insert(user, person);

                return user;
            }
        }

        #region Methods (parse)

        private Contact[] GetContacts()
        {
            var contacts = ParseContacts();
            if (contacts == null)
                return null;

            var school = SchoolIdentifier.Value;

            foreach (var contact in contacts)
            {
                var person = PersonCriteria.BindFirst(
                    x => new
                    {
                        UserIdentifier = x.UserIdentifier,
                        HasMembership = x.User.Memberships.Where(m => m.GroupIdentifier == school).Any()
                    },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier,
                        EmailExact = contact.Email
                    });

                if (person == null)
                    continue;

                contact.UserIdentifier = person.UserIdentifier;
                contact.InGroup = person.HasMembership;
            }

            return contacts;
        }

        private Contact[] ParseContacts()
        {
            var contacts = new List<Contact>();
            var errors = new List<string>();

            using (var reader = new StringReader(InputsPersons.Text))
            {
                string line;
                var lineNumber = 1;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.HasValue())
                    {
                        var contact = ParseLine(line);

                        if (contact == null)
                            errors.Add($"Line {lineNumber} has incorrect format");
                        else if (!EmailAddress.IsValidAddress(contact.Email))
                            errors.Add($"Line {lineNumber} has incorrect email address");
                        else
                            contacts.Add(contact);
                    }

                    lineNumber++;
                }
            }

            if (errors.Count > 0)
                ScreenStatus.AddMessage(AlertType.Error, $"Validation errors:<br>{string.Join("<br>", errors)}");
            else if (contacts.Count == 0)
                ScreenStatus.AddMessage(AlertType.Error, $"No contacts specified");
            else
                return contacts.ToArray();

            return null;
        }

        private static Contact ParseLine(string line)
        {
            var parts = Split(line);

            if (parts == null)
                return null;

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
    }
}