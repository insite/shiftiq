using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Domain.Courses;
using InSite.Domain.Foundations;

using Shift.Common;

namespace InSite.Persistence
{
    public static class Course2Store
    {
        private static ICommander _commander;
        private static IContentSearch _contentSearch;

        public static void Initialize(ICommander commander, IContentSearch contentSearch)
        {
            _commander = commander;
            _contentSearch = contentSearch;
        }

        public static TContentStore _contentStore = new TContentStore();

        public static void InsertCourse(
            QCourse course,
            List<QUnit> units,
            List<QModule> modules,
            List<QActivity> activities,
            List<QActivityCompetency> competencies,
            List<QCoursePrerequisite> prerequisites,
            List<TContent> contents
            )
        {
            var contentMap = contents.GroupBy(x => x.ContainerIdentifier).ToDictionary(x => x.Key, x => x.ToList());

            var commands = new List<ICommand>();

            CourseCommandCreator.Create(null, null, course, GetContent(course.CourseIdentifier, course.CourseName), commands);

            foreach (var unit in units)
                UnitCommandCreator.Create(null, null, unit, GetContent(unit.UnitIdentifier, unit.UnitName), commands);

            foreach (var module in modules)
                ModuleCommandCreator.Create(course.CourseIdentifier, null, null, module, GetContent(module.ModuleIdentifier, module.ModuleName), commands);

            foreach (var activity in activities)
                ActivityCommandCreator.Create(course.CourseIdentifier, null, null, activity, GetContent(activity.ActivityIdentifier, activity.ActivityName), commands);

            if (competencies != null)
            {
                var competencyMap = competencies.GroupBy(x => x.ActivityIdentifier).Select(x => new { ActivityId = x.Key, List = x.ToList() }).ToList();
                foreach (var activityCompetencies in competencyMap)
                {
                    commands.Add(new AddCourseActivityCompetencies(course.CourseIdentifier, activityCompetencies.ActivityId, activityCompetencies.List.Select(x => new ActivityCompetency
                    {
                        CompetencyStandardIdentifier = x.CompetencyStandardIdentifier,
                        CompetencyCode = x.CompetencyCode,
                        RelationshipType = x.RelationshipType
                    }).ToArray()));
                }
            }

            if (prerequisites != null)
                AddPrerequisites();

            _commander.Send(new RunCommands(course.CourseIdentifier, commands.ToArray()));

            ContentContainer GetContent(Guid id, string name)
            {
                if (contentMap.TryGetValue(id, out var list))
                    return _contentSearch.GetBlock(list);

                var content = new ContentContainer();
                content.Title.Text.Default = name;
                return content;
            }

            void AddPrerequisites()
            {
                foreach (var p in prerequisites)
                {
                    var p2 = new Prerequisite
                    {
                        Identifier = p.CoursePrerequisiteIdentifier,
                        TriggerIdentifier = p.TriggerIdentifier,
                        TriggerType = (TriggerType)Enum.Parse(typeof(TriggerType), p.TriggerType),
                        TriggerChange = (TriggerChange)Enum.Parse(typeof(TriggerChange), p.TriggerChange),
                        TriggerConditionScoreFrom = p.TriggerConditionScoreFrom,
                        TriggerConditionScoreThru = p.TriggerConditionScoreThru
                    };

                    if (p.ObjectType == PrerequisiteObjectType.Unit.ToString())
                        commands.Add(new AddCourseUnitPrerequisite(course.CourseIdentifier, p.ObjectIdentifier, p2));
                    else if (p.ObjectType == PrerequisiteObjectType.Module.ToString())
                        commands.Add(new AddCourseModulePrerequisite(course.CourseIdentifier, p.ObjectIdentifier, p2));
                    else if (p.ObjectType == PrerequisiteObjectType.Activity.ToString())
                        commands.Add(new AddCourseActivityPrerequisite(course.CourseIdentifier, p.ObjectIdentifier, p2));
                    else
                        throw new ArgumentException($"ObjectType: {p.ObjectType}");
                }
            }
        }

        public static void UpdateCourse(QCourse course, ContentContainer content)
        {
            QCourse original;
            using (var db = new InternalDbContext())
                original = db.QCourses.Where(x => x.CourseIdentifier == course.CourseIdentifier).FirstOrDefault();

            var commands = CourseCommandCreator.Create(original, null, course, content, new List<ICommand>());

            _commander.Send(new RunCommands(course.CourseIdentifier, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(course.CourseIdentifier);
        }

        public static void DeleteCourse(Guid courseId)
        {
            _commander.Send(new DeleteCourse(courseId));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void ClearCache(Guid organizationId)
        {
            DomainCache.Instance.RemoveCourses(c => c.Organization == organizationId);
        }

        public static void DeleteProgramPrequisite(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var pre = db.TPrerequisites.FirstOrDefault(x => x.PrerequisiteIdentifier == id);
                if (pre != null)
                {
                    db.TPrerequisites.Remove(pre);
                    db.SaveChanges();
                }
            }
        }

        public static void UpdateUnit(QUnit unit, ContentContainer content)
        {
            QUnit original;
            using (var db = new InternalDbContext())
                original = db.QUnits.Where(x => x.UnitIdentifier == unit.UnitIdentifier).FirstOrDefault();

            var commands = UnitCommandCreator.Create(original, null, unit, content, new List<ICommand>());

            _commander.Send(new RunCommands(unit.CourseIdentifier, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(unit.CourseIdentifier);
        }

        public static void UpdateModule(Guid courseId, QModule module, ContentContainer content)
        {
            QModule original;
            using (var db = new InternalDbContext())
                original = db.QModules.Where(x => x.ModuleIdentifier == module.ModuleIdentifier).FirstOrDefault();

            var commands = ModuleCommandCreator.Create(courseId, original, null, module, content, new List<ICommand>());

            _commander.Send(new RunCommands(courseId, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void UpdateActivity(Guid courseId, QActivity activity, ContentContainer content)
        {
            QActivity original;
            using (var db = new InternalDbContext())
                original = db.QActivities.Where(x => x.ActivityIdentifier == activity.ActivityIdentifier).FirstOrDefault();

            var commands = ActivityCommandCreator.Create(courseId, original, null, activity, content, new List<ICommand>());

            _commander.Send(new RunCommands(courseId, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void ModifyCourseActivityType(Guid courseId, Guid activityId, ActivityType activityType)
        {
            _commander.Send(new ModifyCourseActivityType(courseId, activityId, ActivityType.Lesson));
            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void ConnectCourseActivityAssessmentForm(Guid courseId, Guid activityId, Guid formId)
        {
            _commander.Send(new ConnectCourseActivityAssessmentForm(courseId, activityId, formId));
            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void ConnectCourseActivityGradeItem(Guid courseId, Guid activityId, Guid gradeItemId)
        {
            _commander.Send(new ConnectCourseActivityGradeItem(courseId, activityId, gradeItemId));
            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void DeleteModule(Guid courseId, Guid moduleId)
        {
            _commander.Send(new RunCommands(courseId, new ICommand[]
            {
                new RemoveCourseModule(courseId, moduleId),
                new RemoveCourseEmptyNodes(courseId),
            }));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void DeleteActivity(Guid courseId, Guid moduleId, Guid activityId)
        {
            _commander.Send(new RunCommands(courseId, new ICommand[]
            {
                new RemoveCourseActivity(courseId, activityId),
                new RemoveCourseEmptyNodes(courseId),
                new ResequenceCourseActivities(courseId, moduleId)
            }));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void DeleteActivityCompetency(Guid courseId, Guid activityId, Guid competencyId)
        {
            _commander.Send(new RemoveCourseActivityCompetencies(courseId, activityId, new[] { competencyId }));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void Insert(Guid courseId, Guid activityId, ActivityCompetency[] competencies)
        {
            _commander.Send(new AddCourseActivityCompetencies(courseId, activityId, competencies));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void DeleteActivityCompetencies(Guid courseId, Guid activityId, Guid[] competencies)
        {
            _commander.Send(new RemoveCourseActivityCompetencies(courseId, activityId, competencies));

            DomainCache.Instance.RemoveCourse(courseId);
        }

        public static void InsertCategory(IEnumerable<TCourseCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                db.TCourseCategories.AddRange(list);
                db.SaveChanges();
            }
        }

        public static void DeleteCategory(IEnumerable<TCourseCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in list)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }

        public static void InsertCatalog(Guid organizationId, Guid catalogId, string catalogName)
        {
            using (var db = new InternalDbContext())
            {
                db.TCatalogs.Add(new TCatalog
                {
                    OrganizationIdentifier = organizationId,
                    CatalogIdentifier = catalogId,
                    CatalogName = catalogName
                });

                db.SaveChanges();
            }
        }

        public static void InsertProgramCategory(IEnumerable<TProgramCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                db.TProgramCategories.AddRange(list);
                db.SaveChanges();
            }
        }

        public static void DeleteProgramCategory(IEnumerable<TProgramCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in list)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }
    }
}
