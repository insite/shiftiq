using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EditorTranslationClientEvents : StateBagProxy
    {
        #region Properties

        public string OnSetText
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnGetText
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public EditorTranslationClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}