using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public static class ProgramContainmentStore
    {
        public static void Insert(IEnumerable<Guid> parentProgramIdentifiers, Guid childProgramIdentifier, Guid organizationIdentifier, Guid userIdentifier)
        {
            var parentIds = parentProgramIdentifiers?.Distinct().ToArray() ?? new Guid[0];
            if (parentIds.Length == 0)
                return;

            var inserted = false;

            using (var db = new InternalDbContext())
            {
                var child = db.TPrograms.FirstOrDefault(x => x.ProgramIdentifier == childProgramIdentifier);
                if (child == null)
                    throw new InvalidOperationException($"Program not found: {childProgramIdentifier}");

                if (child.OrganizationIdentifier != organizationIdentifier)
                    throw new InvalidOperationException("The child program belongs to a different organization.");

                // Depth cap: the hierarchy is limited to one level. A program that is
                // already a parent cannot be given parents of its own.
                if (db.TProgramContainments.Any(x => x.ParentProgramIdentifier == childProgramIdentifier))
                    throw new InvalidOperationException(
                        $"\"{child.ProgramName}\" is a parent of other programs, so it cannot have parent programs of its own. Program nesting is limited to one level.");

                var graph = ProgramGraph.LoadOrganizationEdges(organizationIdentifier);

                foreach (var parentId in parentIds)
                {
                    if (db.TProgramContainments.Any(x => x.ParentProgramIdentifier == parentId && x.ChildProgramIdentifier == childProgramIdentifier))
                        continue;

                    var parent = db.TPrograms.FirstOrDefault(x => x.ProgramIdentifier == parentId);
                    if (parent == null)
                        throw new InvalidOperationException($"Program not found: {parentId}");

                    if (parent.OrganizationIdentifier != organizationIdentifier)
                        throw new InvalidOperationException("The parent program belongs to a different organization.");

                    // Depth cap: a program that already has parents cannot become a parent.
                    if (db.TProgramContainments.Any(x => x.ChildProgramIdentifier == parentId))
                        throw new InvalidOperationException(
                            $"\"{parent.ProgramName}\" has a parent program of its own, so it cannot be a parent. Program nesting is limited to one level.");

                    if (graph.WouldCreateCycle(parentId, childProgramIdentifier))
                        throw new InvalidOperationException(
                            $"Linking \"{parent.ProgramName}\" as a parent of \"{child.ProgramName}\" would create a circular reference.");

                    var sequence = (db.TProgramContainments
                        .Where(x => x.ParentProgramIdentifier == parentId)
                        .Max(x => (int?)x.ChildSequence) ?? 0) + 1;

                    db.TProgramContainments.Add(new TProgramContainment
                    {
                        ParentProgramIdentifier = parentId,
                        ChildProgramIdentifier = childProgramIdentifier,
                        OrganizationIdentifier = organizationIdentifier,
                        ChildSequence = sequence,
                        Created = DateTimeOffset.UtcNow,
                        CreatedBy = userIdentifier
                    });

                    inserted = true;
                }

                if (inserted)
                    db.SaveChanges();
            }

            if (inserted)
                ProgramContainmentCascade.RecomputeInheritedTasks(childProgramIdentifier);
        }

        public static void Delete(Guid parentProgramIdentifier, Guid childProgramIdentifier)
        {
            var deleted = false;

            using (var db = new InternalDbContext())
            {
                var edge = db.TProgramContainments.FirstOrDefault(x =>
                    x.ParentProgramIdentifier == parentProgramIdentifier && x.ChildProgramIdentifier == childProgramIdentifier);

                if (edge != null)
                {
                    db.TProgramContainments.Remove(edge);
                    db.SaveChanges();
                    deleted = true;
                }
            }

            if (deleted)
                ProgramContainmentCascade.RecomputeInheritedTasks(childProgramIdentifier);
        }

        public static void DeleteAllForProgram(Guid programIdentifier)
        {
            Guid[] childIds;

            using (var db = new InternalDbContext())
            {
                var edges = db.TProgramContainments
                    .Where(x => x.ParentProgramIdentifier == programIdentifier || x.ChildProgramIdentifier == programIdentifier)
                    .ToList();

                childIds = edges
                    .Where(x => x.ParentProgramIdentifier == programIdentifier)
                    .Select(x => x.ChildProgramIdentifier)
                    .Distinct()
                    .ToArray();

                if (edges.Count == 0)
                    return;

                db.TProgramContainments.RemoveRange(edges);
                db.SaveChanges();
            }

            foreach (var childId in childIds)
                ProgramContainmentCascade.RecomputeInheritedTasks(childId);
        }
    }
}
