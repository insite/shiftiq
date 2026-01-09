using System;

namespace Shift.Common.Graphs
{
    [Serializable]
    public abstract class GraphEdgeModel
    {
        #region Properties

        public abstract Guid FromNodeId { get; set; }
        public abstract Guid ToNodeId { get; set; }

        #endregion

        #region Fields

        [NonSerialized]
        private IGraph _nodeContainer;

        #endregion

        #region Internal methods

        internal bool AttachGraph(IGraph container)
        {
            var isValid = _nodeContainer == null && OnGraphAttach(container);

            if (isValid)
                _nodeContainer = container;

            return isValid;
        }

        protected abstract bool OnGraphAttach(IGraph graph);

        #endregion
    }
}
