using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class PageTree
    {
        public PageNode Node { get; set; }
        public PageTree Parent { get; set; }

        [JsonProperty]
        public List<PageTree> Children { get; set; }

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

        public PageTree()
        {
            Node = new PageNode();
            Children = new List<PageTree>();
        }

        public void Add(PageTree child)
        {
            if (child == null)
                throw new ArgumentNullException();
            if (child.Parent != null)
                throw new InvalidOperationException("A tree node must be removed from its parent before adding as child.");
            if (this.Ancestors().Contains(child))
                throw new InvalidOperationException("A tree cannot be a cyclic graph.");
            if (Children == null)
            {
                Children = new List<PageTree>();
            }
            Debug.Assert(!Children.Contains(child), "At this point, the node is definately not a child");
            child.Parent = this;
            Children.Add(child);
        }

        public bool Remove(PageTree child)
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

        public PageTree[] Flatten()
        {
            return new[] { this }.Concat(Children.SelectMany(x => x.Flatten())).ToArray();
        }
    }
}
