using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Persistence
{
    /// <summary>
    /// Derives each child program's inherited task set from its parents' current tasks.
    /// The recompute is idempotent: re-running it always converges on the same state,
    /// so it also serves as a repair tool for drift.
    /// </summary>
    public static class ProgramContainmentCascade
    {
        private class MergedTask
        {
            public Guid ObjectIdentifier { get; set; }
            public string ObjectType { get; set; }
            public string TaskCompletionRequirement { get; set; }
            public bool TaskIsRequired { get; set; }
            public bool TaskIsPlanned { get; set; }
            public int? TaskLifetimeMonths { get; set; }
        }

        /// <summary>
        /// Recomputes the inherited task set of every child (and grandchild) of the given program.
        /// Call after any change to the program's tasks. Fast no-op when the program has no children.
        /// </summary>
        public static void CascadeToChildren(Guid programIdentifier)
        {
            var childIds = ProgramContainmentSearch.GetChildIdentifiers(programIdentifier);
            if (childIds.Length == 0)
                return;

            var visited = new HashSet<Guid> { programIdentifier };

            foreach (var childId in childIds)
                RecomputeInheritedTasks(childId, visited);
        }

        public static void RecomputeInheritedTasks(Guid childProgramIdentifier)
        {
            RecomputeInheritedTasks(childProgramIdentifier, new HashSet<Guid>());
        }

        /// <summary>
        /// Recomputes every child program in the organization. Maintenance entry point
        /// for repairing drift (e.g. rows changed by direct SQL).
        /// </summary>
        public static void RecomputeOrganization(Guid organizationIdentifier)
        {
            var childIds = ProgramContainmentSearch
                .SelectOrganizationEdges(organizationIdentifier)
                .Select(x => x.ChildProgramIdentifier)
                .Distinct();

            foreach (var childId in childIds)
                RecomputeInheritedTasks(childId);
        }

        private static void RecomputeInheritedTasks(Guid childId, HashSet<Guid> visited)
        {
            if (!visited.Add(childId))
                return;

            Guid organizationId;
            var inserted = false;
            var changed = new List<Guid>();

            using (var db = new InternalDbContext())
            {
                var child = db.TPrograms.FirstOrDefault(x => x.ProgramIdentifier == childId);
                if (child == null)
                    return;

                organizationId = child.OrganizationIdentifier;

                var parentIds = db.TProgramContainments
                    .Where(x => x.ChildProgramIdentifier == childId)
                    .Select(x => x.ParentProgramIdentifier)
                    .ToArray();

                var supply = db.TTasks.AsNoTracking()
                    .Where(t => parentIds.Contains(t.ProgramIdentifier))
                    .ToArray();

                var childTasks = db.TTasks
                    .Where(t => t.ProgramIdentifier == childId)
                    .ToList();

                // Most-restrictive merge per achievement: Required beats Optional,
                // Planned beats Unplanned, shortest non-null lifetime wins.
                var merged = supply
                    .GroupBy(t => t.ObjectIdentifier)
                    .Select(g => new MergedTask
                    {
                        ObjectIdentifier = g.Key,
                        ObjectType = g.First().ObjectType,
                        TaskCompletionRequirement = g.First().TaskCompletionRequirement,
                        TaskIsRequired = g.Any(t => t.TaskIsRequired),
                        TaskIsPlanned = g.Any(t => t.TaskIsPlanned),
                        TaskLifetimeMonths = g.Min(t => t.TaskLifetimeMonths)
                    })
                    .ToDictionary(m => m.ObjectIdentifier);

                var nextSequence = childTasks.Count > 0 ? childTasks.Max(t => t.TaskSequence) + 1 : 1;

                foreach (var item in merged.Values)
                {
                    var existing = childTasks.FirstOrDefault(t => t.ObjectIdentifier == item.ObjectIdentifier);

                    if (existing == null)
                    {
                        db.TTasks.Add(new TTask
                        {
                            TaskIdentifier = UniqueIdentifier.Create(),
                            OrganizationIdentifier = organizationId,
                            ProgramIdentifier = childId,
                            ObjectIdentifier = item.ObjectIdentifier,
                            ObjectType = item.ObjectType,
                            TaskCompletionRequirement = item.TaskCompletionRequirement,
                            TaskIsRequired = item.TaskIsRequired,
                            TaskIsPlanned = item.TaskIsPlanned,
                            TaskLifetimeMonths = item.TaskLifetimeMonths,
                            TaskSequence = nextSequence++,
                            TaskIsInherited = true
                        });

                        inserted = true;

                        if (item.ObjectType == ProgramTaskCascade.Achievement)
                            changed.Add(item.ObjectIdentifier);
                    }
                    else if (existing.TaskIsInherited)
                    {
                        // Inherited rows always equal the merge exactly.
                        if (existing.TaskIsRequired != item.TaskIsRequired
                            || existing.TaskIsPlanned != item.TaskIsPlanned
                            || existing.TaskLifetimeMonths != item.TaskLifetimeMonths)
                        {
                            existing.TaskIsRequired = item.TaskIsRequired;
                            existing.TaskIsPlanned = item.TaskIsPlanned;
                            existing.TaskLifetimeMonths = item.TaskLifetimeMonths;
                            db.Entry(existing).State = EntityState.Modified;

                            if (existing.ObjectType == ProgramTaskCascade.Achievement)
                                changed.Add(existing.ObjectIdentifier);
                        }
                    }
                    else
                    {
                        // Local collision: escalate to the merge, raise only, never lower.
                        var required = existing.TaskIsRequired || item.TaskIsRequired;
                        var planned = existing.TaskIsPlanned || item.TaskIsPlanned;
                        var lifetime = MinLifetime(existing.TaskLifetimeMonths, item.TaskLifetimeMonths);

                        if (existing.TaskIsRequired != required
                            || existing.TaskIsPlanned != planned
                            || existing.TaskLifetimeMonths != lifetime)
                        {
                            existing.TaskIsRequired = required;
                            existing.TaskIsPlanned = planned;
                            existing.TaskLifetimeMonths = lifetime;
                            db.Entry(existing).State = EntityState.Modified;

                            if (existing.ObjectType == ProgramTaskCascade.Achievement)
                                changed.Add(existing.ObjectIdentifier);
                        }
                    }
                }

                var unjustified = childTasks
                    .Where(t => t.TaskIsInherited && !merged.ContainsKey(t.ObjectIdentifier))
                    .ToList();

                foreach (var task in unjustified)
                {
                    var prerequisites = db.TPrerequisites
                        .Where(x => x.ObjectIdentifier == task.TaskIdentifier)
                        .ToList();

                    if (prerequisites.Count > 0)
                        db.TPrerequisites.RemoveRange(prerequisites);

                    var enrollments = db.TTaskEnrollments
                        .Where(x => x.TaskIdentifier == task.TaskIdentifier)
                        .ToList();

                    if (enrollments.Count > 0)
                        db.TTaskEnrollments.RemoveRange(enrollments);

                    if (child.CompletionTaskIdentifier == task.TaskIdentifier)
                        child.CompletionTaskIdentifier = null;

                    db.TTasks.Remove(task);

                    if (task.ObjectType == ProgramTaskCascade.Achievement)
                        changed.Add(task.ObjectIdentifier);
                }

                db.SaveChanges();

                foreach (var task in unjustified)
                    ServiceLocator.ContentStore?.DeleteContainer(task.TaskIdentifier);
            }

            // Sync TTaskEnrollment rows for learners already enrolled in the child program.
            if (inserted)
                TaskStore.EnrollUserToProgramTasks(organizationId, childId);

            // An inherited row that appears, changes, or disappears is a curriculum change
            // for everyone enrolled in the child, so their credentials move with it.
            if (changed.Count > 0)
                ProgramTaskCascade.Synchronize(organizationId, childId, changed);

            foreach (var grandchildId in ProgramContainmentSearch.GetChildIdentifiers(childId))
                RecomputeInheritedTasks(grandchildId, visited);
        }

        private static int? MinLifetime(int? a, int? b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            return Math.Min(a.Value, b.Value);
        }
    }
}
