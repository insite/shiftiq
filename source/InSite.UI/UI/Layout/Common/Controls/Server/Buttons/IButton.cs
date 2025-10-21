using System.Web.UI.WebControls;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public interface IButton : IButtonControl, IHasConfirmText, IHasToolTip, IHasText
    {
        string CssClass { get; set; }
        string OnClientClick { get; set; }
        bool Visible { get; set; }
    }
}
