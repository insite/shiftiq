using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class InputClientEvents : StateBagProxy
    {
        #region Properties

        public string OnBlur
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnFocus
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnChange
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnClick
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnKeyDown
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnKeyPress
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnKeyUp
        {
            get { return (string)(GetValue() ?? string.Empty); }
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public InputClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}