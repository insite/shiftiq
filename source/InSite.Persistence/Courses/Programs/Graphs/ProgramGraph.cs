using System;
using System.Collections.Generic;
using System.Linq;

namespace InSite.Persistence
{
    /// <summary>
    /// In-memory view of the program containment edges for one organization,
    /// used to validate new parent-child links and to walk descendants.
    /// </summary>
    public class ProgramGraph
    {
        private readonly Dictionary<Guid, List<Guid>> _childrenByParent;

        private ProgramGraph(Dictionary<Guid, List<Guid>> childrenByParent)
        {
            _childrenByParent = childrenByParent;
        }

        public static ProgramGraph LoadOrganizationEdges(Guid organizationIdentifier)
        {
            var edges = ProgramContainmentSearch.SelectOrganizationEdges(organizationIdentifier);

            var childrenByParent = new Dictionary<Guid, List<Guid>>();

            foreach (var edge in edges)
            {
                if (!childrenByParent.TryGetValue(edge.ParentProgramIdentifier, out var children))
                    childrenByParent.Add(edge.ParentProgramIdentifier, children = new List<Guid>());

                children.Add(edge.ChildProgramIdentifier);
            }

            return new ProgramGraph(childrenByParent);
        }

        public Guid[] GetDescendants(Guid programIdentifier)
        {
            var descendants = new HashSet<Guid>();
            var frontier = new Stack<Guid>();

            frontier.Push(programIdentifier);

            while (frontier.Count > 0)
            {
                var current = frontier.Pop();

                if (!_childrenByParent.TryGetValue(current, out var children))
                    continue;

                foreach (var child in children)
                    if (descendants.Add(child))
                        frontier.Push(child);
            }

            return descendants.ToArray();
        }

        /// <summary>
        /// True when adding the edge (parent -> child) would close a loop:
        /// the parent is the child itself, or the parent is already a descendant of the child.
        /// </summary>
        public bool WouldCreateCycle(Guid parentProgramIdentifier, Guid childProgramIdentifier)
        {
            if (parentProgramIdentifier == childProgramIdentifier)
                return true;

            return GetDescendants(childProgramIdentifier).Contains(parentProgramIdentifier);
        }
    }
}
