using System.Collections.Generic;

namespace InSite.Domain.Foundations
{
    public static class PageTreeExtensions
    {
        public static IEnumerable<PageTree> Ancestors(this PageTree value)
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

        public static IEnumerable<PageTree> Descendants(this PageTree value)
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

        public static IEnumerable<PageTree> GetUpstream(this PageTree value)
        {
            var list = new LinkedList<PageTree>();

            value.BindUpstream(list);

            return list;
        }

        private static void BindUpstream(this PageTree value, LinkedList<PageTree> list)
        {
            list.AddFirst(value);

            value.Parent?.BindUpstream(list);
        }
    }
}
