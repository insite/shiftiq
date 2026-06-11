using System;

using Shift.Common.Graphs;

namespace InSite.Persistence
{
    public class StandardGraphEdge<TNode> : GraphEdgeModel where TNode : GraphNodeModel
    {
        #region Properties

        public override Guid FromNodeId { get => _fromNodeId; set => _fromNodeId = value; }
        public override Guid ToNodeId { get => _toNodeId; set => _toNodeId = value; }

        public TNode FromNode => _graph.GetNode(FromNodeId);
        public TNode ToNode => _graph.GetNode(ToNodeId);

        #endregion

        #region Fields

        private Guid _fromNodeId;
        private Guid _toNodeId;

        private GraphModel<TNode, StandardGraphEdge<TNode>, StandardGraphEdge<TNode>> _graph;

        #endregion

        #region Construction

        public StandardGraphEdge(Guid from, Guid to, int number)
        {
            _fromNodeId = from;
            _toNodeId = to;
        }

        #endregion

        #region Overriden methods

        protected override bool OnGraphAttach(IGraph graph)
        {
            _graph = (GraphModel<TNode, StandardGraphEdge<TNode>, StandardGraphEdge<TNode>>)graph;

            return true;
        }

        #endregion
    }
}
