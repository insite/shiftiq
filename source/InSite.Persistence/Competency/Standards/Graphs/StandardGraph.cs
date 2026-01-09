using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Common.Graphs;

namespace InSite.Persistence
{
    public class StandardGraph<TNode> : GraphModel<TNode, StandardGraphEdge<TNode>, StandardGraphEdge<TNode>> where TNode : GraphNodeModel
    {
        #region Classes

        private class EdgeDbModel
        {
            public Guid ParentStandardIdentifier { get; set; }
            public Guid ChildStandardIdentifier { get; set; }
            public int ToAssetNumber { get; set; }
            public int ChildSequence { get; set; }
            public int Sequence { get; set; }
            public bool IsRelationship { get; set; }
        }

        #endregion

        #region Initialization

        public static StandardGraph<TNode> LoadOrganizationEdges(Guid organizationId, Func<Guid, TNode> nodeCreator)
        {
            var edges = StandardContainmentSearch.SelectOrganizationEdges<EdgeDbModel>(organizationId);

            var graph = new StandardGraph<TNode>();

            graph.Init(edges, nodeCreator);

            return graph;
        }

        private void Init(EdgeDbModel[] edges, Func<Guid, TNode> nodeCreator)
        {
            for (var i = 0; i < edges.Length; i++)
            {
                var edge = edges[i];

                if (!HasNode(edge.ParentStandardIdentifier))
                    AddNode(nodeCreator(edge.ParentStandardIdentifier));

                if (!HasNode(edge.ChildStandardIdentifier))
                    AddNode(nodeCreator(edge.ChildStandardIdentifier));
            }

            var secondaryEdges = new List<EdgeDbModel>();

            foreach (var dbEdge in edges.OrderBy(x => x.ParentStandardIdentifier).ThenBy(x => x.ChildSequence).ThenBy(x => x.Sequence))
                AddContainment(new StandardGraphEdge<TNode>(dbEdge.ParentStandardIdentifier, dbEdge.ChildStandardIdentifier, dbEdge.ToAssetNumber));
        }

        #endregion

        #region Helper methods

        public TNode[][] FindCycles(Guid rootId) => FindCycles(GetNode(rootId));

        public TNode[][] FindCycles(TNode rootNode)
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            if (!HasNode(rootNode))
                throw new ApplicationError($"Root node not found: {rootNode.NodeId}");

            var path = new Stack<TNode>();
            var keys = new HashSet<Guid>();
            var cycles = new List<TNode[]>();

            path.Push(rootNode);
            keys.Add(rootNode.NodeId);

            FindCycles(rootNode, path, keys, cycles);

            keys.Remove(rootNode.NodeId);
            path.Pop();

            return cycles.ToArray();
        }

        private void FindCycles(TNode node, Stack<TNode> path, HashSet<Guid> keys, List<TNode[]> cycles)
        {
            foreach (var edge in GetOutgoingContainments(node.NodeId))
            {
                if (!keys.Contains(edge.ToNodeId))
                {
                    path.Push(edge.ToNode);
                    keys.Add(edge.ToNodeId);

                    FindCycles(edge.ToNode, path, keys, cycles);

                    keys.Remove(edge.ToNodeId);
                    path.Pop();
                }
                else
                {
                    var isStartFound = false;
                    var cyclePath = new List<TNode>();

                    foreach (var pathNode in path.Reverse())
                    {
                        if (!isStartFound && pathNode.NodeId == edge.ToNodeId)
                            isStartFound = true;

                        if (isStartFound)
                            cyclePath.Add(pathNode);
                    }

                    cyclePath.Add(edge.ToNode);

                    cycles.Add(cyclePath.ToArray());
                }
            }
        }

        public IEnumerable<TNode> GetAllChildren(Guid nodeId) => GetAllChildren(GetNode(nodeId));

        public IEnumerable<TNode> GetAllChildren(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!HasNode(node))
                throw new ApplicationError($"Node not found: {node.NodeId}");

            var children = new Dictionary<Guid, TNode>();

            GetAllChildren(node, children);

            return children.Values;
        }

        private void GetAllChildren(TNode parent, IDictionary<Guid, TNode> children)
        {
            foreach (var edge in GetOutgoingContainments(parent.NodeId))
            {
                if (children.ContainsKey(edge.ToNodeId))
                    continue;

                var toNode = edge.ToNode;

                children.Add(toNode.NodeId, toNode);

                GetAllChildren(toNode, children);
            }
        }

        public IEnumerable<TNode> GetRootNodes(Guid nodeId) => GetRootNodes(GetNode(nodeId));

        public IEnumerable<TNode> GetRootNodes(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (!HasNode(node))
                throw new ApplicationError($"Node not found: {node.NodeId}");

            {
                var stack = new Stack<TNode>();
                stack.Push(node);

                var traversedNodes = new HashSet<Guid>();

                while (stack.Count > 0)
                {
                    var currentNode = stack.Pop();
                    var incomingEdges = GetIncomingContainments(currentNode.NodeId);

                    traversedNodes.Add(currentNode.NodeId);

                    if (incomingEdges.Count > 0)
                    {
                        foreach (var inEdge in incomingEdges)
                        {
                            if (traversedNodes.Contains(inEdge.FromNodeId))
                                continue;

                            stack.Push(inEdge.FromNode);
                        }
                    }
                    else
                    {
                        yield return currentNode;
                    }
                }
            }
        }

        public TNode[][][] GetNodesPaths(Guid rootId) => GetNodesPaths(GetNode(rootId));

        public TNode[][][] GetNodesPaths(TNode rootNode)
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            if (!HasNode(rootNode))
                throw new ApplicationError($"Root node not found: {rootNode.NodeId}");

            var path = new Stack<TNode>();
            var keys = new HashSet<Guid>();
            var paths = new Dictionary<Guid, List<TNode[]>>();

            path.Push(rootNode);
            keys.Add(rootNode.NodeId);

            GetNodePaths(rootNode, path, keys, paths);

            keys.Remove(rootNode.NodeId);
            path.Pop();

            return paths.Select(x => x.Value.ToArray()).ToArray();
        }

        private void GetNodePaths(TNode node, Stack<TNode> path, HashSet<Guid> keys, Dictionary<Guid, List<TNode[]>> paths)
        {
            if (paths.ContainsKey(node.NodeId))
                paths[node.NodeId].Add(path.Reverse().ToArray());
            else
                paths.Add(node.NodeId, new List<TNode[]> { path.Reverse().ToArray() });

            foreach (var edge in GetOutgoingContainments(node.NodeId))
            {
                if (keys.Contains(edge.ToNodeId))
                    continue;

                path.Push(edge.ToNode);
                keys.Add(edge.ToNodeId);

                GetNodePaths(edge.ToNode, path, keys, paths);

                keys.Remove(edge.ToNodeId);
                path.Pop();
            }
        }

        #endregion
    }
}
