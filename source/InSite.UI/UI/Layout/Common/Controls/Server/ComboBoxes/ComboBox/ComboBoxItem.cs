using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [SuppressMessage("NDepend", "ND3106:SealMethodsThatSatisfyNonPublicInterfaces", Scope = "method", Justification = "No security threat exists here.")]
    public abstract class ComboBoxItem : IComboBoxItem, IStateManager
    {
        #region Properties

        protected BaseComboBox ComboBox { get; private set; }

        public abstract string Text { get; set; }

        public bool Visible { get; set; }

        #endregion

        #region Fields

        private StateBagItemHelper _stateHelper;

        #endregion

        #region Construction

        public ComboBoxItem()
        {
            Visible = true;
        }

        #endregion

        #region Methods (initialization)

        protected virtual bool SetOwner(BaseComboBox owner, bool isTrackingViewState)
        {
            if (ComboBox == null)
            {
                ComboBox = owner;

                _stateHelper = new StateBagItemHelper(isTrackingViewState, SaveState, LoadState);

                return true;
            }

            if (ComboBox == owner)
                return false;

            throw ApplicationError.Create("ComboBoxItem is already assigned to an owner");
        }

        #endregion

        #region IStateManager

        bool IStateManager.IsTrackingViewState => _stateHelper.IsTracking;

        object IStateManager.SaveViewState() => SaveViewState();

        void IStateManager.LoadViewState(object state) => LoadViewState(state);

        void IStateManager.TrackViewState() => TrackViewState();

        protected virtual object SaveViewState()
        {
            return _stateHelper.Save();
        }

        protected virtual void LoadViewState(object state)
        {
            _stateHelper.Load(state);
        }

        protected virtual void TrackViewState()
        {
            _stateHelper.Track();
        }

        protected virtual void SaveState(IStateWriter writer)
        {
            writer.Add(Visible);
        }

        protected virtual void LoadState(IStateReader reader)
        {
            reader.Get<bool>(x => Visible = x);
        }

        #endregion

        #region IComboBoxItem

        BaseComboBox IComboBoxItem.ComboBox => ComboBox;

        void IComboBoxItem.Render(HtmlTextWriter writer)
        {
            if (Visible)
                Render(writer);
        }

        protected abstract void Render(HtmlTextWriter writer);

        bool IComboBoxItem.SetOwner(BaseComboBox owner, bool isTrackingViewState) => SetOwner(owner, isTrackingViewState);

        #endregion
    }
}