using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Graphs;

namespace InSite.Persistence
{
    public class StandardGraphNode : GraphNodeModel, IStandardGraphNode<StandardGraphNode>
    {
        #region Properties

        public override Guid NodeId { get; set; }

        public IReadOnlyCollection<StandardGraphEdge<StandardGraphNode>> IncomingEdges => _graph.GetIncomingContainments(NodeId);
        public IReadOnlyCollection<StandardGraphEdge<StandardGraphNode>> OutgoingEdges => _graph.GetOutgoingContainments(NodeId);

        public override bool HasParent => _graph.GetIncomingContainments(NodeId).Any();
        public override bool HasChildren => _graph.GetOutgoingContainments(NodeId).Any();

        #endregion

        #region Fields

        private StandardGraph<StandardGraphNode> _graph;

        #endregion

        #region Construction

        public StandardGraphNode()
        {
        }

        public StandardGraphNode(Guid id)
        {
            NodeId = id;
        }

        #endregion

        #region Overriden methods

        protected override bool OnGraphAttach(IGraph graph)
        {
            _graph = (StandardGraph<StandardGraphNode>)graph;

            return true;
        }

        protected override bool OnGraphDetach()
        {
            _graph = null;

            return true;
        }

        #endregion

        #region Helper methods

        public StandardGraphNode[][] FindCycles() => _graph.FindCycles(this);

        public IEnumerable<StandardGraphNode> GetRootNodes() => _graph.GetRootNodes(this);

        public IEnumerable<StandardGraphNode> GetAllChildren() => _graph.GetAllChildren(this);

        public StandardGraphNode[][][] GetNodesPaths() => _graph.GetNodesPaths(this);

        protected StandardGraph<StandardGraphNode> GetGraph() => _graph;

        #endregion
    }
}
