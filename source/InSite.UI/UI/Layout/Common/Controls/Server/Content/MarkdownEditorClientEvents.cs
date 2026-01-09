using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MarkdownEditorClientEvents : StateBagProxy
    {
        #region Properties

        public string OnSetup
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnInited
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

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

        public string OnPreview
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public MarkdownEditorClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}