using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public static class ProgramContainmentSearch
    {
        public class RelatedProgram
        {
            public Guid ProgramIdentifier { get; set; }
            public string ProgramName { get; set; }
        }

        public class ParentTaskSource
        {
            public Guid ParentProgramIdentifier { get; set; }
            public string ParentProgramName { get; set; }
            public Guid ObjectIdentifier { get; set; }
        }

        public static RelatedProgram[] SelectParentPrograms(Guid childProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return (from c in db.TProgramContainments.AsNoTracking()
                        join p in db.TPrograms.AsNoTracking() on c.ParentProgramIdentifier equals p.ProgramIdentifier
                        where c.ChildProgramIdentifier == childProgramIdentifier
                        orderby p.ProgramName
                        select new RelatedProgram
                        {
                            ProgramIdentifier = p.ProgramIdentifier,
                            ProgramName = p.ProgramName
                        }).ToArray();
            }
        }

        public static RelatedProgram[] SelectChildPrograms(Guid parentProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return (from c in db.TProgramContainments.AsNoTracking()
                        join p in db.TPrograms.AsNoTracking() on c.ChildProgramIdentifier equals p.ProgramIdentifier
                        where c.ParentProgramIdentifier == parentProgramIdentifier
                        orderby p.ProgramName
                        select new RelatedProgram
                        {
                            ProgramIdentifier = p.ProgramIdentifier,
                            ProgramName = p.ProgramName
                        }).ToArray();
            }
        }

        /// <summary>
        /// One row per (parent, achievement) pair supplying tasks to the child program.
        /// Used to label inherited tasks with their source programs and to compute
        /// how many tasks each parent supplies exclusively.
        /// </summary>
        public static ParentTaskSource[] SelectParentTaskSources(Guid childProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return (from c in db.TProgramContainments.AsNoTracking()
                        join p in db.TPrograms.AsNoTracking() on c.ParentProgramIdentifier equals p.ProgramIdentifier
                        join t in db.TTasks.AsNoTracking() on c.ParentProgramIdentifier equals t.ProgramIdentifier
                        where c.ChildProgramIdentifier == childProgramIdentifier
                        select new ParentTaskSource
                        {
                            ParentProgramIdentifier = p.ProgramIdentifier,
                            ParentProgramName = p.ProgramName,
                            ObjectIdentifier = t.ObjectIdentifier
                        }).ToArray();
            }
        }

        public static TProgramContainment[] SelectByChild(Guid childProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Where(x => x.ChildProgramIdentifier == childProgramIdentifier)
                    .OrderBy(x => x.ChildSequence)
                    .ToArray();
            }
        }

        public static TProgramContainment[] SelectByParent(Guid parentProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Where(x => x.ParentProgramIdentifier == parentProgramIdentifier)
                    .OrderBy(x => x.ChildSequence)
                    .ToArray();
            }
        }

        public static Guid[] GetParentIdentifiers(Guid childProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Where(x => x.ChildProgramIdentifier == childProgramIdentifier)
                    .Select(x => x.ParentProgramIdentifier)
                    .ToArray();
            }
        }

        public static Guid[] GetChildIdentifiers(Guid parentProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Where(x => x.ParentProgramIdentifier == parentProgramIdentifier)
                    .Select(x => x.ChildProgramIdentifier)
                    .ToArray();
            }
        }

        public static Guid[] GetDescendantIdentifiers(Guid programIdentifier)
        {
            Guid organizationIdentifier;

            using (var db = new InternalDbContext())
            {
                var program = db.TPrograms.AsNoTracking()
                    .FirstOrDefault(x => x.ProgramIdentifier == programIdentifier);

                if (program == null)
                    return new Guid[0];

                organizationIdentifier = program.OrganizationIdentifier;
            }

            var graph = ProgramGraph.LoadOrganizationEdges(organizationIdentifier);

            return graph.GetDescendants(programIdentifier);
        }

        public static TProgramContainment[] SelectOrganizationEdges(Guid organizationIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier)
                    .ToArray();
            }
        }

        public static bool Exists(Guid parentProgramIdentifier, Guid childProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments.AsNoTracking()
                    .Any(x => x.ParentProgramIdentifier == parentProgramIdentifier && x.ChildProgramIdentifier == childProgramIdentifier);
            }
        }

        public static int SelectMaxSequence(Guid parentProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramContainments
                    .Where(x => x.ParentProgramIdentifier == parentProgramIdentifier)
                    .Max(x => (int?)x.ChildSequence) ?? 0;
            }
        }
    }
}
