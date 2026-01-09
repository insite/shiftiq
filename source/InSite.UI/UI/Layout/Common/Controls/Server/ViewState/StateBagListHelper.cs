using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public sealed class StateBagListHelper<T>
    {
        #region Properties

        public bool IsTracking => _isTracking;

        #endregion

        #region Fields

        private bool _isTracking;
        private object[] _initState;
        private ITypeMapper _types;

        #endregion

        #region Construction

        public StateBagListHelper(ITypeMapper types, bool isTracking)
        {
            _types = types;
            _isTracking = isTracking;
        }

        #endregion

        #region Methods

        public object Save(IList<T> list)
        {
            object state1, state2;

            {
                var writer = _initState == null || _initState.Length != list.Count
                    ? (IStateWriter)new StateFullWriter()
                    : new StateDiffWriter(_initState);

                SaveState(writer, list);

                state1 = writer.ToObject();
            }

            {
                var writer = new StateDiffWriter(new object[list.Count]);

                for (var i = 0; i < list.Count; i++)
                {
                    var iState = ((IStateManager)list[i]).SaveViewState();
                    writer.Add(iState);
                }

                state2 = writer.ToObject();
            }

            return state1 != null || state2 != null
                ? new Pair(state1, state2)
                : null;
        }

        public void Load(object state, IList<T> list)
        {
            if (!(state is Pair pair))
                return;

            if (pair.First != null)
            {
                var reader = new StateReader(pair.First);
                LoadState(reader, list);
                reader.Validate();
            }

            if (pair.Second != null)
            {
                var reader = new StateReader(pair.Second);

                for (var i = 0; i < reader.Count && i < list.Count; i++)
                    reader.Get<object>(itemState => ((IStateManager)list[i]).LoadViewState(itemState));

                reader.Validate();
            }
        }

        public void Track(IList<T> list)
        {
            if (_isTracking)
                return;

            var writer = new StateFullWriter();

            SaveState(writer, list);

            _initState = writer.ToArray();
            _isTracking = true;

            for (var i = 0; i < list.Count; i++)
                ((IStateManager)list[i]).TrackViewState();
        }

        private void LoadState(IStateReader reader, IList<T> list)
        {
            for (var i = 0; i < reader.Count; i++)
            {
                reader.Get<int>(typeId =>
                {
                    var typeMap = _types.Get(typeId);

                    if (i < list.Count)
                    {
                        T item = list[i];
                        if (item.GetType() != typeMap.Type)
                            list[i] = (T)typeMap.Create();
                    }
                    else if (i == list.Count)
                    {
                        list.Add((T)typeMap.Create());
                    }
                    else
                        throw ApplicationError.Create("Invalid view state: control items count is less them state items count");
                });
            }

            for (var i = list.Count - 1; i > reader.Count; i--)
                list.RemoveAt(i);
        }

        private void SaveState(IStateWriter writer, IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                writer.Add(_types.Get(list[i].GetType()));
        }

        #endregion
    }
}