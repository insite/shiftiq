using System;
using System.Collections.Generic;

namespace Shift.Common.Graphs
{
    public interface IGraph
    {
        GraphNodeModel GetNode(Guid nodeId);
        IReadOnlyList<GraphEdgeModel> GetOutgoingContainments(Guid nodeId);
        IReadOnlyList<GraphEdgeModel> GetIncomingContainments(Guid nodeId);
        IReadOnlyList<GraphEdgeModel> GetOutgoingConnections(Guid nodeId);
        IReadOnlyList<GraphEdgeModel> GetIncomingConnections(Guid nodeId);
    }
}
