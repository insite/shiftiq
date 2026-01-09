using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NavContentProperties : StateBagProxy
    {
        #region Properties

        public string CssClass
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string Style
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public NavContentProperties(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}