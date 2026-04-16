using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public class DropDownButtonItemCollection : Collection<DropDownButtonBaseItem>, IStateManager
    {
        #region Fields

        private StateBagListHelper<DropDownButtonBaseItem> _stateHelper;

        #endregion

        #region Construction

        public DropDownButtonItemCollection()
        {
            _stateHelper = new StateBagListHelper<DropDownButtonBaseItem>(DropDownButtonItemTypeMapper.Instance, false);
        }

        #endregion

        #region Indexer

        public DropDownButtonBaseItem this[string name] =>
            Items.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region Collection Management

        protected override void InsertItem(int index, DropDownButtonBaseItem item)
        {
            item.InitializeTracking(_stateHelper.IsTracking);

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, DropDownButtonBaseItem item)
        {
            item.InitializeTracking(_stateHelper.IsTracking);

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