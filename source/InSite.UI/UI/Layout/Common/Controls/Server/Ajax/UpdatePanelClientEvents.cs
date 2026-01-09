using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UpdatePanelClientEvents : StateBagProxy
    {
        #region Properties

        public string OnRequestStart
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnResponseEnd
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public UpdatePanelClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}