using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Groups.Write;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Domain.Contacts;
using InSite.Domain.Events;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Data;
using InSite.Web.Helpers;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class AddToWaitingList : PortalBasePage
    {

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

        private Guid? CandidateIdentifier => Guid.TryParse(Request["candidate"], out var id) ? id : (Guid?)null;

        #endregion

        #region Fields

        private static readonly PortalFieldHandler<HtmlGenericControl> _fieldHandler = new PortalFieldHandler<HtmlGenericControl>();

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PersonCodeUniqueValidator.ServerValidate += PersonCodeUniqueValidator_ServerValidate;
            EmailUniqueValidator.ServerValidate += EmailUniqueValidator_ServerValidate;

            EmployerTypeExisting.AutoPostBack = true;
            EmployerTypeExisting.CheckedChanged += EmployerType_CheckedChanged;

            EmployerTypeNew.AutoPostBack = true;
            EmployerTypeNew.CheckedChanged += EmployerType_CheckedChanged;

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += EmployerGroupIdentifier_ValueChanged;

            NewEmployerContactType.AutoPostBack = true;
            NewEmployerContactType.SelectedIndexChanged += NewEmployerContactType_SelectedIndexChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();

            RenderBreadcrumb(GetOutlineLinkArgs());
        }

        #endregion

        #region Event handlers

        private void PersonCodeUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ServiceLocator.PersonSearch.IsPersonExist(Organization.Identifier, args.Value, CandidateIdentifier);
        }

        private void EmailUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !Email.Enabled
                || !ServiceLocator.UserSearch.IsUserExist(args.Value);
        }

        private void EmployerType_CheckedChanged(object sender, EventArgs e)
        {
            ShowEmployerPanel();
        }

        private void EmployerGroupIdentifier_ValueChanged(object sender, EventArgs e)
        {
            BindEmployerDetails();
        }

        private void NewEmployerContactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewEmployerContactFields.Visible = NewEmployerContactType.SelectedValue == "New";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var (person, user) = SavePerson();

            SaveRegistration(person);

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);

            var statusId = Outline.SetStatus(
                AlertType.Success,
                $"{user.FirstName} {user.LastName} ({user.Email}) has been added to the waiting list for " +
                $"{@event.EventTitle} ({@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}). " +
                $"You will be contacted if a space becomes available.");

            var group = EmployerGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value)
                : null;

            ServiceLocator.AlertMailer.Send(Organization.Identifier, User.UserIdentifier, User.Identifier, new AlertAddedToWaitingList
            {
                CandidateFullName = user.FullName,
                CandidateEmail = user.Email,
                ClassTitle = @event.EventTitle,
                ClassStartTime = @event.EventScheduledStart.FormatDateOnly(User.TimeZone),
                WaitlistedBy = User.FullName,
                Employer = group?.GroupName,
                Organization = Organization.CompanyName,
                ClassAchievement = GetAchievementName(@event)
            });

            HttpResponseHelper.Redirect(GetOutlineLink() + $"&status={statusId}");

            string GetAchievementName(QEvent _event)
            {
                if (_event == null || !_event.AchievementIdentifier.HasValue)
                    return null;

                return ServiceLocator.AchievementSearch.GetAchievement(_event.AchievementIdentifier.Value).AchievementTitle;
            }
        }

        #endregion

        #region Methods (load)

        private void LoadData()
        {
            if (!RegisterEmployee.RegisterLinkIsVisible() && CandidateIdentifier == null
                || EventIdentifier == null
                || TGroupPermissionSearch.IsAccessDenied(EventIdentifier.Value, Identity)
                )
            {
                NavigateToSearch();
            }

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Registrations);
            var classDataFields = @event.GetRegistrationFields().ToList();
            var organizationDataFields = Organization.Fields.ClassRegistration.ToList();
            var candidate = @event != null && CandidateIdentifier.HasValue ? UserSearch.Select(CandidateIdentifier.Value) : null;

            if (@event == null
                || @event.OrganizationIdentifier != Organization.OrganizationIdentifier
                || candidate == null && CandidateIdentifier.HasValue
                )
            {
                NavigateToSearch();
            }

            var registration = CandidateIdentifier.HasValue ? @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == CandidateIdentifier) : null;
            if (registration != null
                && (
                    string.Equals(registration.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(registration.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase)
                ))
            {
                HttpResponseHelper.Redirect(GetOutlineLink());
            }

            PageHelper.AutoBindHeader(this);

            var scheduledDate = $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";
            if (@event.EventScheduledEnd.HasValue)
                scheduledDate += $" - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PortalMaster.SidebarVisible(false);
            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(@event.EventTitle, Translate("Scheduled ") + scheduledDate);

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EmployerGroupIdentifier.Filter.GroupType = GroupTypes.Employer;

            SetInputValues(@event, candidate, classDataFields);
            BindEmployer(candidate);

            CancelButton.NavigateUrl = GetOutlineLink();
        }

        private void SetInputValues(QEvent currentEvent, User candidate, List<RegistrationField> classDataFields)
        {
            PersonCodeField.Visible = Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC;

            Email.Text = candidate?.Email?.ToLower();
            FirstName.Text = candidate?.FirstName;
            MiddleName.Text = candidate?.MiddleName;
            LastName.Text = candidate?.LastName;

            var person = candidate != null
                ? PersonSearch.Select(Organization.Identifier, candidate.UserIdentifier)
                : null;

            Birthdate.Value = person?.Birthdate;
            PersonCode.Text = person?.PersonCode;

            Email.Enabled = candidate == null;

            SetTextInputEnabled(FirstName);
            SetTextInputEnabled(MiddleName);
            SetTextInputEnabled(LastName);
            SetTextInputEnabled(PersonCode);
            SetDateSelectorEnabled(Birthdate);

            var helpText = ServiceLocator.ContentSearch.GetTooltipText("[Learner Number Help]", Organization.OrganizationIdentifier);
            if (!string.IsNullOrEmpty(helpText))
                PersonCodeHelpText.Text = helpText;

            SetFieldsVisibility(currentEvent);
        }

        private void SetFieldsVisibility(QEvent currentEvent)
        {
            var orgFields = Organization.Fields.ClassRegistration;
            var regFields = Organization.Toolkits.Events.AllowClassRegistrationFields
                ? currentEvent.GetRegistrationFields()
                : new List<RegistrationField>();

            foreach (var defaultField in PortalFieldInfo.ClassRegistration)
            {
                var orgField = orgFields?.FirstOrDefault(x => x.FieldName == defaultField.FieldName);
                var regFieldName = defaultField.FieldName.ToEnum<RegistrationFieldName>();
                var regField = regFields?.FirstOrDefault(x => x.FieldName == regFieldName);

                _fieldHandler.Init(PersonDetailsPanel, defaultField, orgField, regField);
            }

            var employerPanelField = regFields.FirstOrDefault(x => x.FieldName == RegistrationFieldName.EmployerPanel);
            EmployerField.Visible = employerPanelField == null || employerPanelField.IsVisible;
        }

        private void SetTextInputEnabled(InSite.Common.Web.UI.TextBox textBox)
            => textBox.Enabled = !textBox.Text.HasValue();

        private void SetDateSelectorEnabled(InSite.Common.Web.UI.DateSelector dateSelector)
            => dateSelector.Enabled = !dateSelector.Value.HasValue;

        #endregion

        #region Methods (save)

        private void SaveRegistration(QPerson person)
        {
            var registrationId = UniqueIdentifier.Create();

            var commands = new List<Command>
            {
                new RequestRegistration(
                    registrationId,
                    Organization.OrganizationIdentifier,
                    EventIdentifier.Value,
                    person.UserIdentifier,
                    null,
                    "Waitlisted",
                    null,
                    null,
                    null
                )
            };

            if (person.EmployerGroupIdentifier.HasValue)
                commands.Add(new AssignEmployer(registrationId, person.EmployerGroupIdentifier.Value));

            ServiceLocator.SendCommands(commands);
        }

        private (QPerson Person, QUser User) SavePerson()
        {
            QUser user;
            QPerson person;

            if (CandidateIdentifier.HasValue)
            {
                user = ServiceLocator.UserSearch.GetUser(CandidateIdentifier.Value);
                person = ServiceLocator.PersonSearch.GetPerson(CandidateIdentifier.Value, Organization.Identifier);
            }
            else
            {
                user = UserFactory.Create();
                person = UserFactory.CreatePerson(Organization.Identifier);

                person.EmailEnabled = true;
            }

            var employerId = GetOrCreateEmployer();
            var employerIdBefore = person.EmployerGroupIdentifier;
            var changed = GetCandidateFields(user, person, employerId);

            if (CandidateIdentifier.HasValue)
            {
                if (changed)
                {
                    UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

                    if (person.PersonIdentifier != Guid.Empty)
                        PersonStore.Update(person);
                    else
                    {
                        person.UserIdentifier = user.UserIdentifier;
                        PersonStore.Insert(person);
                    }
                }
            }
            else
            {
                user.Email = Email.Text;

                UserStore.Insert(user, person);
            }

            if (employerId.HasValue && employerId != employerIdBefore && MembershipPermissionHelper.CanModifyMembership(employerId.Value))
                MembershipHelper.Save(employerId.Value, user.UserIdentifier, "Employee");

            return (person, user);
        }

        private bool GetCandidateFields(QUser user, QPerson person, Guid? employerId)
        {
            var changed = false;

            if (user.FirstName != FirstName.Text)
            {
                user.FirstName = FirstName.Text;
                changed = true;
            }

            if (user.MiddleName.EmptyIfNull() != MiddleName.Text.EmptyIfNull())
            {
                user.MiddleName = MiddleName.Text;
                changed = true;
            }

            if (user.LastName != LastName.Text)
            {
                user.LastName = LastName.Text;
                changed = true;
            }

            if (person.Birthdate != Birthdate.Value)
            {
                person.Birthdate = Birthdate.Value;
                changed = true;
            }

            if (person.PersonCode.EmptyIfNull() != PersonCode.Text.EmptyIfNull())
            {
                person.PersonCode = PersonCode.Text;
                changed = true;
            }

            if (employerId.HasValue && person.EmployerGroupIdentifier != employerId.Value)
            {
                person.EmployerGroupIdentifier = employerId;
                changed = true;
            }

            return changed;
        }

        #endregion

        #region Employer

        private void BindEmployer(User candidate)
        {
            var employerGroupIdentifier = PersonSearch.Select(Organization.Identifier, User.UserIdentifier).EmployerGroupIdentifier;

            if (!employerGroupIdentifier.HasValue)
            {
                var role = MembershipSearch.SelectFirst(x =>
                    x.UserIdentifier == User.UserIdentifier
                    && x.MembershipType == MembershipType.EmployerContact
                    && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier
                );

                if (role != null)
                {
                    EmployerGroupIdentifier.Value = role.GroupIdentifier;
                }
                else if (candidate != null)
                {
                    var candidatePerson = PersonSearch.Select(Organization.Identifier, candidate.UserIdentifier);
                    if (candidatePerson?.EmployerGroupIdentifier != null)
                        EmployerGroupIdentifier.Value = candidatePerson.EmployerGroupIdentifier;
                }
            }
            else
            {
                EmployerGroupIdentifier.Value = employerGroupIdentifier;
            }

            if (Identity.Organization.Toolkits.Events?.CompanySelectionAndCreationDisabledDuringRegistration == true)
            {
                EmployerGroupIdentifier.Enabled = false;
                EmployerTypeExisting.Enabled = false;
                EmployerTypeNew.Enabled = false;
            }
            else
            {
                NewEmployerProvinceSelector.RefreshData();
                NewEmployerProvinceSelector.Value = Organization.PlatformCustomization.TenantLocation.Province;
            }

            ShowEmployerPanel();
        }

        private void ShowEmployerPanel()
        {
            var isNew = EmployerTypeNew.Checked;

            ExistingEmployerPanel.Visible = !isNew;
            EmployerGroupValidator.Visible = !isNew;
            ExistingEmployerColumn.Visible = !isNew;

            NewEmployerColumn.Visible = isNew;

            if (!isNew)
                BindEmployerDetails();
        }

        private void BindEmployerDetails()
        {
            var group = EmployerGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value)
                : null;

            ExistingEmployerColumn.Visible = group != null;

            if (group == null)
                return;

            ExistingEmployerContactName.Text = "None";
            ExistingEmployerContactPhone.Text = "None";
            ExistingEmployerContactEmail.Text = "None";

            var managerReference = EmployerGroupIdentifier.HasValue
                ? MembershipSearch.SelectFirst(
                    x => x.GroupIdentifier == EmployerGroupIdentifier.Value.Value
                      && x.MembershipType == MembershipType.EmployerContact)
                : null;

            Person manager = null;

            if (managerReference != null)
            {
                manager = PersonSearch.Select(Organization.Identifier, managerReference.UserIdentifier, x => x.User);

                if (manager != null)
                {
                    ExistingEmployerContactName.Text = manager.User.FullName;
                    ExistingEmployerContactEmail.Text = manager.User.Email;

                    ExistingEmployerContactPhoneField.Visible = !string.IsNullOrEmpty(manager.Phone);

                    if (!string.IsNullOrEmpty(manager.Phone))
                        ExistingEmployerContactPhone.Text = manager.Phone;
                }
            }

            var address = group != null ? ServiceLocator.GroupSearch.GetAddress(group.GroupIdentifier, AddressType.Shipping) : null;
            var addressText = address != null ? ClassVenueAddressInfo.GetAddress(address) : null;
            var isAddressVisible = !string.IsNullOrEmpty(addressText);
            var isPhoneVisible = !string.IsNullOrEmpty(group?.GroupPhone);

            ExistingEmployerName.Text = group?.GroupName;

            ExistingEmployerAddressItem.Visible = isAddressVisible;
            ExistingEmployerAddressItem.Attributes["class"] = isPhoneVisible ? "d-flex pb-3 border-bottom" : "d-flex pt-2";

            ExistingEmployerAddressLink.Text = addressText;
            ExistingEmployerAddressLink.NavigateUrl = address.GetGMapsAddressLink();

            ExistingEmployerPhoneItem.Visible = isPhoneVisible;
            ExistingEmployerPhoneField.Visible = isPhoneVisible;
            ExistingEmployerPhone.Text = group?.GroupPhone;

            ExistingEmployerAddress.Visible = isAddressVisible || isPhoneVisible;

            ExistingEmployerContact.Visible = manager != null;
        }

        private Guid? GetOrCreateEmployer()
        {
            if (EmployerTypeExisting.Checked)
                return EmployerGroupIdentifier.Value;

            var group = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter
                {
                    GroupName = NewEmployerName.Text,
                    OrganizationIdentifier = Organization.Identifier,
                    GroupType = GroupTypes.Employer
                })
                .FirstOrDefault();

            if (group == null)
                group = CreateNewEmployerGroup();
            else if (!MembershipPermissionHelper.CanModifyMembership(group))
                return null;

            var userIdentifier = NewEmployerContactType.SelectedValue == "Existing"
                ? User.UserIdentifier
                : CreateNewEmployerContact(group);

            MembershipHelper.Save(group.GroupIdentifier, userIdentifier, MembershipType.EmployerContact);

            return group.GroupIdentifier;
        }

        private QGroup CreateNewEmployerGroup()
        {
            var id = UniqueIdentifier.Create();
            var commands = new List<Command>();

            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var name = NewEmployerName.Text.Trim();

            commands.Add(new CreateGroup(id, organization, GroupTypes.Employer, name));
            commands.Add(new DescribeGroup(id, null, null, null, "Company"));

            var address = new GroupAddress
            {
                Street1 = NewEmployerStreet1.Text,
                City = NewEmployerCity.Text,
                Province = NewEmployerProvinceSelector.Value,
                PostalCode = NewEmployerPostalCode.Text,
                Country = "Canada"
            };

            commands.Add(new ChangeGroupAddress(id, AddressType.Shipping, address));

            ServiceLocator.SendCommands(commands);

            return ServiceLocator.GroupSearch.GetGroup(id);
        }

        private Guid CreateNewEmployerContact(QGroup group)
        {
            UserFactory factory = new UserFactory();

            factory.RegisterUser(
                NewEmployerContactEmail.Text.Trim(),
                Organization.Identifier,
                NewEmployerContactFirstName.Text,
                NewEmployerContactLastName.Text,
                null,
                group.GroupIdentifier,
                NewEmployerContactPhone.Text,
                null,
                Organization.Toolkits.Contacts?.DefaultMFA ?? false
                );

            return factory.User.UserIdentifier;
        }

        #endregion

        #region Methods (navigation)

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/portal/events/classes/search", true);

        private string GetOutlineLink()
        {
            return $"/ui/portal/events/classes/outline" + GetOutlineLinkArgs();
        }

        private string GetOutlineLinkArgs()
        {
            return $"?event={EventIdentifier}";
        }

        #endregion
    }
}