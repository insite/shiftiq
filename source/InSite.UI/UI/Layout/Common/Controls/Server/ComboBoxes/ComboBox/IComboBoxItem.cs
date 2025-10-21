using System.Web.UI;

namespace InSite.Common.Web.UI
{
    internal interface IComboBoxItem
    {
        BaseComboBox ComboBox { get; }

        void Render(HtmlTextWriter writer);

        bool SetOwner(BaseComboBox owner, bool isTrackingViewState);
    }
}
