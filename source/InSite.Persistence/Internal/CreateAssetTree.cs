using System;

using Shift.Common.Trees;

namespace InSite.Persistence
{
    /// <summary>
    /// The AssetTree class is a wrapper for the ITree interface, exposing only the properties and
    /// methods required by this solution. It is context-insensitive with regard to user/role membership.
    /// </summary>
    [Serializable]
    public class CreateAssetTree
    {
        /// <summary>
        /// The internal representation for the hierarchy of groups.
        /// </summary>
        private readonly ITree<CreateAssetNode> _tree;

        /// <summary>
        /// The root of the tree.
        /// </summary>
        public INode<CreateAssetNode> Root { get { return _tree.Root; } }


        /// <summary>
        /// Enables indexing by group name.
        /// </summary>
        public INode<CreateAssetNode> this[CreateAssetNode group] { get { return _tree[group]; } }

        /// <summary>
        /// Initializes the tree. Two groups are considered equal if they have the same name. Names
        /// are sanitized by the StringComparer.Sanitize method.
        /// </summary>
        public CreateAssetTree()
        {
            var comparer = new CreateAssetNodeComparer();
            _tree = NodeTree<CreateAssetNode>.NewTree(comparer);
        }

        /// <summary>
        /// Adds a new group to the hierarchy. Duplicates are not permitted.
        /// </summary>
        public void Add(CreateAssetNode group)
        {
            if (_tree.Root.Contains(group))
                throw new Exception("Duplicate Not Allowed: " + group);

            _tree.AddChild(group);
        }

        /// <summary>
        /// Returns the path to a specific node in a tree.
        /// </summary>
        public static string CalculatePath(INode<CreateAssetNode> node)
        {
            if (!node.HasParent)
                return node.Data.Id.ToString();
            return CalculatePath(node.Parent) + "/" + node.Data.Id;
        }

        /// <summary>
        /// Returns true if the hierarchy already contains the group.
        /// </summary>
        public bool Contains(CreateAssetNode group)
        {
            return _tree.Contains(group);
        }

        /// <summary>
        /// Adds a child group to a parent group.
        /// </summary>
        public void Add(CreateAssetNode child, CreateAssetNode parent)
        {
            string error;

            // A child cannot be its own parent.

            if (_tree.DataComparer.Equals(child, parent))
            {
                error = string.Format("Self-Reference Not Allowed: {0} cannot be its own parent", child);
                throw new Exception(error);
            }

            // If the child is already in the tree, then the parent must also be in the tree,
            // and the child must be one of its descendents.

            if (Contains(child))
            {
                var childNode = _tree[child];

                if (!Contains(parent))
                {
                    error = string.Format("Duplicate Not Allowed: {0} is already in the tree", child);
                    throw new Exception(error);
                }

                var parentNode = _tree[parent];

                // If the child is already in the hierarchy under the parent then return (silent success).

                if (parentNode.AllChildren.Nodes.Contains(childNode))
                    return;

                // Throw an exception otherwise. A child can't be assigned to multiple locations in the hierarchy.

                error = string.Format(
                    "Multiple Parents Not Allowed: {0} is already assigned to {1}",
                    child,
                    parent);
                throw new Exception(error);
            }

            // Otherwise, add the parent if it is not already in the tree, and then add the child.

            if (!Contains(parent))
                Add(parent);

            _tree[parent].AddChild(child);
        }

        public INode<CreateAssetNode> FindById(int id)
        {
            var mock = new CreateAssetNode { Id = id };
            return Root[mock];
        }
    }
}