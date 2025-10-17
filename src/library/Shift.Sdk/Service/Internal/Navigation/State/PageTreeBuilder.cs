using System.Collections.Generic;
using System.Linq;

namespace InSite.Domain.Foundations
{
    public static class PageTreeBuilder
    {
        public static PageTree BuildTree(IEnumerable<PageNode> nodes)
        {
            if (nodes == null) 
                return new PageTree();

            var list = nodes.ToList();
            var tree = FindTreeRoot(list);
            BuildTree(tree, list);

            return tree;
        }

        private static void BuildTree(PageTree tree, IList<PageNode> descendants)
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

        private static PageTree FindTreeRoot(IList<PageNode> nodes)
        {
            var rootNodes = nodes.Where(node => node.Parent == null);
            if (rootNodes.Count() != 1)
                return new PageTree();
            var rootNode = rootNodes.Single();
            nodes.Remove(rootNode);
            return Map(rootNode);
        }

        private static PageTree Map(PageNode node)
        {
            return new PageTree
            {
                Node = node
            };
        }
    }
}
