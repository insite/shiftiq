using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shift.Common.Graphs
{
    [Serializable]
    public class GraphModel<TNode, TContainment, TConnection> : IGraph, ISerializable where TNode : GraphNodeModel where TContainment : GraphEdgeModel where TConnection : GraphEdgeModel
    {
        #region Classes

        [Serializable]
        private class EdgeKey : Tuple<Guid, Guid>
        {
            public EdgeKey(GraphEdgeModel edge)
                : this(edge.FromNodeId, edge.ToNodeId)
            {

            }

            public EdgeKey(Guid fromId, Guid toId)
                : base(fromId, toId)
            {

            }
        }

        #endregion

        #region Properties

        public IEnumerable<TNode> Nodes => _nodesByNodeId.Values;

        #endregion

        #region Fields

        private Dictionary<Guid, TNode> _nodesByNodeId;

        private Dictionary<Guid, List<TContainment>> _containmentsByFromNodeId;
        private Dictionary<Guid, List<TContainment>> _containmentsByToNodeId;
        private Dictionary<EdgeKey, TContainment> _containmentsByKey;

        private Dictionary<Guid, List<TConnection>> _connectionsByFromNodeId;
        private Dictionary<Guid, List<TConnection>> _connectionsByToNodeId;
        private Dictionary<EdgeKey, TConnection> _connectionsByKey;

        #endregion

        #region Construction

        public GraphModel()
        {
            _nodesByNodeId = new Dictionary<Guid, TNode>();

            _containmentsByFromNodeId = new Dictionary<Guid, List<TContainment>>();
            _containmentsByToNodeId = new Dictionary<Guid, List<TContainment>>();
            _containmentsByKey = new Dictionary<EdgeKey, TContainment>();

            _connectionsByFromNodeId = new Dictionary<Guid, List<TConnection>>();
            _connectionsByToNodeId = new Dictionary<Guid, List<TConnection>>();
            _connectionsByKey = new Dictionary<EdgeKey, TConnection>();
        }

        #endregion

        #region Serialization

        private GraphModel(SerializationInfo info, StreamingContext context)
            : this()
        {
            var nodes = (TNode[])info.GetValue("Nodes", typeof(TNode[]));
            for (var i = 0; i < nodes.Length; i++)
                AddNode(nodes[i]);

            var containments = (TContainment[])info.GetValue("Containments", typeof(TContainment[]));
            for (var i = 0; i < containments.Length; i++)
                AddContainment(containments[i]);

            var connections = (TConnection[])info.GetValue("Connections", typeof(TConnection[]));
            for (var i = 0; i < connections.Length; i++)
                AddConnection(connections[i]);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Nodes", _nodesByNodeId.Values.ToArray());
            info.AddValue("Containments", _containmentsByKey.Values.ToArray());
            info.AddValue("Connections", _connectionsByKey.Values.ToArray());
        }

        #endregion

        #region Methods

        public void AddNode(TNode node)
        {
            if (_nodesByNodeId.ContainsKey(node.NodeId))
                throw new ArgumentException("The node already exists in this graph.");

            if (!node.AttachGraph(this))
                throw new ArgumentException("The node already exists in another graph.");

            _nodesByNodeId.Add(node.NodeId, node);

            _containmentsByFromNodeId.Add(node.NodeId, new List<TContainment>());
            _containmentsByToNodeId.Add(node.NodeId, new List<TContainment>());

            _connectionsByFromNodeId.Add(node.NodeId, new List<TConnection>());
            _connectionsByToNodeId.Add(node.NodeId, new List<TConnection>());
        }

        public void ReplaceNode(TNode node)
        {
            if (!_nodesByNodeId.ContainsKey(node.NodeId))
                throw new ArgumentException($"The graph doesn't contain node with ID = {node.NodeId}.");

            var oldNode = _nodesByNodeId[node.NodeId];

            if (!oldNode.DetachGraph())
                throw new ArgumentException($"Can't detach graph.");

            if (!node.AttachGraph(this))
                throw new ArgumentException($"The node belongs to another graph.");

            _nodesByNodeId[node.NodeId] = node;
        }

        public void AddContainment(TContainment edge)
        {
            var key = new EdgeKey(edge);

            if (_containmentsByKey.ContainsKey(key))
                throw new ArgumentException($"The containment ({edge.FromNodeId} -> {edge.ToNodeId}) already exists in this graph.");

            if (!edge.AttachGraph(this))
                throw new ArgumentException($"The containment ({edge.FromNodeId} -> {edge.ToNodeId}) exists in another graph.");

            _containmentsByKey.Add(key, edge);
            _containmentsByFromNodeId[edge.FromNodeId].Add(edge);
            _containmentsByToNodeId[edge.ToNodeId].Add(edge);
        }

        public void AddConnection(TConnection edge)
        {
            var key = new EdgeKey(edge);

            if (_connectionsByKey.ContainsKey(key))
                throw new ArgumentException($"The connection ({edge.FromNodeId} -> {edge.ToNodeId}) already exists in this graph.");

            if (!edge.AttachGraph(this))
                throw new ArgumentException($"The connection ({edge.FromNodeId} -> {edge.ToNodeId}) exists in another graph.");

            _connectionsByKey.Add(key, edge);
            _connectionsByFromNodeId[edge.FromNodeId].Add(edge);
            _connectionsByToNodeId[edge.ToNodeId].Add(edge);
        }

        public bool HasNode(Guid nodeId) =>
            _nodesByNodeId.ContainsKey(nodeId);

        public bool HasNode(TNode node) =>
            node != null && _nodesByNodeId.ContainsKey(node.NodeId) && object.Equals(_nodesByNodeId[node.NodeId], node);

        public TNode GetNode(Guid nodeId) =>
            _nodesByNodeId.ContainsKey(nodeId) ? _nodesByNodeId[nodeId] : null;

        public IReadOnlyList<TContainment> GetOutgoingContainments(Guid nodeId) =>
            _containmentsByFromNodeId.ContainsKey(nodeId) ? _containmentsByFromNodeId[nodeId] : new List<TContainment>();

        public IReadOnlyList<TContainment> GetIncomingContainments(Guid nodeId) =>
            _containmentsByToNodeId.ContainsKey(nodeId) ? _containmentsByToNodeId[nodeId] : new List<TContainment>();

        public IReadOnlyList<TConnection> GetOutgoingConnections(Guid nodeId) =>
            _connectionsByFromNodeId.ContainsKey(nodeId) ? _connectionsByFromNodeId[nodeId] : new List<TConnection>();

        public IReadOnlyList<TConnection> GetIncomingConnections(Guid nodeId) =>
            _connectionsByToNodeId.ContainsKey(nodeId) ? _connectionsByToNodeId[nodeId] : new List<TConnection>();

        #endregion

        #region IGraph

        GraphNodeModel IGraph.GetNode(Guid nodeId) => GetNode(nodeId);

        IReadOnlyList<GraphEdgeModel> IGraph.GetOutgoingContainments(Guid nodeId) => GetOutgoingContainments(nodeId);

        IReadOnlyList<GraphEdgeModel> IGraph.GetIncomingContainments(Guid nodeId) => GetIncomingContainments(nodeId);

        IReadOnlyList<GraphEdgeModel> IGraph.GetOutgoingConnections(Guid nodeId) => GetOutgoingConnections(nodeId);

        IReadOnlyList<GraphEdgeModel> IGraph.GetIncomingConnections(Guid nodeId) => GetIncomingConnections(nodeId);

        #endregion
    }
}
