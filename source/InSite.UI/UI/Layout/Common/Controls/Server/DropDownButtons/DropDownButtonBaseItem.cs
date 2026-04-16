using System;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [Serializable]
    public abstract class DropDownButtonBaseItem : IStateManager
    {
        #region Properties

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrEmpty(value) ? StringHelper.RemoveNonAlphanumericCharacters(value) : null;
        }

        public bool Visible { get; set; }

        #endregion

        #region Fields

        private string _name;
        private StateBagItemHelper _stateHelper;

        #endregion

        #region Construction

        public DropDownButtonBaseItem()
        {
            Visible = true;
        }

        #endregion

        #region Methods (initialization)

        internal bool InitializeTracking(bool isTrackingViewState)
        {
            if (_stateHelper != null)
                return false;

            _stateHelper = new StateBagItemHelper(isTrackingViewState, SaveState, LoadState);
            return true;
        }

        #endregion

        #region IStateManager

        bool IStateManager.IsTrackingViewState => _stateHelper?.IsTracking ?? false;

        object IStateManager.SaveViewState() => SaveViewState();

        void IStateManager.LoadViewState(object state) => LoadViewState(state);

        void IStateManager.TrackViewState() => TrackViewState();

        protected virtual object SaveViewState()
        {
            return _stateHelper?.Save();
        }

        protected virtual void LoadViewState(object state)
        {
            _stateHelper?.Load(state);
        }

        protected virtual void TrackViewState()
        {
            _stateHelper?.Track();
        }

        protected virtual void SaveState(IStateWriter writer)
        {
            writer.Add(Name);
            writer.Add(Visible);
        }

        protected virtual void LoadState(IStateReader reader)
        {
            reader.Get<string>(x => Name = x);
            reader.Get<bool>(x => Visible = x);
        }

        #endregion

        #region Helper methods

        public DropDownButtonBaseItem Clone()
        {
            return (DropDownButtonBaseItem)MemberwiseClone();
        }

        #endregion
    }
}