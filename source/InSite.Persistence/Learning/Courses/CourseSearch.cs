using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Records.Read;
using InSite.Application.Surveys.Read;
using InSite.Domain.CourseObjects;
using InSite.Domain.Foundations;
using InSite.Persistence.Content;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using TriggerType = InSite.Domain.Courses.TriggerType;

namespace InSite.Persistence
{
    public static class CourseSearch
    {
        #region Classes

        private class CourseReadHelper : ReadHelper<QCourse>
        {
            public static readonly CourseReadHelper Instance = new CourseReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QCourse>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QCourses.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(
                Expression<Func<QCourse, T>> binder,
                QCourseFilter filter)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QCourses.AsQueryable().AsNoTracking();

                    IQueryable<T> bind(IQueryable<QCourse> q) => q.Select(binder);
                    IQueryable<QCourse> filterQuery(IQueryable<QCourse> q) => QCourseFilterHelper.ApplyFilter(q, filter, context);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, null, filter.OrderBy, false);

                    return modelQuery.ToArray();
                }
            }
        }

        private class VCourseReadHelper : ReadHelper<VCourse>
        {
            public static readonly VCourseReadHelper Instance = new VCourseReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCourse>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VCourses.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(
                Expression<Func<VCourse, T>> binder,
                QCourseFilter filter)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VCourses.AsQueryable().AsNoTracking();

                    IQueryable<T> bind(IQueryable<VCourse> q) => q.Select(binder);
                    IQueryable<VCourse> filterQuery(IQueryable<VCourse> q) => QCourseFilterHelper.ApplyFilter(q, filter, context);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, null, filter.OrderBy, false);

                    return modelQuery.ToArray();
                }
            }
        }

        private class ModuleReadHelper : ReadHelper<QModule>
        {
            public static readonly ModuleReadHelper Instance = new ModuleReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QModule>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QModules.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private class UnitReadHelper : ReadHelper<QUnit>
        {
            public static readonly UnitReadHelper Instance = new UnitReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QUnit>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QUnits.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private class ActivityReadHelper : ReadHelper<QActivity>
        {
            public static readonly ActivityReadHelper Instance = new ActivityReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QActivity>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QActivities.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private class CourseCategoryReadHelper : ReadHelper<TCourseCategory>
        {
            public static readonly CourseCategoryReadHelper Instance = new CourseCategoryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCourseCategory>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TCourseCategories.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private class ProgramCategoryReadHelper : ReadHelper<TProgramCategory>
        {
            public static readonly ProgramCategoryReadHelper Instance = new ProgramCategoryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TProgramCategory>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TProgramCategories.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public class TPrerequisiteSearchResult
        {
            public Guid PrerequisiteIdentifier { get; set; }
            public string TriggerType { get; set; }
            public Guid TriggerIdentifier { get; set; }
            public string TriggerChange { get; set; }
            public string TriggerDescription { get; set; }
        }

        public static TPrerequisite[] SelectProgramPrerequisites(IEnumerable<Guid> containers)
        {
            using (var db = new InternalDbContext())
            {
                return db.TPrerequisites
                    .Where(x => containers.Contains(x.ObjectIdentifier))
                    .ToArray();
            }
        }

        public static TPrerequisite[] SelectProgramPrerequisites(Guid containerId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TPrerequisites.Where(x => x.ObjectIdentifier == containerId).ToArray();
            }
        }

        public static TPrerequisiteSearchResult[] GetPrerequisites(Guid containerId)
        {
            var list = new List<TPrerequisiteSearchResult>();
            using (var db = new InternalDbContext())
            {
                var prerequisites = db.QCoursePrerequisites.Where(x => x.ObjectIdentifier == containerId).ToArray();
                foreach (var prerequisite in prerequisites)
                {
                    var item = new TPrerequisiteSearchResult
                    {
                        PrerequisiteIdentifier = prerequisite.CoursePrerequisiteIdentifier,
                        TriggerType = prerequisite.TriggerType,
                        TriggerIdentifier = prerequisite.TriggerIdentifier,
                        TriggerChange = Shift.Common.Humanizer.TitleCase(prerequisite.TriggerChange),
                        TriggerDescription = GetTriggerDescription(prerequisite.TriggerType, prerequisite.TriggerIdentifier)
                    };

                    if (prerequisite.TriggerConditionScoreFrom.HasValue && prerequisite.TriggerConditionScoreThru.HasValue)
                        item.TriggerChange += $" between {prerequisite.TriggerConditionScoreFrom}% and {prerequisite.TriggerConditionScoreThru}%";

                    list.Add(item);
                }
            }
            return list
                .OrderBy(x => x.TriggerChange)
                .ThenBy(x => x.TriggerDescription)
                .ToArray();

            string GetTriggerDescription(string type, Guid id)
            {
                if (!Enum.TryParse<TriggerType>(type, true, out var triggerType))
                    throw new ApplicationError($"Unknown trigger type: ${type}");

                if (triggerType == TriggerType.Activity)
                {
                    var activity = SelectActivity(id);
                    if (activity != null)
                        return $"{activity.ActivityName}";
                }

                if (triggerType == TriggerType.AssessmentForm)
                {
                    var form = new BankSearch(null).GetForm(id);
                    if (form != null)
                        return form.FormName;
                }

                if (triggerType == TriggerType.AssessmentQuestion)
                {
                    var question = new BankSearch(null).GetQuestion(id);
                    if (question != null)
                        return $"{question.QuestionText}";
                }

                if (triggerType == TriggerType.GradeItem)
                {
                    var gradeitem = new RecordSearch(null).GetGradeItem(id);
                    if (gradeitem != null)
                        return $"{gradeitem.GradeItemName}";
                }

                return "Unknown";
            }
        }

        #endregion

        #region Select (entity)

        public static QActivity SelectActivity(Guid id, params Expression<Func<QActivity, object>>[] includes)
        {
            return ActivityReadHelper.Instance.SelectFirst(x => x.ActivityIdentifier == id, includes);
        }

        public static QCourse SelectCourse(Guid id, params Expression<Func<QCourse, object>>[] includes)
        {
            return CourseReadHelper.Instance.SelectFirst(x => x.CourseIdentifier == id, includes);
        }

        public static QModule SelectModule(Guid id, params Expression<Func<QModule, object>>[] includes)
        {
            return ModuleReadHelper.Instance.SelectFirst(x => x.ModuleIdentifier == id, includes);
        }

        public static QUnit SelectUnit(Guid id, params Expression<Func<QUnit, object>>[] includes)
        {
            return UnitReadHelper.Instance.SelectFirst(x => x.UnitIdentifier == id, includes);
        }

        public static QUnit[] SelectUnitsForCourse(Guid id)
        {
            using (var db = new InternalDbContext(false, false))
            {
                var units = db.QUnits.AsNoTracking()
                    .Include(x => x.Modules)
                    .Where(x => x.CourseIdentifier == id)
                    .OrderBy(x => x.UnitSequence)
                    .ToArray();

                var activities = db.QActivities.AsNoTracking()
                    .Where(x => x.Module.Unit.CourseIdentifier == id)
                    .ToArray();

                foreach (var unit in units)
                {
                    unit.Modules = unit.Modules.OrderBy(x => x.ModuleSequence).ToArray();
                    foreach (var module in unit.Modules)
                        module.Activities = activities.Where(x => x.ModuleIdentifier == module.ModuleIdentifier).OrderBy(x => x.ActivitySequence).ToArray();
                }

                return units;
            }
        }

        public static QModule[] SelectModulesForUnit(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var modules = db.QModules.AsNoTracking()
                      .Where(x => x.Unit.UnitIdentifier == id)
                      .Include(x => x.Activities)
                      .OrderBy(x => x.ModuleSequence)
                      .ToArray();

                foreach (var module in modules)
                    module.Activities = module.Activities.OrderBy(x => x.ActivitySequence).ToArray();

                return modules;
            }
        }

        public static T[] BindCourses<T>(
            Expression<Func<QCourse, T>> binder,
            Expression<Func<QCourse, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            CourseReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] BindUnits<T>(
            Expression<Func<QUnit, T>> binder,
            Expression<Func<QUnit, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            UnitReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] BindModules<T>(
            Expression<Func<QModule, T>> binder,
            Expression<Func<QModule, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ModuleReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] BindActivities<T>(
            Expression<Func<QActivity, T>> binder,
            Expression<Func<QActivity, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ActivityReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] BindCourseCategories<T>(
            Expression<Func<TCourseCategory, T>> binder,
            Expression<Func<TCourseCategory, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            CourseCategoryReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] BindProgramCategories<T>(
            Expression<Func<TProgramCategory, T>> binder,
            Expression<Func<TProgramCategory, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ProgramCategoryReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static TCourseCategory[] SelectCourseCategories(
            Expression<Func<TCourseCategory, bool>> filter,
            string sort = null) =>
            CourseCategoryReadHelper.Instance.Select(filter, sort, null);

        public static CourseSearchResult[] SelectCoursesByCategory(Guid categoryId)
        {
            using (var db = new InternalDbContext())
            {
                var courses = db.TCourseCategories
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.ItemIdentifier == categoryId)
                    .Join(
                        db.QCourses,
                        category => category.CourseIdentifier,
                        course => course.CourseIdentifier,
                        (category, course) => new
                        {
                            CourseName = course.CourseName
                        }
                    ).Select(x => new CourseSearchResult
                    {
                        CourseName = x.CourseName
                    })
                    .OrderBy(x => x.CourseName);

                return courses.ToArray();
            }
        }

        public static TProgramCategory[] SelectProgramCategories(
            Expression<Func<TProgramCategory, bool>> filter,
            string sort = null) =>
            ProgramCategoryReadHelper.Instance.Select(filter, sort, null);

        public static int CountActivities(Expression<Func<QActivity, bool>> filter) =>
            ActivityReadHelper.Instance.Count(filter);

        public static bool ActivityExists(Expression<Func<QActivity, bool>> filter) =>
            ActivityReadHelper.Instance.Exists(filter);

        public static bool CourseExists(Expression<Func<QCourse, bool>> filter) =>
            CourseReadHelper.Instance.Exists(filter);

        public static IEnumerable<Guid> GetActivityCompetencies(Guid activity)
        {
            using (var db = new InternalDbContext())
            {
                return db.TActivityCompetencies
                    .Where(x => x.ActivityIdentifier == activity)
                    .Select(x => x.CompetencyIdentifier)
                    .ToList();
            }
        }

        public static int CountActivityCompetencies(Expression<Func<TActivityCompetency, bool>> filter)
        {
            using (var db = new InternalDbContext())
                return db.TActivityCompetencies.Where(filter).Count();
        }

        #endregion

        #region Select (filter)

        public static int CountCourses(QCourseFilter filter)
        {
            using (var db = new InternalDbContext())
                return QCourseFilterHelper.ApplyFilter(db.QCourses.AsQueryable().AsNoTracking(), filter, db).Count();
        }

        public static int CountVCourses(QCourseFilter filter)
        {
            using (var db = new InternalDbContext())
                return QCourseFilterHelper.ApplyFilter(db.VCourses.AsQueryable().AsNoTracking(), filter, db).Count();
        }

        public static int CountModules(Guid course)
        {
            using (var db = new InternalDbContext())
                return db.QModules.Count(x => x.Unit.CourseIdentifier == course);
        }

        public static T[] BindCourses<T>(Expression<Func<QCourse, T>> binder, QCourseFilter filter) =>
            CourseReadHelper.Instance.Bind(binder, filter);

        public static T[] BindVCourses<T>(Expression<Func<VCourse, T>> binder, QCourseFilter filter) =>
            VCourseReadHelper.Instance.Bind(binder, filter);

        public static T BindCourseFirst<T>(
            Expression<Func<QCourse, T>> binder,
            Expression<Func<QCourse, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return CourseReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static T BindUnitFirst<T>(
            Expression<Func<QUnit, T>> binder,
            Expression<Func<QUnit, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return UnitReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static T BindModuleFirst<T>(
            Expression<Func<QModule, T>> binder,
            Expression<Func<QModule, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ModuleReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static T BindActivityFirst<T>(
            Expression<Func<QActivity, T>> binder,
            Expression<Func<QActivity, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ActivityReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        #endregion

        public static List<TActivityCompetency> SelectActivityCompetencies(IEnumerable<Guid> activityIds)
        {
            using (var db = new InternalDbContext())
            {
                return db.TActivityCompetencies
                    .Where(x => activityIds.Contains(x.ActivityIdentifier))
                    .ToList();
            }
        }

        public static List<QCourse> SelectRecentCourses(Guid organizationIdentifier, int count)
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourses
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier)
                    .OrderByDescending(x => x.Modified)
                    .Take(count)
                    .ToList();
            }
        }

        public static int SelectNextActivitySequence(Guid module)
        {
            using (var db = new InternalDbContext())
            {
                var activities = db.QActivities
                    .AsNoTracking()
                    .Where(x => x.ModuleIdentifier == module).ToList();

                if (activities.Count == 0)
                    return 1;

                return 1 + activities.Max(x => x.ActivitySequence);
            }
        }

        public static Guid? GetPreviousCourseActivity(Guid course, Guid activity)
        {
            using (var db = new InternalDbContext())
            {
                var activities = db.QActivities
                    .AsNoTracking()
                    .Where(x => x.Module.Unit.CourseIdentifier == course)
                    .OrderBy(x => x.Module.ModuleSequence).ThenBy(x => x.ActivitySequence)
                    .ToList();

                var cursor = activities.Single(a => a.ActivityIdentifier == activity);
                var index = activities.IndexOf(cursor);
                if (index > 0)
                    return activities[index - 1].ActivityIdentifier;
                return null;
            }
        }

        public static List<QCourseEnrollment> SelectCourseUsersForMessages()
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourseEnrollments
                    .AsNoTracking()
                    .Include(x => x.Course)
                    .Include(x => x.LearnerUser)
                    .Where(x => x.Course.SendMessageStalledAfterDays != null
                        && x.Course.SendMessageStalledMaxCount != null
                        && (x.Course.StalledToAdministratorMessageIdentifier != null || x.Course.StalledToLearnerMessageIdentifier != null)
                        && x.CourseCompleted == null
                    )
                    .ToList();
            }
        }

        #region Select (Outline)

        public static CourseListItem SelectCourseListItem(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var course = db.QCourses
                    .Where(x => x.CourseIdentifier == id)
                    .Select(x => new CourseListItem { Identifier = x.CourseIdentifier, Name = x.CourseName })
                    .FirstOrDefault();

                if (course != null)
                    course.Modules = db.QModules
                        .Where(x => x.Unit.CourseIdentifier == id)
                        .OrderBy(module => module.ModuleSequence)
                        .Select(x => new ModuleListItem
                        {
                            Identifier = x.ModuleIdentifier,
                            Name = x.ModuleName,
                            Activities = x.Activities
                                .OrderBy(activity => activity.ActivitySequence)
                                .Select(y => new ActivityListItem { Identifier = y.ActivityIdentifier, Name = y.ActivityName }).ToList()
                        }).ToList();

                return course;
            }
        }

        public static Course SelectCourse(IRecordSearch records, Guid courseId)
        {
            return DomainCache.Instance.GetCourse(courseId, () => LoadCourse(courseId, records));
        }

        private static Course LoadCourse(Guid courseId, IRecordSearch records)
        {
            var item = SelectCourse(courseId);
            if (item == null)
                return null;

            var course = new Course(courseId);
            course.Hook = item.CourseHook ?? item.CourseIdentifier.ToString();
            course.Slug = item.CourseSlug;
            course.Icon = item.CourseIcon;
            course.Image = item.CourseImage;
            course.Style = item.CourseStyle;
            course.FlagText = item.CourseFlagText;
            course.FlagColor = item.CourseFlagColor;

            LoadGradebook(item, course, records);

            course.Framework = item.FrameworkStandardIdentifier;
            course.Organization = item.OrganizationIdentifier;

            course.Asset = item.CourseAsset;
            course.Code = item.CourseCode;
            course.Label = item.CourseLabel;
            course.Sequence = item.CourseSequence;

            course.AllowDiscussion = item.AllowDiscussion;
            course.AllowMultipleUnits = item.IsMultipleUnitsEnabled;
            course.Catalog = item.CatalogIdentifier;
            course.IsHidden = item.CourseIsHidden;
            course.IsProgressReportEnabled = item.IsProgressReportEnabled;
            course.OutlineWidth = item.OutlineWidth;
            course.CompletionActivityIdentifier = item.CompletionActivityIdentifier;

            course.Content = TContentSearch.Instance.GetBlock(course.Identifier);
            if (course.Content.Title.Text.Default.IsEmpty())
                course.Content.Title.Text.Default = item.CourseName;

            course.CourseMessageStalledToLearner = item.StalledToLearnerMessageIdentifier;
            course.CourseMessageStalledToAdministrator = item.StalledToAdministratorMessageIdentifier;
            course.CourseMessageCompletedToLearner = item.CompletedToLearnerMessageIdentifier;
            course.CourseMessageCompletedToAdministrator = item.CompletedToAdministratorMessageIdentifier;
            course.SendMessageStalledAfterDays = item.SendMessageStalledAfterDays;
            course.SendMessageStalledMaxCount = item.SendMessageStalledMaxCount;

            course.Closed = item.Closed;

            using (var db = new InternalDbContext())
            {
                var permissions = db.TGroupPermissions
                    .Include(x => x.Group)
                    .Where(x => courseId == x.ObjectIdentifier)
                    .ToList();

                foreach (var p in permissions)
                    course.PrivacyGroups.Add(new PrivacyGroup
                    {
                        Identifier = p.GroupIdentifier,
                        Type = "Group",
                        Name = p.Group.GroupName
                    });
            }

            LoadUnits(course);

            return course;
        }

        private static void LoadGradebook(QCourse item, Course course, IRecordSearch records)
        {
            if (item.GradebookIdentifier == null)
                return;

            var gradebook = records.GetGradebookState(item.GradebookIdentifier.Value);

            course.Gradebook = new Gradebook { Identifier = gradebook.Identifier, Achievement = gradebook.Achievement, IsLocked = gradebook.IsLocked };

            foreach (var gi in gradebook.AllItems)
            {
                course.Gradebook.Items.Add(new GradeItem
                {
                    Identifier = gi.Identifier,
                    Type = "GradeItem",
                    Name = gi.Name,
                    Format = gi.Format,
                    PassPercent = gi.PassPercent,
                    Achievement = gi.Achievement?.Achievement
                });
            }
        }

        private static void LoadUnits(Course course)
        {
            var list = BindUnits(x => x, x => x.CourseIdentifier == course.Identifier, nameof(QUnit.UnitSequence))
                .OrderBy(x => x.UnitSequence)
                .ToList();

            List<QCoursePrerequisite> prerequisites;
            List<TGroupPermission> permissions;

            using (var db = new InternalDbContext())
            {
                var unitIdentifiers = list.Select(x => x.UnitIdentifier).ToArray();

                prerequisites = db.QCoursePrerequisites
                    .Where(x => unitIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();

                permissions = db.TGroupPermissions
                    .Include(x => x.Group)
                    .Where(x => unitIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();
            }

            foreach (var item in list)
            {
                var u = new Unit
                {
                    Identifier = item.UnitIdentifier,
                    Asset = item.UnitAsset,
                    Course = course,
                    Code = item.UnitCode,
                    IsAdaptive = item.UnitIsAdaptive,
                    PrerequisiteDeterminer = item.PrerequisiteDeterminer.ToEnum(DeterminerType.Any)
                };

                u.Content = TContentSearch.Instance.GetBlock(item.UnitIdentifier);
                if (u.Content.Title.Text.Default.IsEmpty())
                    u.Content.Title.Text.Default = item.UnitName;

                foreach (var prerequisite in prerequisites.Where(x => x.ObjectIdentifier == u.Identifier))
                    if (TryCreatePrerequisite(prerequisite, out var pModel))
                        u.Prerequisites.Add(pModel);

                course.Units.Add(u);
                LoadModules(course, u);

                foreach (var permission in permissions.Where(x => x.ObjectIdentifier == u.Identifier))
                    u.PrivacyGroups.Add(new PrivacyGroup
                    {
                        Identifier = permission.GroupIdentifier,
                        Type = "Group",
                        Name = permission.Group.GroupName
                    });
            }
        }

        private static void LoadModules(Course course, Unit unit)
        {
            var list = BindModules(x => x, x => x.UnitIdentifier == unit.Identifier, nameof(QModule.ModuleSequence));

            List<QCoursePrerequisite> prerequisites;
            List<TGroupPermission> permissions;

            using (var db = new InternalDbContext())
            {
                var moduleIdentifiers = list.Select(x => x.ModuleIdentifier).ToArray();

                prerequisites = db.QCoursePrerequisites
                    .Where(x => moduleIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();

                permissions = db.TGroupPermissions
                    .Include(x => x.Group)
                    .Where(x => moduleIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();
            }

            foreach (var item in list)
            {
                var m = new Module
                {
                    Identifier = item.ModuleIdentifier,
                    Code = item.ModuleCode,
                    Asset = item.ModuleAsset,
                    IsAdaptive = item.ModuleIsAdaptive,
                    Unit = unit,
                    PrerequisiteDeterminer = item.PrerequisiteDeterminer.ToEnum(DeterminerType.Any)
                };

                m.Content = TContentSearch.Instance.GetBlock(item.ModuleIdentifier);
                if (m.Content.Title.Text.Default.IsEmpty())
                    m.Content.Title.Text.Default = item.ModuleName;

                foreach (var prerequisite in prerequisites.Where(x => x.ObjectIdentifier == m.Identifier))
                    if (TryCreatePrerequisite(prerequisite, out var pModel))
                        m.Prerequisites.Add(pModel);

                unit.Modules.Add(m);

                LoadActivities(course, m);

                foreach (var permission in permissions.Where(x => x.ObjectIdentifier == m.Identifier))
                    m.PrivacyGroups.Add(new PrivacyGroup
                    {
                        Identifier = permission.GroupIdentifier,
                        Type = "Group",
                        Name = permission.Group.GroupName
                    });
            }
        }

        private static void LoadActivities(Course course, Module module)
        {
            var list = BindActivities(x => x, x => x.ModuleIdentifier == module.Identifier, nameof(QActivity.ActivitySequence));

            List<QBankForm> assessments;
            List<QSurveyForm> surveys;
            List<QCoursePrerequisite> prerequisites;
            List<TGroupPermission> permissions;

            using (var db = new InternalDbContext())
            {
                var activityIdentifiers = list.Select(x => x.ActivityIdentifier).ToArray();
                var assessmentIdentifiers = list.Select(x => x.AssessmentFormIdentifier).ToArray();
                var surveyIdentifiers = list.Select(x => x.SurveyFormIdentifier).ToArray();

                assessments = db.BankForms.AsNoTracking()
                    .Include(x => x.BankSpecification)
                    .Where(x => assessmentIdentifiers.Contains(x.FormIdentifier))
                    .ToList();

                surveys = db.QSurveyForms.AsNoTracking()
                    .Where(x => surveyIdentifiers.Contains(x.SurveyFormIdentifier))
                    .ToList();

                prerequisites = db.QCoursePrerequisites.AsNoTracking()
                    .Where(x => activityIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();

                permissions = db.TGroupPermissions.AsNoTracking()
                    .Include(x => x.Group)
                    .Where(x => activityIdentifiers.Contains(x.ObjectIdentifier))
                    .ToList();
            }

            foreach (var item in list)
            {
                var a = new Activity
                {
                    Identifier = item.ActivityIdentifier,
                    Asset = item.ActivityAsset,
                    Code = item.ActivityCode,
                    Module = module,
                    Hook = item.ActivityHook ?? item.ActivityIdentifier.ToString(),
                    Type = item.ActivityType,
                    IsAdaptive = item.ActivityIsAdaptive,
                    ContentDeliveryPlatform = item.ActivityPlatform,
                    DurationMinutes = item.ActivityMinutes,
                    IsMultilingual = item.ActivityIsMultilingual,
                    PrerequisiteDeterminer = item.PrerequisiteDeterminer.ToEnum(DeterminerType.Any),
                    Requirement = item.RequirementCondition.ToEnum(RequirementType.None),
                    DoneButtonText = item.DoneButtonText,
                    DoneButtonInstructions = item.DoneButtonInstructions,
                    DoneMarkedInstructions = item.DoneMarkedInstructions,
                };

                if (item.ActivityUrl != null)
                    a.Link = new Link { Url = item.ActivityUrl, Type = item.ActivityUrlType, Target = item.ActivityUrlTarget };

                a.Content = TContentSearch.Instance.GetBlock(a.Identifier);
                if (a.Content.Title.Text.Default.IsEmpty())
                    a.Content.Title.Text.Default = item.ActivityName;

                if (item.AssessmentFormIdentifier.HasValue)
                {
                    var assessment = assessments.FirstOrDefault(x => x.FormIdentifier == item.AssessmentFormIdentifier);
                    if (assessment != null)
                    {
                        a.Assessment = new ActivityAssessment
                        {
                            Identifier = assessment.FormIdentifier,
                            Type = "AssessmentForm",
                            Name = assessment.FormName,
                            Instructions = assessment.FormInstructionsForOnline,
                            Disclosure = assessment.BankSpecification.CalcDisclosure
                        };
                    }
                }

                if (item.QuizIdentifier.HasValue)
                {
                    a.QuizIdentifier = item.QuizIdentifier;
                }

                if (item.SurveyFormIdentifier.HasValue)
                {
                    var survey = surveys.FirstOrDefault(x => x.SurveyFormIdentifier == item.SurveyFormIdentifier);
                    if (survey != null)
                    {
                        a.Survey = new ActivitySurvey
                        {
                            Identifier = survey.SurveyFormIdentifier,
                            Type = "SurveyForm",
                            Name = survey.SurveyFormName
                        };
                    }
                }

                if (course.Gradebook != null && item.GradeItemIdentifier.HasValue)
                {
                    if (course.Gradebook.Items.Any(x => x.Identifier == item.GradeItemIdentifier.Value))
                    {
                        var gi = course.Gradebook.Items.Single(x => x.Identifier == item.GradeItemIdentifier.Value);
                        a.GradeItem = new GradeItem
                        {
                            Identifier = gi.Identifier,
                            Type = gi.Type.ToString(),
                            Name = gi.Name,
                            Format = gi.Format,
                            PassPercent = gi.PassPercent,
                        };
                    }
                }

                foreach (var prerequisite in prerequisites.Where(x => x.ObjectIdentifier == a.Identifier))
                    if (TryCreatePrerequisite(prerequisite, out var pModel))
                        a.Prerequisites.Add(pModel);

                foreach (var permission in permissions.Where(x => x.ObjectIdentifier == a.Identifier))
                    a.PrivacyGroups.Add(new PrivacyGroup
                    {
                        Identifier = permission.GroupIdentifier,
                        Type = "Group",
                        Name = permission.Group.GroupName
                    });

                module.Activities.Add(a);
            }
        }

        public static VActivityCompetency[] GetCourseCompetencies(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                return db.VActivityCompetencies
                    .Where(x => x.CourseIdentifier == course)
                    .ToArray();
            }
        }

        public static Guid? GetFirstCourseActivity(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                var activity = db.QActivities.AsNoTracking()
                    .Where(x => x.Module.Unit.CourseIdentifier == course)
                    .OrderBy(x => x.Module.Unit.UnitSequence)
                    .ThenBy(x => x.Module.ModuleSequence)
                    .ThenBy(x => x.ActivitySequence)
                    .FirstOrDefault();
                return activity?.ActivityIdentifier;
            }
        }

        public static Guid? GetFirstUnitActivity(Guid unit)
        {
            using (var db = new InternalDbContext())
            {
                var activity = db.QActivities.AsNoTracking()
                    .Where(x => x.Module.UnitIdentifier == unit)
                    .OrderBy(x => x.Module.ModuleSequence)
                    .ThenBy(x => x.ActivitySequence)
                    .FirstOrDefault();
                return activity?.ActivityIdentifier;
            }
        }

        public static Guid? GetFirstCourseModule(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                var module = db.QModules.AsNoTracking()
                    .Where(x => x.Unit.CourseIdentifier == course)
                    .OrderBy(x => x.ModuleSequence)
                    .FirstOrDefault();
                return module?.ModuleIdentifier;
            }
        }

        public static Guid? GetFirstCourseUnit(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                var unit = db.QUnits.AsNoTracking()
                    .Where(x => x.CourseIdentifier == course)
                    .OrderBy(x => x.UnitSequence)
                    .FirstOrDefault();
                return unit?.UnitIdentifier;
            }
        }

        private static bool TryCreatePrerequisite(QCoursePrerequisite entity, out Prerequisite model)
        {
            model = null;

            var condition = entity.TriggerChange.ToEnumNullable<PrerequisiteType>();
            if (!condition.HasValue)
                return false;

            model = new Prerequisite
            {
                Type = condition.Value,
                Condition = new PrerequisiteCondition
                {
                    Identifier = entity.TriggerIdentifier,
                    Type = entity.TriggerType,
                    ScoreFrom = entity.TriggerConditionScoreFrom,
                    ScoreThru = entity.TriggerConditionScoreThru
                },
                Lock = new BaseObject
                {
                    Identifier = entity.ObjectIdentifier,
                    Type = entity.ObjectType
                },
            };

            return true;
        }

        #endregion

        #region Select (User portal enrolled courses)

        public class MyRecordedCourses
        {
            public Guid CourseIdentifier { get; set; }
            public Guid? GradebookIdentifier { get; set; }
            public Guid LearnerIdentifier { get; set; }
            public Guid? OrganizationIdentifier { get; set; }
            public Guid? EnrollmentIdentifier { get; set; }
            public Guid? PeriodIdentifier { get; set; }
            public Guid? PageIdentifier { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public Guid? CredentialIdentifier { get; set; }

            public string CourseName { get; set; }
            public string EmaiCourseHooklBody { get; set; }
            public string CourseImage { get; set; }
            public string CourseSlug { get; set; }
            public string PageCourseImage { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementTitle { get; set; }
            public string CourseUrl { get; set; }

            public DateTimeOffset? EnrollmentStarted { get; set; }
            public DateTimeOffset? EnrollmentCompleted { get; set; }

            public bool IsCompleted { get; set; }
        }

        public static MyRecordedCourses[] GetMyEnrolledCourses(Guid user, Guid organization)
        {
            const string query = @"EXEC records.MyEnrolledCourses @UserIdentifier, @OrganizationIdentifier";

            object[] sqlParameters =
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@OrganizationIdentifier", organization),
            };

            using (var db = new InternalDbContext())
            {
                var list = db.Database
                    .SqlQuery<MyRecordedCourses>(query, sqlParameters).ToList().OrderByDescending(x => x.CourseName).ToArray();

                return list;
            }
        }

        public static int CountMyEnrolledCourses(Guid user, Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<int>(
                        "EXEC records.MyEnrolledCoursesCount @UserIdentifier, @OrganizationIdentifier",
                        new[]
                        {
                            new SqlParameter("@UserIdentifier", user),
                            new SqlParameter("@OrganizationIdentifier", organization),
                        })
                    .First();
            }
        }

        public static MyRecordedCourses[] GetMyCompletedCourses(Guid user, Guid organization)
        {
            const string query = @"EXEC records.MyCompletedCourses @UserIdentifier, @OrganizationIdentifier";

            object[] sqlParameters =
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@OrganizationIdentifier", organization),
            };

            using (var db = new InternalDbContext())
            {
                var list = db.Database
                    .SqlQuery<MyRecordedCourses>(query, sqlParameters)
                    .ToList().OrderByDescending(x => x.CourseName).ToArray();

                return list;
            }
        }

        public static void DeleteCatalogs(Guid organization, string name)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TCatalogs
                    .Where(x => x.OrganizationIdentifier == organization && x.CatalogName.Contains(name));

                if (list.Any())
                {
                    db.TCatalogs.RemoveRange(list);
                    db.SaveChanges();
                }
            }
        }

        public static List<TCatalog> GetCatalogs(Guid? organization, string name)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TCatalogs
                    .Include(x => x.Courses)
                    .AsNoTracking()
                    .AsQueryable();

                if (organization.HasValue)
                    list = list.Where(x => x.OrganizationIdentifier == organization);

                if (name != null)
                    list = list.Where(x => x.CatalogName.Contains(name));

                return list.OrderBy(x => x.CatalogName)
                    .ToList();
            }
        }

        public static List<CatalogSearchResult> SearchCatalogs(Guid organization, string catalogName)
        {
            using (var db = new InternalDbContext())
            {
                var queryable = db.TCatalogs
                    .AsNoTracking()
                    .Include(x => x.Courses)
                    .Where(x => x.OrganizationIdentifier == organization)
                    .AsQueryable();

                var list = queryable
                    .GroupJoin(
                        db.TPrograms,
                        catalog => catalog.CatalogIdentifier,
                        program => program.CatalogIdentifier,
                        (catalog, programs) => new CatalogSearchResult
                        {
                            CatalogIdentifier = catalog.CatalogIdentifier,
                            CatalogName = catalog.CatalogName,
                            IsHidden = catalog.IsHidden,
                            CourseCount = catalog.Courses.Count,
                            ProgramCount = programs.Count()
                        }
                    )
                    .AsQueryable();

                if (catalogName != null)
                    list = list.Where(x => x.CatalogName.Contains(catalogName));

                return list
                    .OrderBy(x => x.CatalogName)
                    .ToList();
            }
        }

        public static TCatalog GetCatalog(Guid? organization, string name)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TCatalogs.AsNoTracking().AsQueryable();

                if (organization.HasValue)
                    list = list.Where(x => x.OrganizationIdentifier == organization);

                if (name != null)
                    list = list.Where(x => x.CatalogName.Contains(name));

                return list.FirstOrDefault();
            }
        }

        public static TCatalog GetCatalog(Guid? organization, Guid catalogId)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TCatalogs.AsNoTracking().AsQueryable();

                if (organization.HasValue)
                    list = list.Where(x => x.OrganizationIdentifier == organization);

                list = list.Where(x => x.CatalogIdentifier == catalogId);

                return list.FirstOrDefault();
            }
        }

        public static int CountCatalogs(TCatalogFilter filter)
        {
            return GetCatalogs(filter.OrganizationIdentifier, filter.CatalogName).Count;
        }

        public static TCatalog SelectCatalog(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCatalogs.FirstOrDefault(x => x.CatalogIdentifier == id);
            }
        }

        public static List<LaunchCard> GetCatalogCourseList(Guid? catalog, ISecurityFramework identity, Func<string, Guid?, string> progress)
        {
            var list = new List<LaunchCard>();

            if (catalog == null)
                return list;

            using (var db = new InternalDbContext())
            {
                var courses = db.QCourses
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.CatalogIdentifier == catalog)
                    .OrderBy(x => x.CourseSequence)
                    .ThenBy(x => x.CourseName)
                    .ToList();

                foreach (var course in courses)
                {
                    if (!TGroupPermissionSearch.IsAccessAllowed(course.CourseIdentifier, identity))
                        continue;

                    var language = identity.Language;

                    var content = new TContentSearch().GetBlock(
                        course.CourseIdentifier,
                        language,
                        new[] { ContentLabel.ImageUrl, ContentLabel.Summary, ContentLabel.Title });

                    var item = new LaunchCard
                    {
                        Icon = course.CourseIcon,
                        Image = content.GetText("ImageUrl", language).IfNullOrEmpty(course.CourseImage),
                        Identifier = course.CourseIdentifier,
                        Url = $"ui/portal/learning/course/{course.CourseIdentifier}",
                        Title = content.Title.GetText(language).IfNullOrEmpty(course.CourseName),
                        Summary = content.Summary.GetText(language)
                    };

                    if (course.CourseFlagColor != null && course.CourseFlagText != null)
                        item.Flag = new Flag { Color = course.CourseFlagColor, Text = course.CourseFlagText };

                    item.Progress = new Flag { Text = progress(course.CourseHook, course.CourseIdentifier) };

                    list.Add(item);
                }
            }

            return list;
        }

        #endregion
    }
}