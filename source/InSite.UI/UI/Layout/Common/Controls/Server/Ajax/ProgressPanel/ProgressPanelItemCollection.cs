using System.Collections.ObjectModel;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ProgressPanelItemCollection : Collection<ProgressPanelItem>, IStateManager
    {
        #region Fields

        private static readonly TypeMapper<ProgressPanelItem> _itemTypeMapper;

        private StateBagListHelper<ProgressPanelItem> _stateHelper;

        #endregion

        #region Construction

        static ProgressPanelItemCollection()
        {
            _itemTypeMapper = new TypeMapper<ProgressPanelItem>();
        }

        public ProgressPanelItemCollection(bool isTrackingViewState)
        {
            _stateHelper = new StateBagListHelper<ProgressPanelItem>(_itemTypeMapper, isTrackingViewState);
        }

        #endregion

        #region Collection Management

        protected override void InsertItem(int index, ProgressPanelItem item)
        {
            item.Init(_stateHelper.IsTracking);

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ProgressPanelItem item)
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