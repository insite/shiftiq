using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Persistence;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Courses
{
    public class Course2Serializer
    {
        private QCourse _course;
        private List<QUnit> _units;
        private List<QModule> _modules;
        private List<QActivity> _activities;
        private List<TContent> _contents;
        private List<TGroupPermission> _privacyGroups;
        private List<TActivityCompetency> _activityCompetencies;
        private List<QCoursePrerequisite> _prerequisites;

        private Dictionary<Guid, string> _privacyGroupMap = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _standardMap = new Dictionary<Guid, string>();

        private Course2Serializer()
        {
        }

        public static byte[] Serialize(Guid courseIdentifier, IContractResolver resolver = null)
        {
            var serializer = new Course2Serializer();
            serializer.LoadData(courseIdentifier);

            var courseSerialized = serializer.GetCourseSerialized();

            var json = JsonConvert.SerializeObject(courseSerialized, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = resolver
            });

            return Encoding.UTF8.GetBytes(json);
        }

        private void LoadData(Guid courseIdentifier)
        {
            _course = CourseSearch.SelectCourse(courseIdentifier);

            _units = CourseSearch.BindUnits(x => x, x => x.CourseIdentifier == courseIdentifier)
                .OrderBy(x => x.UnitSequence)
                .ThenBy(x => x.UnitName)
                .ToList()
                .ToList();

            var unitIds = _units.Select(x => x.UnitIdentifier).ToList();

            _modules = unitIds.Count > 0
                ? CourseSearch.BindModules(x => x, x => unitIds.Contains(x.UnitIdentifier))
                    .OrderBy(x => x.ModuleSequence)
                    .ThenBy(x => x.ModuleName)
                    .ToList()
                : new List<QModule>();

            var moduleIds = _modules.Select(x => x.ModuleIdentifier).ToList();

            _activities = moduleIds.Count > 0
                ? CourseSearch.BindActivities(x => x, x => moduleIds.Contains(x.ModuleIdentifier))
                    .OrderBy(x => x.ActivitySequence)
                    .ThenBy(x => x.ActivityName)
                    .ToList()
                : new List<QActivity>();

            var activityIds = _activities.Select(x => x.ActivityIdentifier).ToList();

            _activityCompetencies = activityIds.Count > 0
                ? CourseSearch.SelectActivityCompetencies(activityIds)
                : new List<TActivityCompetency>();

            var allIds = new List<Guid> { _course.CourseIdentifier };
            allIds.AddRange(unitIds);
            allIds.AddRange(moduleIds);
            allIds.AddRange(activityIds);

            _contents = ServiceLocator.ContentSearch.SelectContainers(x => allIds.Contains(x.ContainerIdentifier)).ToList();
            _privacyGroups = ServiceLocator.ContentSearch.SelectPrivacyGroup(x => allIds.Contains(x.ObjectIdentifier)).ToList();

            _prerequisites = ServiceLocator.CourseSearch
                .GetPrerequisites(courseIdentifier)
                .Where(x => allIds.Contains(x.CoursePrerequisiteIdentifier))
                .ToList();
        }

        private CourseSerialized GetCourseSerialized()
        {
            var courseSerialized = new CourseSerialized
            {
                Code = _course.CourseCode,
                Name = _course.CourseName + " - Copy",
                Framework = GetStandardName(_course.FrameworkStandardIdentifier),
                Icon = _course.CourseIcon,
                Image = _course.CourseImage,
                Label = _course.CourseLabel,
                Slug = _course.CourseSlug,
                Style = _course.CourseStyle,
                FlagColor = _course.CourseFlagColor,
                FlagText = _course.CourseFlagText,
                AllowMultipleUnits = _course.IsMultipleUnitsEnabled,
                IsProgressReportEnabled = _course.IsProgressReportEnabled,
                OutlineWidth = _course.OutlineWidth,
                Content = GetContent(_course.CourseIdentifier),
                PrivacyGroups = GetPrivacyGroups(_course.CourseIdentifier)
            };

            var titleContent = courseSerialized.Content.Find(x => StringHelper.Equals(x.Language, "en") && StringHelper.Equals(x.Label, "Title"));
            if (titleContent != null)
                titleContent.Text += " - Copy";

            if (_units.Count > 0)
            {
                courseSerialized.Units = new List<UnitSerialized>();

                foreach (var unit in _units)
                    courseSerialized.Units.Add(GetUnitSerialized(unit));
            }

            return courseSerialized;
        }

        private UnitSerialized GetUnitSerialized(QUnit unit)
        {
            var unitSerialized = new UnitSerialized
            {
                Code = unit.UnitCode,
                Name = unit.UnitName,
                Content = GetContent(unit.UnitIdentifier),
                Prerequisites = GetPrerequisites(unit.UnitIdentifier),
                PrivacyGroups = GetPrivacyGroups(unit.UnitIdentifier)
            };

            var modules = _modules
                .Where(x => x.UnitIdentifier == unit.UnitIdentifier)
                .ToList();

            if (modules.Count > 0)
            {
                unitSerialized.Modules = new List<ModuleSerialized>();

                foreach (var module in modules)
                    unitSerialized.Modules.Add(GetModuleSerialized(module));
            }

            return unitSerialized;
        }

        private ModuleSerialized GetModuleSerialized(QModule module)
        {
            var moduleSerialized = new ModuleSerialized
            {
                Code = module.ModuleCode,
                Name = module.ModuleName,
                Content = GetContent(module.ModuleIdentifier),
                Prerequisites = GetPrerequisites(module.ModuleIdentifier),
                PrivacyGroups = GetPrivacyGroups(module.ModuleIdentifier)
            };

            var activities = _activities
                .Where(x => x.ModuleIdentifier == module.ModuleIdentifier)
                .ToList();

            if (activities.Count > 0)
            {
                moduleSerialized.Activities = new List<ActivitySerialized>();

                foreach (var activity in activities)
                    moduleSerialized.Activities.Add(GetActivitySerialized(activity));
            }

            return moduleSerialized;
        }

        private ActivitySerialized GetActivitySerialized(QActivity activity)
        {
            var activitySerialized = new ActivitySerialized
            {
                Code = activity.ActivityCode,
                Name = activity.ActivityName,
                Type = activity.ActivityType,
                Content = GetContent(activity.ActivityIdentifier),
                Url = activity.ActivityUrl,
                UrlType = activity.ActivityUrlType,
                UrlTarget = activity.ActivityUrlTarget,
                ContentDeliveryPlatform = activity.ActivityPlatform,
                DurationMinutes = activity.ActivityMinutes,
                IsAdaptive = activity.ActivityIsAdaptive,
                Requirement = activity.RequirementCondition,
                Assessment = GetBankFormName(activity.AssessmentFormIdentifier),
                Survey = GetSurveyFormName(activity.SurveyFormIdentifier),
                Prerequisites = GetPrerequisites(activity.ActivityIdentifier),
                PrivacyGroups = GetPrivacyGroups(activity.ActivityIdentifier)
            };

            var competencies = _activityCompetencies
                .Where(x => x.ActivityIdentifier == activity.ActivityIdentifier)
                .ToList();

            if (competencies.Count > 0)
            {
                activitySerialized.Competencies = new List<CompetencySerialized>();

                foreach (var competency in competencies)
                {
                    var name = GetStandardName(competency.CompetencyIdentifier);
                    if (name == null)
                        continue;

                    activitySerialized.Competencies.Add(new CompetencySerialized
                    {
                        Code = competency.CompetencyCode,
                        Name = name,
                        Type = competency.RelationshipType
                    });
                }
            }

            return activitySerialized;
        }

        private List<PrerequisiteSerialized> GetPrerequisites(Guid containerIdentifier)
        {
            var prerequisites = _prerequisites
                .Where(x => x.ObjectIdentifier == containerIdentifier)
                .ToList();

            if (prerequisites.Count == 0)
                return null;

            var result = new List<PrerequisiteSerialized>();
            foreach (var p in prerequisites)
            {
                var trigger = GetTriggerName(p.TriggerType, p.TriggerIdentifier);
                if (trigger == null)
                    continue;

                result.Add(new PrerequisiteSerialized
                {
                    TriggerChange = p.TriggerChange,
                    TriggerType = p.TriggerType,
                    TriggerConditionScoreFrom = p.TriggerConditionScoreFrom,
                    TriggerConditionScoreThru = p.TriggerConditionScoreThru,
                    Trigger = trigger
                });
            }

            return result.Count > 0 ? result : null;
        }

        private string GetTriggerName(string type, Guid identifier)
        {
            if (StringHelper.Equals(type, "Assessment Question"))
            {
                var question = ServiceLocator.BankSearch.GetQuestion(identifier);
                if (question == null)
                    return null;

                var bank = ServiceLocator.BankSearch.GetBank(question.BankIdentifier);
                return $"{bank.BankName}.{question.BankIndex}";
            }

            if (StringHelper.Equals(type, "Assessment Form"))
                return ServiceLocator.BankSearch.GetForm(identifier)?.FormName;

            if (StringHelper.Equals(type, "Activity"))
                return _activities.Find(x => x.ActivityIdentifier == identifier)?.ActivityName;

            return null;
        }

        private List<string> GetPrivacyGroups(Guid containerIdentifier)
        {
            var privacyGroups = _privacyGroups
                .Where(x => x.ObjectIdentifier == containerIdentifier)
                .ToList();

            if (privacyGroups.Count == 0)
                return null;

            var result = new List<string>();
            foreach (var o in privacyGroups)
            {
                if (!_privacyGroupMap.TryGetValue(o.GroupIdentifier, out var name))
                {
                    name = ServiceLocator.GroupSearch.GetGroup(o.GroupIdentifier)?.GroupName;
                    _privacyGroupMap.Add(o.GroupIdentifier, name);
                }

                if (name != null)
                    result.Add(name);
            }

            return result;
        }

        private string GetStandardName(Guid? identifier)
        {
            if (identifier == null)
                return null;

            if (!_standardMap.TryGetValue(identifier.Value, out var name))
            {
                name = StandardSearch.SelectFirst(x => x.StandardIdentifier == identifier)?.ContentName;
                _standardMap.Add(identifier.Value, name);
            }

            return name;
        }

        private static string GetProgramName(Guid? programIdentifier)
        {
            return programIdentifier.HasValue
                ? ProgramSearch.GetProgram(programIdentifier.Value)?.ProgramName
                : null;
        }

        private static string GetBankFormName(Guid? formIdentifier)
        {
            return formIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetForm(formIdentifier.Value)?.FormName
                : null;
        }

        private static string GetSurveyFormName(Guid? formIdentifier)
        {
            return formIdentifier.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyForm(formIdentifier.Value)?.SurveyFormName
                : null;
        }

        private List<CourseContentSerialized> GetContent(Guid containerIdentifier)
        {
            return _contents
                .Where(x => x.ContainerIdentifier == containerIdentifier)
                .Select(x => new CourseContentSerialized
                {
                    Language = x.ContentLanguage,
                    Label = x.ContentLabel,
                    Text = x.ContentText,
                    Html = x.ContentHtml
                })
                .ToList();
        }
    }
}