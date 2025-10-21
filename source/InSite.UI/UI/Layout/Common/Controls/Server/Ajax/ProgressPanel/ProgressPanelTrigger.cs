using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public abstract class ProgressPanelTrigger : IStateManager
    {
        #region Fields

        private StateBagItemHelper _stateHelper;

        #endregion

        #region Methods (initialization)

        public virtual void Init(bool isTrackingViewState)
        {
            if (_stateHelper != null)
                throw ApplicationError.Create("ProgressPanelItem is already initialized");

            _stateHelper = new StateBagItemHelper(isTrackingViewState, SaveState, LoadState);
        }

        #endregion

        #region IStateManager

        public abstract ProgressPanelTriggerClientData GetClientData(Control container);

        bool IStateManager.IsTrackingViewState => _stateHelper.IsTracking;

        object IStateManager.SaveViewState() => _stateHelper.Save();

        void IStateManager.LoadViewState(object state) => _stateHelper.Load(state);

        void IStateManager.TrackViewState() => _stateHelper.Track();

        protected abstract void SaveState(IStateWriter writer);

        protected abstract void LoadState(IStateReader reader);

        #endregion
    }
}