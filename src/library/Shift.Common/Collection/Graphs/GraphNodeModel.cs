using System;

namespace Shift.Common.Graphs
{
    [Serializable]
    public abstract class GraphNodeModel
    {
        #region Properties

        public abstract Guid NodeId { get; set; }
        public abstract bool HasParent { get; }
        public abstract bool HasChildren { get; }

        #endregion

        #region Fields

        [NonSerialized]
        private IGraph _graph;

        #endregion

        #region Internal methods

        internal bool AttachGraph(IGraph container)
        {
            var isValid = _graph == null && OnGraphAttach(container);

            if (isValid)
                _graph = container;

            return isValid;
        }

        internal bool DetachGraph()
        {
            var isValid = _graph != null && OnGraphDetach();

            if (isValid)
                _graph = null;

            return isValid;
        }

        protected abstract bool OnGraphAttach(IGraph graph);

        protected abstract bool OnGraphDetach();

        #endregion
    }
}
