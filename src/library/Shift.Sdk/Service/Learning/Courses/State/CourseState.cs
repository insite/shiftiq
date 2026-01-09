using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Courses
{
    [Serializable]
    public class CourseState : AggregateState
    {
        public static class Defaults
        {
            public const int CourseSequence = 0;
            public const bool IsHidden = false;
            public const bool IsMultipleUnitsEnabled = false;
            public const bool IsProgressReportEnabled = false;
            public const bool AllowDiscussion = false;
        }

        #region Properties

        [JsonIgnore]
        private Dictionary<Guid, Activity> Activities { get; set; }
        [JsonIgnore]
        private Dictionary<Guid, Module> Modules { get; set; }

        [JsonProperty, JsonConverter(typeof(CourseStateDictionaryConverter))]
        private Dictionary<CourseField, object> Values = new Dictionary<CourseField, object>();

        public Guid Identifier { get; set; }
        public Guid Organization { get; set; }
        public ContentContainer Content { get; set; }
        public List<Unit> Units { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public bool IsRemoved { get; set; }

        #endregion

        #region Helper methods

        public string GetTextValue(CourseField personField) => Values.TryGetValue(personField, out var value) ? (string)value : null;
        public bool? GetBoolValue(CourseField personField) => Values.TryGetValue(personField, out var value) ? (bool?)value : null;
        public int? GetIntValue(CourseField personField) => Values.TryGetValue(personField, out var value) ? (int?)value : null;
        public DateTimeOffset? GetDateOffsetValue(CourseField personField) => Values.TryGetValue(personField, out var value) ? (DateTimeOffset?)value : null;
        public Guid? GetGuidValue(CourseField personField) => Values.TryGetValue(personField, out var value) ? (Guid?)value : null;

        private void SetValue<T>(CourseField personField, T value, bool directlyModified)
        {
            var field = CourseFieldList.GetField(personField);
            if (value != null && field.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid person field: {personField}");

            if (directlyModified && !field.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {personField}");

            if (value == null)
            {
                if (field.Required)
                    throw new ArgumentNullException($"Field {personField} is a required field");

                Values.Remove(personField);
            }
            else
                Values[personField] = value;
        }

        private void DeleteModule(Guid moduleId)
        {
            if (!Modules.TryGetValue(moduleId, out var module))
                return;

            foreach (var activity in module.Activities)
                Activities.Remove(activity.Identifier);

            module.Unit.Modules.Remove(module);

            Modules.Remove(moduleId);
        }

        public Unit GetUnit(Guid unitId)
        {
            return Units.Find(x => x.Identifier == unitId);
        }

        public Module GetModule(Guid moduleId)
        {
            Modules.TryGetValue(moduleId, out var module);
            return module;
        }

        public Activity GetActivity(Guid activityId)
        {
            Activities.TryGetValue(activityId, out var activity);
            return activity;
        }

        public Activity GetActivityByGradeItem(Guid gradeItemId)
        {
            return Activities.Values
                .Where(x => x.GetGuidValue(ActivityField.GradeItemIdentifier) == gradeItemId)
                .FirstOrDefault();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Modules = new Dictionary<Guid, Module>();
            Activities = new Dictionary<Guid, Activity>();

            foreach (var unit in Units)
            {
                foreach (var module in unit.Modules)
                {
                    module.Unit = unit;
                    Modules.Add(module.Identifier, module);

                    foreach (var activity in module.Activities)
                    {
                        activity.Module = module;
                        Activities.Add(activity.Identifier, activity);
                    }
                }
            }
        }

        #endregion

        #region When

        public void When(CourseCreated e)
        {
            if (e.CourseContent == null)
                throw new ArgumentNullException("e.CourseContent");

            Identifier = e.AggregateIdentifier;
            Organization = e.OrganizationId;
            Units = new List<Unit>();
            Enrollments = new List<Enrollment>();

            Content = e.CourseContent.Clone();
            Content.CreateSnips();

            Activities = new Dictionary<Guid, Activity>();
            Modules = new Dictionary<Guid, Module>();

            SetValue(CourseField.CourseAsset, e.CourseAsset, false);
            SetValue(CourseField.CourseName, e.CourseName, false);

            SetValue(CourseField.CourseSequence, Defaults.CourseSequence, false);
            SetValue(CourseField.CourseIsHidden, Defaults.IsHidden, false);
            SetValue(CourseField.IsMultipleUnitsEnabled, Defaults.IsMultipleUnitsEnabled, false);
            SetValue(CourseField.IsProgressReportEnabled, Defaults.IsProgressReportEnabled, false);
            SetValue(CourseField.AllowDiscussion, Defaults.AllowDiscussion, false);
        }

        public void When(CourseActivitiesResequenced e)
        {
            var module = Modules[e.ModuleId];

            foreach (var activityAndSequence in e.Activities)
            {
                var activity = module.Activities.Find(x => x.Identifier == activityAndSequence.ActivityId);
                if (activity == null)
                    throw new ApplicationError($"Activity {activityAndSequence.ActivityId} is not found the module {module.Identifier}");

                activity.SetValue(ActivityField.ActivitySequence, activityAndSequence.Sequence, false);
            }
        }

        public void When(CourseActivityAdded e)
        {
            var module = Modules[e.ModuleId];

            var activity = new Activity
            {
                Identifier = e.ActivityId,
                Module = module,
                ActivityType = e.ActivityType,
                Prerequisites = new List<Prerequisite>(),
                Competencies = new List<ActivityCompetency>(),
            };

            var sequence = module.Activities.Count > 0
                ? module.Activities.Max(x => x.GetIntValue(ActivityField.ActivitySequence).Value) + 1
                : 1;

            activity.SetValue(ActivityField.ActivityAsset, e.ActivityAsset, false);
            activity.SetValue(ActivityField.ActivityName, e.ActivityName, false);
            activity.SetValue(ActivityField.ActivitySequence, sequence, false);

            activity.Content = e.ActivityContent.Clone();
            activity.Content.CreateSnips();

            activity.SetValue(ActivityField.ActivityIsMultilingual, Activity.Defaults.IsMultilingual, false);
            activity.SetValue(ActivityField.ActivityIsAdaptive, Activity.Defaults.IsAdaptive, false);
            activity.SetValue(ActivityField.ActivityIsDispatch, Activity.Defaults.IsDispatch, false);

            Activities.Add(e.ActivityId, activity);

            module.Activities.Add(activity);
        }

        public void When(CourseActivityMoved e)
        {
            var activity = GetActivity(e.ActivityId);
            if (activity == null)
                return;

            var destModule = GetModule(e.MoveToModuleId);
            if (destModule == null)
                return;

            var sourceModule = activity.Module;

            activity.Module = destModule;

            sourceModule.Activities.Remove(activity);
            destModule.Activities.Add(activity);
        }

        public void When(CourseActivityAssessmentFormConnected e)
        {
            Activities[e.ActivityId].SetValue(ActivityField.AssessmentFormIdentifier, e.AssessmentFormId, false);
        }

        public void When(CourseActivityCompetenciesAdded e)
        {
            if (e.Competencies == null)
                throw new ArgumentNullException("e.Competencies");

            var activity = Activities[e.ActivityId];

            foreach (var competency in e.Competencies)
            {
                var existing = activity.Competencies.Find(x => x.CompetencyStandardIdentifier == competency.CompetencyStandardIdentifier);
                if (existing != null)
                {
                    existing.CompetencyCode = competency.CompetencyCode;
                    existing.RelationshipType = competency.RelationshipType;
                }
                else
                    activity.Competencies.Add(competency.Clone());
            }
        }

        public void When(CourseActivityCompetenciesRemoved e)
        {
            if (e.CompetencyIds == null)
                throw new ArgumentNullException("e.CompetencyIds");

            var activity = Activities[e.ActivityId];

            foreach (var id in e.CompetencyIds)
            {
                var index = activity.Competencies.FindIndex(x => x.CompetencyStandardIdentifier == id);
                if (index >= 0)
                    activity.Competencies.RemoveAt(index);
            }
        }

        public void When(CourseActivityContentModified e)
        {
            if (e.ActivityContent == null)
                throw new ArgumentNullException("e.ActivityContent");

            var content = Activities[e.ActivityId].Content;

            content.Set(e.ActivityContent, ContentContainer.SetNullAction.Remove);
            content.CreateSnips();
        }

        public void When(CourseActivityFieldBoolModified e)
        {
            Activities[e.ActivityId].SetValue(e.ActivityField, e.Value, true);
        }

        public void When(CourseActivityFieldGuidModified e)
        {
            Activities[e.ActivityId].SetValue(e.ActivityField, e.Value, true);
        }

        public void When(CourseActivityFieldIntModified e)
        {
            Activities[e.ActivityId].SetValue(e.ActivityField, e.Value, true);
        }

        public void When(CourseActivityFieldTextModified e)
        {
            Activities[e.ActivityId].SetValue(e.ActivityField, e.Value, true);
        }

        public void When(CourseActivityFieldDateModified e)
        {
            Activities[e.ActivityId].SetValue(e.ActivityField, e.Value, true);
        }

        public void When(CourseActivityGradeItemConnected e)
        {
            Activities[e.ActivityId].SetValue(ActivityField.GradeItemIdentifier, e.GradeItemId, false);
        }

        public void When(CourseActivityPrerequisiteAdded e)
        {
            Activities[e.ActivityId].Prerequisites.Add(e.Prerequisite.Clone());
        }

        public void When(CourseActivityPrerequisiteRemoved e)
        {
            var activity = Activities[e.ActivityId];
            var index = activity.Prerequisites.FindIndex(x => x.Identifier == e.PrerequisiteId);

            if (index >= 0)
                activity.Prerequisites.RemoveAt(index);
        }

        public void When(CourseActivityLegacyPrerequisiteConnected e)
        {
            Activities[e.ActivityId].SetValue(ActivityField.PrerequisiteActivityIdentifier, e.PrerequisiteActivityId, false);
        }

        public void When(CourseActivityQuizConnected e)
        {
            Activities[e.ActivityId].SetValue(ActivityField.QuizIdentifier, e.QuizId, false);
        }

        public void When(CourseActivityRemoved e)
        {
            if (!Activities.TryGetValue(e.ActivityId, out var activity))
                return;

            activity.Module.Activities.Remove(activity);

            Activities.Remove(e.ActivityId);

            if (GetGuidValue(CourseField.CompletionActivityIdentifier) == e.ActivityId)
                SetValue(CourseField.CompletionActivityIdentifier, (Guid?)null, false);
        }

        public void When(CourseActivitySurveyFormConnected e)
        {
            Activities[e.ActivityId].SetValue(ActivityField.SurveyFormIdentifier, e.SurveyFormId, false);
        }

        public void When(CourseActivityTimestampsModified e)
        {
            var activity = Activities[e.ActivityId];
            activity.SetValue(ActivityField.Created, e.Created, false);
            activity.SetValue(ActivityField.CreatedBy, e.CreatedBy, false);
            activity.SetValue(ActivityField.Modified, e.Modified, false);
            activity.SetValue(ActivityField.ModifiedBy, e.ModifiedBy, false);
        }

        public void When(CourseActivityTypeModified e)
        {
            Activities[e.ActivityId].ActivityType = e.Type;
        }

        public void When(CourseActivityUrlModified e)
        {
            var activity = Activities[e.ActivityId];
            activity.SetValue(ActivityField.ActivityUrl, e.Url, false);
            activity.SetValue(ActivityField.ActivityUrlTarget, e.Target, false);
            activity.SetValue(ActivityField.ActivityUrlType, e.Type, false);
        }

        public void When(CourseCatalogConnected e)
        {
            SetValue(CourseField.CatalogIdentifier, e.CatalogId, false);
        }

        public void When(CourseEnrollmentCompleted e)
        {
            var enrollment = Enrollments.Find(x => x.Identifier == e.CourseEnrollmentId);
            enrollment.CourseCompleted = e.Completed;
        }

        public void When(CourseCompletionActivityConfigured e)
        {
            SetValue(CourseField.CompletionActivityIdentifier, e.ActivityId, false);
        }

        public void When(CourseContentModified e)
        {
            Content.Set(e.CourseContent, ContentContainer.SetNullAction.Remove);
            Content.CreateSnips();
        }

        public void When(CourseEnrollmentAdded e)
        {
            if (Enrollments.Any(x => x.LearnerUserIdentifier == e.LearnerUserId))
                throw new ArgumentException($"User {e.LearnerUserId} is already enrolled");

            Enrollments.Add(new Enrollment
            {
                Identifier = e.CourseEnrollmentId,
                LearnerUserIdentifier = e.LearnerUserId,
                CourseStarted = e.CourseStarted
            });
        }

        public void When(CourseEnrollmentIncreased e)
        {
            var enrollment = Enrollments.Find(x => x.Identifier == e.CourseEnrollmentId)
                ?? throw new ArgumentException($"Enrollment not found: {e.CourseEnrollmentId}");

            switch (e.Message)
            {
                case CourseEnrollmentMessageType.Stalled:
                    enrollment.MessageStalledSentCount++;
                    break;
                case CourseEnrollmentMessageType.Completed:
                    enrollment.MessageCompletedSentCount++;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: ${e.Message}");
            }
        }

        public void When(CourseEnrollmentModified e)
        {
            var enrollment = Enrollments.Find(x => x.Identifier == e.CourseEnrollmentId)
                ?? throw new ArgumentException($"Enrollment not found: {e.CourseEnrollmentId}");

            switch (e.Message)
            {
                case CourseEnrollmentMessageType.Stalled:
                    enrollment.MessageStalledSentCount = e.SentCount;
                    break;
                case CourseEnrollmentMessageType.Completed:
                    enrollment.MessageCompletedSentCount = e.SentCount;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: ${e.Message}");
            }
        }

        public void When(CourseFieldBoolModified e)
        {
            SetValue(e.CourseField, e.Value, true);
        }

        public void When(CourseFieldDateTimeOffsetModified e)
        {
            SetValue(e.CourseField, e.Value, true);
        }

        public void When(CourseFieldGuidModified e)
        {
            SetValue(e.CourseField, e.Value, true);
        }

        public void When(CourseFieldIntModified e)
        {
            SetValue(e.CourseField, e.Value, true);
        }

        public void When(CourseFieldTextModified e)
        {
            SetValue(e.CourseField, e.Value, true);
        }

       
        public void When(CourseFrameworkConnected e)
        {
            SetValue(CourseField.FrameworkStandardIdentifier, e.FrameworkStandardId, false);
        }

        public void When(CourseGradebookConnected e)
        {
            SetValue(CourseField.GradebookIdentifier, e.GradebookId, false);
        }

        public void When(CourseEnrollmentRemoved e)
        {
            var index = Enrollments.FindIndex(x => x.Identifier == e.CourseEnrollmentId);
            if (index >= 0)
                Enrollments.RemoveAt(index);
        }

        public void When(CourseMessageConnected e)
        {
            switch (e.MessageType)
            {
                case CourseMessageType.StalledToLearner:
                    SetValue(CourseField.StalledToLearnerMessageIdentifier, e.MessageId, false);
                    SetValue(CourseField.SendMessageStalledAfterDays, e.AfterDays, false);
                    SetValue(CourseField.SendMessageStalledMaxCount, e.MaxCount, false);
                    break;
                case CourseMessageType.StalledToAdministrator:
                    SetValue(CourseField.StalledToAdministratorMessageIdentifier, e.MessageId, false);
                    break;
                case CourseMessageType.CompletedToLearner:
                    SetValue(CourseField.CompletedToLearnerMessageIdentifier, e.MessageId, false);
                    break;
                case CourseMessageType.CompletedToAdministrator:
                    SetValue(CourseField.CompletedToAdministratorMessageIdentifier, e.MessageId, false);
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: {e.MessageType}");
            }
        }

        public void When(CourseModuleAdaptiveModified e)
        {
            Modules[e.ModuleId].ModuleIsAdaptive = e.IsAdaptive;
        }

        public void When(CourseModuleAdded e)
        {
            var unit = Units.Find(x => x.Identifier == e.UnitId);

            var sequence = unit.Modules.Count > 0
                ? unit.Modules.Max(x => x.ModuleSequence) + 1
                : 1;

            var module = new Module
            {
                Identifier = e.ModuleId,
                Unit = unit,
                ModuleAsset = e.ModuleAsset,
                ModuleName = e.ModuleName,
                ModuleSequence = sequence,
                ModuleIsAdaptive = Module.Defaults.IsAdaptive,
                Prerequisites = new List<Prerequisite>(),
                Activities = new List<Activity>()
            };

            module.Content = e.ModuleContent.Clone();
            module.Content.CreateSnips();

            Modules.Add(e.ModuleId, module);

            unit.Modules.Add(module);
        }

        public void When(CourseModuleMoved e)
        {
            var module = GetModule(e.ModuleId);
            if (module == null)
                return;

            var destUnit = GetUnit(e.MoveToUnitId);
            if (destUnit == null)
                return;

            var sourceUnit = module.Unit;

            module.Unit = destUnit;

            sourceUnit.Modules.Remove(module);
            destUnit.Modules.Add(module);
        }

        public void When(CourseModuleCodeModified e)
        {
            Modules[e.ModuleId].ModuleCode = e.ModuleCode;
        }

        public void When(CourseModuleContentModified e)
        {
            if (e.ModuleContent == null)
                throw new ArgumentNullException("e.ModuleContent");

            var module = Modules[e.ModuleId];
            module.Content.Set(e.ModuleContent, ContentContainer.SetNullAction.Remove);
            module.Content.CreateSnips();
        }

        public void When(CourseModuleImageModified e)
        {
            Modules[e.ModuleId].ModuleImage = e.Image;
        }

        public void When(CourseModulePrerequisiteAdded e)
        {
            Modules[e.ModuleId].Prerequisites.Add(e.Prerequisite.Clone());
        }

        public void When(CourseModulePrerequisiteDeterminerModified e)
        {
            Modules[e.ModuleId].PrerequisiteDeterminer = e.PrerequisiteDeterminer;
        }

        public void When(CourseModulePrerequisiteRemoved e)
        {
            var module = Modules[e.ModuleId];
            var index = module.Prerequisites.FindIndex(x => x.Identifier == e.PrerequisiteId);

            if (index >= 0)
                module.Prerequisites.RemoveAt(index);
        }

        public void When(CourseModuleRemoved e)
        {
            DeleteModule(e.ModuleId);
        }

        public void When(CourseModuleRenamed e)
        {
            Modules[e.ModuleId].ModuleName = e.ModuleName;
        }

        public void When(CourseModuleSequenceModified e)
        {
            Modules[e.ModuleId].ModuleSequence = e.ModuleSequence;
        }

        public void When(CourseModuleSourceModified e)
        {
            Modules[e.ModuleId].SourceIdentifier = e.Source;
        }

        public void When(CourseModuleTimestampsModified e)
        {
            var module = Modules[e.ModuleId];
            module.Created = e.Created;
            module.CreatedBy = e.CreatedBy;
            module.Modified = e.Modified;
            module.ModifiedBy = e.ModifiedBy;
        }

        public void When(CourseDeleted e)
        {
            IsRemoved = true;
        }

        public void When(CourseTimestampsModified e)
        {
            SetValue(CourseField.Created, e.Created, false);
            SetValue(CourseField.CreatedBy, e.CreatedBy, false);
            SetValue(CourseField.Modified, e.Modified, false);
            SetValue(CourseField.ModifiedBy, e.ModifiedBy, false);
        }

        public void When(CourseUnitAdaptiveModified e)
        {
            Units.Find(x => x.Identifier == e.UnitId).UnitIsAdaptive = e.IsAdaptive;
        }

        public void When(CourseUnitAdded e)
        {
            var sequence = Units.Count > 0
                ? Units.Max(x => x.UnitSequence) + 1
                : 1;

            var unit = new Unit
            {
                Identifier = e.UnitId,
                UnitAsset = e.UnitAsset,
                UnitName = e.UnitName,
                UnitSequence = sequence,
                UnitIsAdaptive = Unit.Defaults.IsAdaptive,
                Prerequisites = new List<Prerequisite>(),
                Modules = new List<Module>()
            };

            unit.Content = e.UnitContent.Clone();
            unit.Content.CreateSnips();

            Units.Add(unit);
        }

        public void When(CourseUnitCodeModified e)
        {
            Units.Find(x => x.Identifier == e.UnitId).UnitCode = e.UnitCode;
        }

        public void When(CourseUnitContentModified e)
        {
            if (e.UnitContent == null)
                throw new ArgumentNullException("e.UnitContent");

            var unit = Units.Find(x => x.Identifier == e.UnitId);
            unit.Content.Set(e.UnitContent, ContentContainer.SetNullAction.Remove);
        }

        public void When(CourseUnitPrerequisiteAdded e)
        {
            Units.Find(x => x.Identifier == e.UnitId).Prerequisites.Add(e.Prerequisite.Clone());
        }

        public void When(CourseUnitPrerequisiteDeterminerModified e)
        {
            Units.Find(x => x.Identifier == e.UnitId).PrerequisiteDeterminer = e.PrerequisiteDeterminer;
        }

        public void When(CourseUnitPrerequisiteRemoved e)
        {
            var unit = Units.Find(x => x.Identifier == e.UnitId);
            var index = unit.Prerequisites.FindIndex(x => x.Identifier == e.PrerequisiteId);

            if (index >= 0)
                unit.Prerequisites.RemoveAt(index);
        }

        public void When(CourseUnitRemoved e)
        {
            var unit = Units.Find(x => x.Identifier == e.UnitId);
            if (unit == null)
                return;

            while (unit.Modules.Count > 0)
                DeleteModule(unit.Modules[0].Identifier);

            Units.Remove(unit);
        }

        public void When(CourseUnitRenamed e)
        {
            Units.Find(x => x.Identifier == e.UnitId).UnitName = e.UnitName;
        }

        public void When(CourseUnitSequenceModified e)
        {
            Units.Find(x => x.Identifier == e.UnitId).UnitSequence = e.UnitSequence;
        }

        public void When(CourseUnitSourceModified e)
        {
            Units.Find(x => x.Identifier == e.UnitId).SourceIdentifier = e.Source;
        }

        public void When(CourseUnitTimestampsModified e)
        {
            var unit = Units.Find(x => x.Identifier == e.UnitId);
            unit.Created = e.Created;
            unit.CreatedBy = e.CreatedBy;
            unit.Modified = e.Modified;
            unit.ModifiedBy = e.ModifiedBy;
        }

        #endregion
    }
}
