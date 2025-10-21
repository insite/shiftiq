using System.Collections.ObjectModel;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ProgressPanelTriggerCollection : Collection<ProgressPanelTrigger>, IStateManager
    {
        #region Fields

        private static readonly TypeMapper<ProgressPanelTrigger> _itemTypeMapper;

        private StateBagListHelper<ProgressPanelTrigger> _stateHelper;

        #endregion

        #region Construction

        static ProgressPanelTriggerCollection()
        {
            _itemTypeMapper = new TypeMapper<ProgressPanelTrigger>();
        }

        public ProgressPanelTriggerCollection(bool isTrackingViewState)
        {
            _stateHelper = new StateBagListHelper<ProgressPanelTrigger>(_itemTypeMapper, isTrackingViewState);
        }

        #endregion

        #region Collection Management

        public void AddControl(Control control)
        {
            Add(new ProgressControlTrigger(control));
        }

        protected override void InsertItem(int index, ProgressPanelTrigger item)
        {
            item.Init(_stateHelper.IsTracking);

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ProgressPanelTrigger item)
        {
            item.Init(_stateHelper.IsTracking);

            base.SetItem(index, item);
        }

        #endregion

        #region IStateManager

        bool IStateManager.IsTrackingViewState => _stateHelper.IsTracking;

        object IStateManager.SaveViewState() => _stateHelper.Save(this);

        void IStateManager.LoadViewState(object state) => _stateHelper.Load(state, this);

        void IStateManager.TrackViewState() => _stateHelper.Track(this);

        #endregion
    }
}