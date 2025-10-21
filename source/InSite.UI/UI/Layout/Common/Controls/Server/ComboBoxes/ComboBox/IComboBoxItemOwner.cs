namespace InSite.Common.Web.UI
{
    internal interface IComboBoxItemOwner
    {
        void ItemAssigned(ComboBoxItem item);

        void ItemSelected(ComboBoxItem item);
    }
}
