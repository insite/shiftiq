using System;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class AddressSection : UserControl
    {
        private static readonly string[] AddressTypes = new []
        {
            ContactAddressType.Shipping,
            ContactAddressType.Billing,
            ContactAddressType.Physical,
        };

        protected override void CreateChildControls()
        {
            AddPills();
            base.CreateChildControls();
        }

        public void SetInputValues(Guid groupIdentifier)
        {
            var addresses = ServiceLocator.GroupSearch.GetAddresses(groupIdentifier);

            AddPills();

            foreach (var addressType in AddressTypes)
            {
                var address = addresses.Find(x => string.Equals(x.AddressType, addressType, StringComparison.OrdinalIgnoreCase));

                GetAddressViewer(addressType).SetInputValues(addressType, address);
            }
        }

        private void AddPills()
        {
            if (AddressesNav.ItemsCount > 0)
                return;

            foreach (var type in AddressTypes)
            {
                var navItem = new NavItem { Title = ((IAdminPage)Page).Translator.Translate(type) };
                AddressesNav.AddItem(navItem);

                var addressViewer = (AddressViewer)LoadControl("AddressViewer.ascx");

                navItem.Controls.Add(addressViewer);
            }
        }

        private AddressViewer GetAddressViewer(string addressType)
        {
            var index = -1;
            var types = AddressTypes;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Equals(addressType, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            var items = AddressesNav.GetItems();

            return (AddressViewer)items[index].Controls[0];
        }
    }
}