using System;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public sealed class StateBagItemHelper
    {
        #region Properties

        public bool IsTracking => _isTracking;

        #endregion

        #region Fields

        private bool _isTracking;
        private object[] _initState;
        private Action<IStateWriter> _save;
        private Action<IStateReader> _load;

        #endregion

        #region Construction

        public StateBagItemHelper(bool isTracking, Action<IStateWriter> save, Action<IStateReader> load)
        {
            _save = save;
            _load = load;
            _isTracking = isTracking;
        }

        #endregion

        #region Methods

        public object Save()
        {
            var writer = _initState == null
                ? (IStateWriter)new StateFullWriter()
                : new StateDiffWriter(_initState);

            _save(writer);

            return writer.ToObject();
        }

        public void Load(object state)
        {
            if (state == null)
                return;

            var reader = new StateReader(state);

            _load(reader);

            reader.Validate();
        }

        public void Track()
        {
            if (_isTracking)
                return;

            var writer = new StateFullWriter();

            _save(writer);

            _initState = writer.ToArray();

            _isTracking = true;
        }

        #endregion
    }
}