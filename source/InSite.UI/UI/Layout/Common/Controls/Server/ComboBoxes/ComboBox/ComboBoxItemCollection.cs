using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public sealed class ComboBoxItemCollection<T> : Collection<T>, IStateManager where T : ComboBoxItem
    {
        #region Fields

        private BaseComboBox _comboBox;
        private StateBagListHelper<T> _stateHelper;

        #endregion

        #region Construction

        public ComboBoxItemCollection(BaseComboBox comboBox, bool isTrackingViewState)
            : base()
        {
            _comboBox = comboBox;
            _stateHelper = new StateBagListHelper<T>(ComboBoxItemTypeMapper.Instance, isTrackingViewState);
        }

        public ComboBoxItemCollection(BaseComboBox comboBox, bool isTrackingViewState, IList<T> list)
            : base(list)
        {
            _comboBox = comboBox;
            _stateHelper = new StateBagListHelper<T>(ComboBoxItemTypeMapper.Instance, isTrackingViewState);
        }

        #endregion

        #region Methods

        public IComboBoxOption GetOption(int index) => (IComboBoxOption)this[index];

        #endregion

        #region Collection Management

        protected override void InsertItem(int index, T item)
        {
            var isAssigned = SetItemOwner(item);

            base.InsertItem(index, item);

            if (isAssigned)
                ((IComboBoxItemOwner)_comboBox).ItemAssigned(item);
        }

        protected override void SetItem(int index, T item)
        {
            var isAssigned = SetItemOwner(item);

            base.SetItem(index, item);

            if (isAssigned)
                ((IComboBoxItemOwner)_comboBox).ItemAssigned(item);
        }

        private bool SetItemOwner(IComboBoxItem item)
        {
            return item.SetOwner(_comboBox, _stateHelper.IsTracking);
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