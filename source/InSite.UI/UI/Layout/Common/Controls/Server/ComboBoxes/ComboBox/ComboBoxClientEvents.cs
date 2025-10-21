using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ComboBoxClientEvents : StateBagProxy
    {
        #region Properties

        public string OnChange
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public ComboBoxClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}