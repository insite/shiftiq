using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Courses;
using InSite.Domain.Foundations;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Courses
{
    public class Course2Deserializer
    {
        public class Result
        {
            public List<string> Warnings { get; } = new List<string>();
            public QCourse Course { get; set; }
            public List<QUnit> Units { get; } = new List<QUnit>();
            public List<QModule> Modules { get; } = new List<QModule>();
            public List<QActivity> Activities { get; } = new List<QActivity>();
            public List<TContent> Contents { get; } = new List<TContent>();
            public List<TGroupPermission> PrivacyGroups { get; } = new List<TGroupPermission>();
            public List<QActivityCompetency> Competencies { get; } = new List<QActivityCompetency>();
            public List<QCoursePrerequisite> Prerequisites { get; } = new List<QCoursePrerequisite>();
        }

        private class Prerequisite
        {
            public Guid ContainerIdentifier { get; set; }
            public string ContainerType { get; set; }
            public string TriggerChange { get; set; }
            public string TriggerType { get; set; }
            public string Trigger { get; set; }
            public int? TriggerConditionRangeFrom { get; set; }
            public int? TriggerConditionRangeThru { get; set; }
        }

        private CourseSerialized _sourceCourse;
        private ISecurityFramework _identity;
        private Result _result;
        private List<Prerequisite> _prerequisites;
        private Dictionary<string, Guid?> _groups;
        private Dictionary<string, Guid?> _banks;
        private Dictionary<string, Guid?> _questions;
        private Dictionary<string, Guid?> _forms;
        private Dictionary<string, Guid> _competencies;
        private HashSet<string> _usedSurveys;
        private HashSet<string> _usedForms;

        public Result Deserialize(string filePath)
        {
            var file = File.ReadAllText(filePath);

            _sourceCourse = JsonConvert.DeserializeObject<CourseSerialized>(file);
            _identity = CurrentSessionState.Identity;
            _result = new Result();
            _prerequisites = new List<Prerequisite>();
            _groups = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
            _banks = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
            _questions = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
            _forms = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
            _competencies = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            _usedSurveys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _usedForms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            CreateCourse();

            MapPrerequisites();

            return _result;
        }

        private void CreateCourse()
        {
            _result.Course = new QCourse
            {
                CourseAsset = Sequence.Increment(_identity.Organization.Identifier, SequenceType.Asset),
                CourseIdentifier = UniqueIdentifier.Create(),
                CourseName = _sourceCourse.Name,
                OrganizationIdentifier = _identity.Organization.Identifier,
                IsMultipleUnitsEnabled = _sourceCourse.AllowMultipleUnits,
                IsProgressReportEnabled = _sourceCourse.IsProgressReportEnabled,
                OutlineWidth = _sourceCourse.OutlineWidth,
                CourseCode = _sourceCourse.Code,
                CourseLabel = _sourceCourse.Label,
                CourseIcon = _sourceCourse.Icon,
                CourseImage = _sourceCourse.Image,
                CourseStyle = _sourceCourse.Style,
                CourseFlagText = _sourceCourse.FlagText,
                CourseFlagColor = _sourceCourse.FlagColor,
                FrameworkStandardIdentifier = LoadFramework(_sourceCourse.Framework),
            };

            AddContent(_result.Course.CourseIdentifier, "Course", _sourceCourse.Content);

            AddPrivacyGroups(_result.Course.CourseIdentifier, "Course", _sourceCourse.PrivacyGroups);

            if (_sourceCourse.Units != null)
            {
                for (int i = 0; i < _sourceCourse.Units.Count; i++)
                    CreateUnit(_sourceCourse.Units[i]);
            }
        }

        private void CreateUnit(UnitSerialized sourceUnit)
        {
            var unit = new QUnit
            {
                CourseIdentifier = _result.Course.CourseIdentifier,

                UnitAsset = Sequence.Increment(_identity.Organization.Identifier, SequenceType.Asset),
                UnitIdentifier = UniqueIdentifier.Create(),
                UnitName = sourceUnit.Name,
                UnitCode = sourceUnit.Code,
            };

            _result.Units.Add(unit);

            AddContent(unit.UnitIdentifier, "Unit", sourceUnit.Content);

            AddPrivacyGroups(unit.UnitIdentifier, "Unit", sourceUnit.PrivacyGroups);

            if (sourceUnit.Modules != null)
            {
                for (int i = 0; i < sourceUnit.Modules.Count; i++)
                    CreateModule(unit.UnitIdentifier, sourceUnit.Modules[i]);
            }
        }

        private void CreateModule(Guid unitIdentifier, ModuleSerialized sourceModule)
        {
            var module = new QModule
            {
                UnitIdentifier = unitIdentifier,

                ModuleAsset = Sequence.Increment(_identity.Organization.Identifier, SequenceType.Asset),
                ModuleIdentifier = UniqueIdentifier.Create(),
                ModuleName = sourceModule.Name,
                ModuleCode = sourceModule.Code,
            };

            _result.Modules.Add(module);

            AddContent(module.ModuleIdentifier, "Module", sourceModule.Content);
            AddPrerequisites(module.ModuleIdentifier, PrerequisiteObjectType.Module.ToString(), sourceModule.Prerequisites);
            AddPrivacyGroups(module.ModuleIdentifier, "Module", sourceModule.PrivacyGroups);

            if (sourceModule.Activities != null)
            {
                for (int i = 0; i < sourceModule.Activities.Count; i++)
                    CreateActivity(module.ModuleIdentifier, sourceModule.Activities[i]);
            }
        }

        private void CreateActivity(Guid moduleIdentifier, ActivitySerialized sourceActivity)
        {
            var activity = new QActivity
            {
                ActivityAsset = Sequence.Increment(_identity.Organization.Identifier, SequenceType.Asset),
                ActivityIdentifier = UniqueIdentifier.Create(),
                ActivityName = sourceActivity.Name,
                ActivityType = sourceActivity.Type,
                ModuleIdentifier = moduleIdentifier,
                ActivityAuthorDate = DateTime.Today,
                ActivityAuthorName = _identity.User.FullName,
                ActivityCode = sourceActivity.Code,
                ActivityIsAdaptive = sourceActivity.IsAdaptive,
                ActivityMinutes = sourceActivity.DurationMinutes,
                ActivityPlatform = sourceActivity.ContentDeliveryPlatform,
                RequirementCondition = sourceActivity.Requirement,
                ActivityUrl = sourceActivity.Url,
                ActivityUrlType = sourceActivity.UrlType,
                ActivityUrlTarget = sourceActivity.UrlTarget,
                AssessmentFormIdentifier = GetForm(sourceActivity.Assessment, true),
                SurveyFormIdentifier = GetSurvey(sourceActivity.Survey),
            };

            _result.Activities.Add(activity);

            AddContent(activity.ActivityIdentifier, "Activity", sourceActivity.Content);
            AddPrerequisites(activity.ActivityIdentifier, PrerequisiteObjectType.Activity.ToString(), sourceActivity.Prerequisites);
            AddPrivacyGroups(activity.ActivityIdentifier, "Activity", sourceActivity.PrivacyGroups);
            AddCompetencies(activity.ActivityIdentifier, sourceActivity.Competencies, sourceActivity.Name);
        }

        private void AddCompetencies(Guid activityIdentifier, List<CompetencySerialized> competencies, string activityName)
        {
            if (competencies == null || _result.Course.FrameworkStandardIdentifier == null)
                return;

            foreach (var competency in competencies)
            {
                if (!_competencies.TryGetValue(competency.Name, out var competencyIdentifier))
                {
                    _result.Warnings.Add($"Competency '{competency.Name}' is not found.");
                    continue;
                }

                if (_result.Competencies.Any(x => x.ActivityIdentifier == activityIdentifier && x.CompetencyStandardIdentifier == competencyIdentifier))
                {
                    _result.Warnings.Add($"Competency '{competency.Name}' is assigned to the activity '{activityName}' more than once.");
                    continue;
                }

                _result.Competencies.Add(new QActivityCompetency
                {
                    ActivityIdentifier = activityIdentifier,
                    CompetencyStandardIdentifier = competencyIdentifier,
                    CompetencyCode = competency.Code,
                    RelationshipType = competency.Type
                });
            }
        }

        private Guid? LoadFramework(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var framework = StandardSearch.SelectFirst(x => x.OrganizationIdentifier == _identity.Organization.Identifier && x.ContentName == name);
            if (framework == null)
            {
                _result.Warnings.Add($"The framework '{name}' is not found.");
                return null;
            }

            LoadCompetencies(framework.StandardIdentifier);

            return framework.StandardIdentifier;
        }

        private void LoadCompetencies(Guid parentIdentifier)
        {
            var standards = StandardSearch.Select(x => x.ParentStandardIdentifier == parentIdentifier);
            foreach (var standard in standards)
            {
                if (!string.IsNullOrEmpty(standard.ContentName)
                    && !_competencies.ContainsKey(standard.ContentName)
                    )
                {
                    _competencies.Add(standard.ContentName, standard.StandardIdentifier);
                }

                LoadCompetencies(standard.StandardIdentifier);
            }
        }

        private void AddPrivacyGroups(Guid containerIdentifier, string containerType, List<string> privacyGroups)
        {
            if (privacyGroups == null)
                return;

            foreach (var group in privacyGroups)
            {
                if (!_groups.TryGetValue(group, out var identifier))
                {
                    identifier = ServiceLocator.GroupSearch
                        .GetGroups(new QGroupFilter { OrganizationIdentifier = _identity.Organization.Identifier, GroupName = group })
                        .FirstOrDefault()?
                        .GroupIdentifier;

                    _groups.Add(group, identifier);

                    if (identifier == null)
                        _result.Warnings.Add($"The group '{group}' is not found.");
                }

                if (identifier == null)
                    continue;

                _result.PrivacyGroups.Add(new TGroupPermission
                {
                    ObjectIdentifier = containerIdentifier,
                    ObjectType = containerType,
                    PermissionIdentifier = UniqueIdentifier.Create(),
                    GroupIdentifier = identifier.Value,
                    PermissionGranted = DateTimeOffset.UtcNow,
                    PermissionGrantedBy = _identity.User.UserIdentifier
                });
            }
        }

        private void AddContent(Guid containerIdentifier, string containerType, List<CourseContentSerialized> contentList)
        {
            foreach (var content in contentList)
                AddContent(content.Label, content.Language, content.Text, content.Html);

            void AddContent(string label, string lang, string text, string html)
            {
                if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(html))
                    return;

                _result.Contents.Add(new TContent
                {
                    OrganizationIdentifier = _identity.Organization.Identifier,
                    ContainerIdentifier = containerIdentifier,
                    ContentIdentifier = UniqueIdentifier.Create(),
                    ContentLabel = label,
                    ContentLanguage = lang,
                    ContainerType = containerType,
                    ContentText = text,
                    ContentHtml = html
                });
            }
        }

        private void AddPrerequisites(Guid containerIdentifier, string containerType, List<PrerequisiteSerialized> sourcePrerequisites)
        {
            if (sourcePrerequisites == null)
                return;

            foreach (var source in sourcePrerequisites)
            {
                string triggerType;
                if (string.Equals(source.TriggerType, "Assessment Form", StringComparison.OrdinalIgnoreCase))
                    triggerType = TriggerType.AssessmentForm.ToString();
                else if (string.Equals(source.TriggerType, "Grade Item", StringComparison.OrdinalIgnoreCase))
                    triggerType = TriggerType.GradeItem.ToString();
                else if (string.Equals(source.TriggerType, "Assessment Question", StringComparison.OrdinalIgnoreCase))
                    triggerType = TriggerType.AssessmentQuestion.ToString();
                else
                    triggerType = source.TriggerType;

                _prerequisites.Add(new Prerequisite
                {
                    ContainerIdentifier = containerIdentifier,
                    ContainerType = containerType,
                    TriggerChange = source.TriggerChange,
                    TriggerType = triggerType,
                    Trigger = source.Trigger,
                    TriggerConditionRangeFrom = source.TriggerConditionScoreFrom,
                    TriggerConditionRangeThru= source.TriggerConditionScoreThru
                });
            }
        }

        private void MapPrerequisites()
        {
            foreach (var source in _prerequisites)
            {
                var triggerIdentifier = GetTriggerIdentifier(source.TriggerType, source.Trigger);
                if (triggerIdentifier == null)
                    continue;

                _result.Prerequisites.Add(new QCoursePrerequisite
                {
                    CourseIdentifier = _result.Course.CourseIdentifier,
                    CoursePrerequisiteIdentifier = UniqueIdentifier.Create(),
                    TriggerIdentifier = triggerIdentifier.Value,
                    TriggerChange = source.TriggerChange,
                    TriggerType = source.TriggerType,
                    ObjectType = source.ContainerType,
                    ObjectIdentifier = source.ContainerIdentifier
                });
            }
        }

        private Guid? GetBank(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (!_banks.TryGetValue(name, out var identifier))
            {
                var banks = ServiceLocator.BankSearch.GetBanks(new QBankFilter
                {
                    OrganizationIdentifier = _identity.Organization.Identifier,
                    BankName = name
                });

                if (banks.Count > 0)
                {
                    identifier = banks[0].BankIdentifier;

                    var questions = ServiceLocator.BankSearch.GetQuestions(new QBankQuestionFilter { BankIdentifier = identifier });
                    foreach (var question in questions)
                        _questions.Add($"{name}.{question.BankIndex}", question.QuestionIdentifier);
                }
                else
                {
                    identifier = null;
                    _result.Warnings.Add($"Bank '{name}' is not found.");
                }

                _banks.Add(name, identifier);
            }

            return identifier;
        }

        private Guid? GetQuestion(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (!_questions.TryGetValue(name, out var identifier))
            {
                var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2 || !int.TryParse(parts[1], out var _))
                {
                    identifier = null;
                    _result.Warnings.Add($"Invalid question definition: '{name}'.");
                }
                else
                {
                    var bankIdentifier = GetBank(parts[0]);
                    if (bankIdentifier == null)
                        identifier = null;
                    else if (!_questions.TryGetValue(name, out identifier))
                    {
                        identifier = null;
                        _result.Warnings.Add($"The question with index '{parts[1]}' is not found.");
                    }
                }

                if (identifier == null)
                    _questions.Add(name, identifier);
            }

            return identifier;
        }

        private Guid? GetForm(string name, bool checkUsedForms)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (checkUsedForms)
            {
                if (_usedForms.Contains(name))
                {
                    _result.Warnings.Add($"Form '{name}' is used more than once in this course.");
                    return null;
                }
                else
                {
                    _usedForms.Add(name);
                }
            }

            if (!_forms.TryGetValue(name, out var identifier))
            {
                identifier = ServiceLocator.BankSearch
                    .GetForms(new QBankFormFilter
                    {
                        OrganizationIdentifier = _identity.Organization.Identifier,
                        FormName = name
                    })
                    .FirstOrDefault()?.FormIdentifier;

                if (identifier == null)
                {
                    _result.Warnings.Add($"The form '{name}' is not found.");
                }
                else if (checkUsedForms && CourseSearch.ActivityExists(x => x.AssessmentFormIdentifier == identifier))
                {
                    _result.Warnings.Add($"Form '{name}' is already used by another course.");
                    identifier = null;
                }

                _forms.Add(name, identifier);
            }

            return identifier;
        }

        private Guid? GetTriggerIdentifier(string type, string trigger)
        {
            if (StringHelper.Equals(type, "Assessment Question"))
                return GetQuestion(trigger);

            if (StringHelper.Equals(type, "Assessment Form"))
                return GetForm(trigger, false);

            if (StringHelper.Equals(type, "Activity"))
                return _result.Activities.Find(x => StringHelper.Equals(x.ActivityName, trigger))?.ActivityIdentifier;

            return null;
        }

        private Guid? GetSurvey(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (_usedSurveys.Contains(name))
            {
                _result.Warnings.Add($"Form '{name}' is used more than once in this course.");
                return null;
            }

            var identifier = ServiceLocator.SurveySearch.GetSurveyFormByName(_identity.Organization.Identifier, name)?.SurveyFormIdentifier;

            if (identifier == null)
            {
                _result.Warnings.Add($"Form '{name}' is not found.");
            }
            else if (CourseSearch.ActivityExists(x => x.SurveyFormIdentifier == identifier))
            {
                _result.Warnings.Add($"Form '{name}' is already used by another course.");
                identifier = null;
            }

            _usedSurveys.Add(name);

            return identifier;
        }

        private Guid? GetProgram(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var programs = ProgramSearch.GetPrograms(new TProgramFilter { OrganizationIdentifier = _identity.Organization.Identifier, ProgramName = name });
            if (programs.Count > 0)
                return programs[0].ProgramIdentifier;

            _result.Warnings.Add($"The program '{name}' is not found.");

            return null;
        }
    }
}