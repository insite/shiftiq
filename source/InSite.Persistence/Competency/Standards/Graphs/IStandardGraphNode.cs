using System;
using System.Collections.Generic;

using Shift.Common.Graphs;

namespace InSite.Persistence
{
    public interface IStandardGraphNode<TNode> where TNode: GraphNodeModel
    {
        Guid NodeId { get; }

        IReadOnlyCollection<StandardGraphEdge<TNode>> IncomingEdges { get; }
        IReadOnlyCollection<StandardGraphEdge<TNode>> OutgoingEdges { get; }

        bool HasParent { get; }
        bool HasChildren { get; }
    }
}
