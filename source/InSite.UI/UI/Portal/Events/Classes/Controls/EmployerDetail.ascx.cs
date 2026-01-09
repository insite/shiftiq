using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class EmployerDetail : UserControl
    {
        public Guid? EmployerIdentifier => EmployerGroupIdentifier.Value;

        public string ContactEmail => EmployerContactEmail.Text;

        public (QGroup, QGroupAddress) GetEmployer()
        {
            if (!CompanyTypeExisting.Checked)
                return GetNewEmployerGroup();

            if (EmployerGroupIdentifier.Value == null)
                return (null, null);

            var employer = ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value);
            var address = ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping);

            return (employer, address);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += EmployerGroupIdentifier_ValueChanged;

            CompanyTypeExisting.AutoPostBack = true;
            CompanyTypeExisting.CheckedChanged += CompanyType_CheckedChanged;

            CompanyTypeNew.AutoPostBack = true;
            CompanyTypeNew.CheckedChanged += CompanyType_CheckedChanged;

            NewCompanyContactType.AutoPostBack = true;
            NewCompanyContactType.SelectedIndexChanged += NewCompanyContactType_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                EmployerGroupIdentifier.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier;
                EmployerGroupIdentifier.Filter.GroupType = GroupTypes.Employer;
            }
        }

        private void EmployerGroupIdentifier_ValueChanged(object sender, EventArgs e)
        {
            BindCompanyDetails();
        }

        private void CompanyType_CheckedChanged(object sender, EventArgs e)
        {
            ShowCompanyPanel();
        }

        private void NewCompanyContactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewCompanyContactFields.Visible = NewCompanyContactType.SelectedValue == "New";
        }

        public QGroup GetOrCreateEmployer()
        {
            if (CompanyTypeExisting.Checked)
            {
                return EmployerGroupIdentifier.HasValue && MembershipPermissionHelper.CanModifyMembership(EmployerGroupIdentifier.Value.Value)
                    ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value)
                    : null;
            }

            var organizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier;
            var currentUserIdentifier = CurrentSessionState.Identity.User.UserIdentifier;

            var groupName = NewCompanyName.Text.Trim();
            var group = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter
                {
                    GroupName = groupName,
                    OrganizationIdentifier = organizationIdentifier,
                    GroupType = GroupTypes.Employer
                })
                .FirstOrDefault();

            if (group == null)
                group = CreateNewEmployerGroup();
            else if (!MembershipPermissionHelper.CanModifyMembership(group))
                return null;

            Guid userIdentifier;

            if (NewCompanyContactType.SelectedValue == "Existing")
            {
                userIdentifier = currentUserIdentifier;
            }
            else
            {
                var user = ServiceLocator.UserSearch.GetUserByEmail(NewCompanyContactEmail.Text);
                if (user == null)
                {
                    user = UserFactory.Create();
                    user.UserIdentifier = UniqueIdentifier.Create();
                    user.FirstName = NewCompanyContactFirstName.Text;
                    user.LastName = NewCompanyContactLastName.Text;
                    user.Email = NewCompanyContactEmail.Text;

                    var person = UserFactory.CreatePerson(organizationIdentifier, currentUserIdentifier);
                    person.Phone = NewCompanyContactPhone.Text;
                    person.EmployerGroupIdentifier = group.GroupIdentifier;

                    UserStore.Insert(user, person);
                }

                userIdentifier = user.UserIdentifier;
            }

            MembershipHelper.Save(group.GroupIdentifier, userIdentifier, MembershipType.EmployerContact);

            return group;
        }

        public void ShowCompanyPanel()
        {
            var isNew = CompanyTypeNew.Checked;

            ExistingCompanyPanel.Visible = !isNew;
            AddressAndPhoneColumn.Visible = !isNew;
            EmployerContactColumn.Visible = !isNew;

            NewEmployerColumn.Visible = isNew;
            NewEmployerContactColumn.Visible = isNew;

            if (!isNew)
                BindCompanyDetails();
        }

        private void BindCompanyDetails()
        {
            EmployerContactName.Text = "None";
            EmployerContactPhoneNumber.Text = "None";
            EmployerContactEmail.Text = "None";

            var managerReference = EmployerGroupIdentifier.HasValue
                ? MembershipSearch.SelectFirst(x => x.GroupIdentifier == EmployerGroupIdentifier.Value.Value && x.MembershipType == MembershipType.EmployerContact)
                : null;

            Person manager = null;
            if (managerReference != null)
            {
                manager = PersonSearch.Select(CurrentSessionState.Identity.Organization.Identifier, managerReference.UserIdentifier, x => x.User);

                if (manager != null)
                {
                    EmployerContactName.Text = manager.User.FullName;
                    EmployerContactEmail.Text = manager.User.Email;

                    EmployerContactPhoneNumberField.Visible = !string.IsNullOrEmpty(manager.Phone);

                    if (!string.IsNullOrEmpty(manager.Phone))
                        EmployerContactPhoneNumber.Text = manager.Phone;
                }
            }

            var company = EmployerGroupIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(EmployerGroupIdentifier.Value.Value) : null;
            var address = VenueAddress.GetAddress(company?.GroupIdentifier, AddressType.Shipping);

            var isAddressVisible = !string.IsNullOrEmpty(address);
            var isPhoneVisible = !string.IsNullOrEmpty(company?.GroupPhone);

            AddressArea.Visible = isAddressVisible;
            AddressArea.Attributes["class"] = isPhoneVisible ? "d-flex pb-3 border-bottom" : "d-flex pt-2";
            EmployerGroupName.InnerText = company?.GroupName;
            EmployerAddress.Text = address;

            EmployerPhoneField.Visible = isPhoneVisible;
            EmployerPhone.Text = company?.GroupPhone;

            EmployerPhoneArea.Visible = isPhoneVisible;

            AddressAndPhoneColumn.Visible = isAddressVisible || isPhoneVisible;

            EmployerContactColumn.Visible = manager != null;
        }

        private (QGroup, QGroupAddress) GetNewEmployerGroup()
        {
            var employer = new QGroup
            {
                GroupType = GroupTypes.Employer,
                GroupName = NewCompanyName.Text.Trim()
            };

            var address = new QGroupAddress
            {
                AddressIdentifier = UniqueIdentifier.Create(),
                Street1 = NewCompanyStreet1.Text,
                City = NewCompanyCity.Text,
                Province = NewCompanyProvinceSelector.Value,
                PostalCode = NewCompanyPostalCode.Text,
                Country = "Canada"
            };

            return (employer, address);
        }

        private QGroup CreateNewEmployerGroup()
        {
            var id = UniqueIdentifier.Create();
            var commands = new List<Command>();

            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var name = NewCompanyName.Text.Trim();

            commands.Add(new CreateGroup(id, organization, GroupTypes.Employer, name));
            commands.Add(new DescribeGroup(id, null, null, null, "Company"));

            var address = new GroupAddress
            {
                Street1 = NewCompanyStreet1.Text,
                City = NewCompanyCity.Text,
                Province = NewCompanyProvinceSelector.Value,
                PostalCode = NewCompanyPostalCode.Text,
                Country = "Canada"
            };

            commands.Add(new ChangeGroupAddress(id, AddressType.Shipping, address));

            ServiceLocator.SendCommands(commands);

            return ServiceLocator.GroupSearch.GetGroup(id);
        }
    }
}