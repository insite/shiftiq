using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Addresses.Controls
{
    public partial class AddressList : UserControl
    {
        private static readonly Dictionary<string, AddressType> AddressToType = new Dictionary<string, AddressType>(StringComparer.OrdinalIgnoreCase)
        {
            { ContactAddressType.Shipping, AddressType.Shipping },
            { ContactAddressType.Billing, AddressType.Billing },
            { ContactAddressType.Physical, AddressType.Physical },
            { ContactAddressType.Home, AddressType.Home },
            { ContactAddressType.Work, AddressType.Work }
        };

        private static readonly Dictionary<EntityType, string[]> AddressTypes = new Dictionary<EntityType, string[]>
        {
            {
                EntityType.User,
                new[]
                {
                    ContactAddressType.Home,
                    ContactAddressType.Shipping,
                    ContactAddressType.Billing,
                    ContactAddressType.Work,
                }
            },
            {
                EntityType.Group,
                new[]
                {
                    ContactAddressType.Shipping,
                    ContactAddressType.Billing,
                    ContactAddressType.Physical,
                }
            },
        };

        public EntityType ContactType { get; set; }

        private string[] CurrentAddressTypes => AddressTypes[ContactType];

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        private Dictionary<string, AddressEditor> _editors = new Dictionary<string, AddressEditor>();

        protected override void CreateChildControls()
        {
            AddPills();

            base.CreateChildControls();
        }

        public void AddPills()
        {
            if (AddressesNav.ItemsCount > 0)
                return;

            foreach (var type in CurrentAddressTypes)
            {
                var navItem = new NavItem { Title = ((IAdminPage)Page).Translator.Translate(type) };
                AddressesNav.AddItem(navItem);

                var addressEditor = (AddressEditor)LoadControl("AddressEditor.ascx");
                addressEditor.ValidationGroup = ValidationGroup ?? "ContactData";
                addressEditor.FieldsWidth = Unit.Percentage(100);

                navItem.Controls.Add(addressEditor);

                _editors.Add(type, addressEditor);
            }
        }

        public void SetInputValues(QPerson person)
        {
            if (ContactType != EntityType.User)
                throw new ApplicationError("Unexpected entity type");

            AddPills();

            var employerName = person.EmployerGroup?.GroupName;
            var homeAddress = person.GetAddress(ContactAddressType.Home);
            if (homeAddress.AddressIdentifier == Guid.Empty)
                homeAddress = null;

            foreach (var addressType in CurrentAddressTypes)
            {
                var userAddress = person.GetAddress(addressType);

                if (!AddressToType.TryGetValue(addressType, out var type))
                    throw new ArgumentException($"Unsupported address type : {addressType}");

                var employerAddress = person.EmployerGroupIdentifier.HasValue
                    ? ServiceLocator.GroupSearch.GetAddress(person.EmployerGroupIdentifier.Value,
                    (type == AddressType.Work ? AddressType.Physical : type))
                    : null;

                if (addressType != ContactAddressType.Home)
                    GetAddressEditor(addressType).SetUserInputValues(addressType, userAddress, employerAddress, employerName, homeAddress);
                else
                    GetAddressEditor(addressType).SetUserInputValues(addressType, userAddress, employerAddress, employerName);
            }
        }

        public void SetInputValues(QGroup group)
        {
            if (ContactType != EntityType.Group)
                throw new ApplicationError("Unexpected entity type");

            var addresses = ServiceLocator.GroupSearch.GetAddresses(group.GroupIdentifier);

            ContactType = EntityType.Group;

            AddPills();

            foreach (var addressType in CurrentAddressTypes)
            {
                var address = addresses.Find(x => string.Equals(x.AddressType, addressType, StringComparison.OrdinalIgnoreCase));

                GetAddressEditor(addressType).SetGroupInputValues(addressType, address, group.GroupIdentifier);
            }
        }

        public void GetInputValues(QPerson person)
        {
            if (ContactType != EntityType.User)
                throw new ApplicationError("Unexpected entity type");

            foreach (var addressType in CurrentAddressTypes)
            {
                var address = person.GetAddress(addressType);

                GetAddressEditor(addressType).GetInputValues(address);
            }
        }

        public void GetInputValues(Dictionary<AddressType, GroupAddress> addresses)
        {
            if (ContactType != EntityType.Group)
                throw new ApplicationError("Unexpected entity type");

            foreach (var addressType in CurrentAddressTypes)
            {
                if (!AddressToType.TryGetValue(addressType, out var type))
                    throw new ArgumentException($"Unsupported address type : {addressType}");

                var address = new GroupAddress();

                GetAddressEditor(addressType).GetInputValues(address);

                addresses.Add(type, address);
            }
        }

        private AddressEditor GetAddressEditor(string addressType)
        {
            var index = -1;
            var types = CurrentAddressTypes;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Equals(addressType, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            var items = AddressesNav.GetItems();

            return (AddressEditor)items[index].Controls[0];
        }
    }
}