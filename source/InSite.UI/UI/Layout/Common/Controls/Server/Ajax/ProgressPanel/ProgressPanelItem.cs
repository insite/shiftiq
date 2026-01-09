using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public abstract class ProgressPanelItem : IStateManager
    {
        #region Properties

        public string Name
        {
            get => _name;
            set => _name = StringHelper.RemoveNonAlphanumericCharacters(value).NullIfEmpty();
        }

        public virtual bool HasContextItem => Name != null;

        #endregion

        #region Fields

        private string _name;
        private StateBagItemHelper _stateHelper;

        #endregion

        #region Methods (initialization)

        public virtual void Init(bool isTrackingViewState)
        {
            if (_stateHelper != null)
                throw ApplicationError.Create("ProgressPanelItem is already initialized");

            _stateHelper = new StateBagItemHelper(isTrackingViewState, SaveState, LoadState);
        }

        public abstract ProgressPanelItemClientData GetClientData();

        public abstract ProgressPanelContextItem GetContextData();

        #endregion

        #region IStateManager

        bool IStateManager.IsTrackingViewState => _stateHelper.IsTracking;

        object IStateManager.SaveViewState() => _stateHelper.Save();

        void IStateManager.LoadViewState(object state) => _stateHelper.Load(state);

        void IStateManager.TrackViewState() => _stateHelper.Track();

        protected virtual void SaveState(IStateWriter writer)
        {
            writer.Add(Name);
        }

        protected virtual void LoadState(IStateReader reader)
        {
            reader.Get<string>(x => Name = x);
        }

        #endregion
    }
}
