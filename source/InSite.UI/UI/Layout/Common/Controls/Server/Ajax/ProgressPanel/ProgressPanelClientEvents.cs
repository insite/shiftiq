using System.ComponentModel;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProgressPanelClientEvents : StateBagProxy
    {
        #region Properties

        public string OnPollingStart
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnPollingError
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnPollingStopped
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnCancelled
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnSubmitDetected
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string OnSubmitError
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public ProgressPanelClientEvents(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}