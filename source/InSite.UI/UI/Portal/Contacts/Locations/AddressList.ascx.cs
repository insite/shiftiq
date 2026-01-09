using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Individual.Controls
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

        private EntityType? CurrentEntity
        {
            get => (EntityType?)ViewState[nameof(CurrentEntity)];
            set => ViewState[nameof(CurrentEntity)] = ViewState[nameof(CurrentEntity)] ?? value;
        }

        private string[] CurrentAddressTypes =>
            AddressTypes[CurrentEntity.Value];

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
                var navItem = new NavItem { Title = type };
                AddressesNav.AddItem(navItem);

                var addressEditor = (AddressEditor)LoadControl("AddressEditor.ascx");
                addressEditor.FieldsWidth = Unit.Percentage(100);

                navItem.Controls.Add(addressEditor);

                _editors.Add(type, addressEditor);
            }
        }

        public void SetDefaultInputValues(EntityType entity, string defaultAddressType)
        {
            CurrentEntity = entity;

            AddPills();

            if (!string.IsNullOrEmpty(defaultAddressType) && _editors.ContainsKey(defaultAddressType))
                _editors[defaultAddressType].SetUserInputValues(defaultAddressType, new QPersonAddress { Country = "Canada" });
        }

        public void SetInputValues(QPerson person)
        {
            CurrentEntity = EntityType.User;

            AddPills();

            var employerName = person?.EmployerGroup?.GroupName;

            var homeAddress = person?.GetAddress(ContactAddressType.Home);
            if (homeAddress != null && homeAddress.AddressIdentifier == Guid.Empty)
                homeAddress = null;

            foreach (var addressType in CurrentAddressTypes)
            {
                var userAddress = person?.GetAddress(addressType);

                if (!AddressToType.TryGetValue(addressType, out var type))
                    throw new ArgumentException($"Unsupported address type : {addressType}");

                var employerAddress = person?.EmployerGroupIdentifier != null
                    ? ServiceLocator.GroupSearch.GetAddress(person.EmployerGroupIdentifier.Value,
                    (type == AddressType.Work ? AddressType.Physical : type))
                    : null;

                if (addressType != ContactAddressType.Home)
                    GetAddressEditor(addressType).SetUserInputValues(addressType, userAddress, employerAddress, employerName, homeAddress);
                else
                    GetAddressEditor(addressType).SetUserInputValues(addressType, userAddress, employerAddress, employerName);
            }
        }

        public void GetInputValues(QPerson person)
        {
            if (CurrentEntity != EntityType.User)
                throw new ApplicationError("Unexpected entity type");

            foreach (var addressType in CurrentAddressTypes)
            {
                var address = person.GetAddress(addressType);

                GetAddressEditor(addressType).GetInputValues(address);
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