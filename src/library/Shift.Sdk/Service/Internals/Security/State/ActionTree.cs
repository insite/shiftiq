using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class ActionTree 
    {
        public ActionNode Node { get; set; }
        public ActionTree Parent { get; set; }

        [JsonProperty]
        public List<ActionTree> Children { get; set; }

        private int? _depth = null;
        public int Depth
        {
            get
            {
                if (_depth == null)
                    _depth = Parent == null ? 0 : Parent.Depth + 1;
                return _depth.Value;
            }
        }

        private string _path = null;
        public string Path
        {
            get
            {
                if (_path == null)
                    _path = (Parent != null ? Parent.Path + "/" : string.Empty) + Node.Slug;
                return _path;
            }
        }

        public ActionTree()
        {
            Node = new ActionNode();
            Children = new List<ActionTree>();
        }

        public void Add(ActionTree child)
        {
            if (child == null)
                throw new ArgumentNullException();
            if (child.Parent != null)
                throw new InvalidOperationException("A tree node must be removed from its parent before adding as child.");
            if (this.Ancestors().Contains(child))
                throw new InvalidOperationException("A tree cannot be a cyclic graph.");
            if (Children == null)
            {
                Children = new List<ActionTree>();
            }
            Debug.Assert(!Children.Contains(child), "At this point, the node is definately not a child");
            child.Parent = this;
            Children.Add(child);
        }

        public bool Remove(ActionTree child)
        {
            if (child == null)
                throw new ArgumentNullException();
            if (child.Parent != this)
                return false;
            Debug.Assert(Children.Contains(child), "At this point, the node is definately a child");
            child.Parent = null;
            Children.Remove(child);
            if (!Children.Any())
                Children = null;
            return true;
        }

        public ActionTree[] Flatten()
        {
            return new[] { this }.Concat(Children.SelectMany(x => x.Flatten())).ToArray();
        }
    }

    public static class ActionTreeExtensions
    {
        public static IEnumerable<ActionTree> Ancestors(this ActionTree value)
        {
            // an ancestor is the node self and any ancestor of the parent
            var ancestor = value;
            // post-order tree walker
            while (ancestor != null)
            {
                yield return ancestor;
                ancestor = ancestor.Parent;
            }
        }

        public static IEnumerable<ActionTree> Descendants(this ActionTree value)
        {
            // a descendant is the node self and any descendant of the children
            if (value == null) yield break;
            yield return value;
            // depth-first pre-order tree walker
            foreach (var child in value.Children)
                foreach (var descendantOfChild in child.Descendants())
                {
                    yield return descendantOfChild;
                }
        }
    }

    public static class ActionTreeBuilder
    {
        public static ActionTree BuildTree(IEnumerable<ActionNode> nodes)
        {
            if (nodes == null)
                return new ActionTree();
            var nodeList = nodes.ToList();
            var tree = FindTreeRoot(nodeList);
            BuildTree(tree, nodeList);
            return tree;
        }

        private static void BuildTree(ActionTree tree, IList<ActionNode> descendants)
        {
            var children = descendants.Where(node => node.Parent == tree.Node.Identifier).ToArray();
            foreach (var child in children)
            {
                var branch = Map(child);
                tree.Add(branch);
                descendants.Remove(child);
            }
            foreach (var branch in tree.Children)
            {
                BuildTree(branch, descendants);
            }
        }

        private static ActionTree FindTreeRoot(IList<ActionNode> nodes)
        {
            var rootNodes = nodes.Where(node => node.Parent == null);
            if (rootNodes.Count() != 1)
                return new ActionTree();
            var rootNode = rootNodes.Single();
            nodes.Remove(rootNode);
            return Map(rootNode);
        }

        private static ActionTree Map(ActionNode node)
        {
            return new ActionTree
            {
                Node = node
            };
        }
    }
}
