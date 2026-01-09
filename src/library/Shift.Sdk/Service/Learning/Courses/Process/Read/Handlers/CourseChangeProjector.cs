using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Read
{
    /// <summary>
    /// Implements the projector for Person changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class CourseChangeProjector
    {
        private readonly ICourseStore _courseStore;

        public CourseChangeProjector(IChangeQueue publisher, ICourseStore courseStore)
        {
            _courseStore = courseStore;

            publisher.Subscribe<CourseCreated>(Handle);
            publisher.Subscribe<CourseDeleted>(Handle);
            publisher.Subscribe<CourseTimestampsModified>(Handle);
            publisher.Subscribe<CourseCatalogConnected>(Handle);
            publisher.Subscribe<CourseEnrollmentCompleted>(Handle);
            publisher.Subscribe<CourseCompletionActivityConfigured>(Handle);
            publisher.Subscribe<CourseContentModified>(Handle);
            publisher.Subscribe<CourseEnrollmentAdded>(Handle);
            publisher.Subscribe<CourseEnrollmentIncreased>(Handle);
            publisher.Subscribe<CourseEnrollmentModified>(Handle);
            publisher.Subscribe<CourseFieldBoolModified>(Handle);
            publisher.Subscribe<CourseFieldDateTimeOffsetModified>(Handle);
            publisher.Subscribe<CourseFieldGuidModified>(Handle);
            publisher.Subscribe<CourseFieldIntModified>(Handle);
            publisher.Subscribe<CourseFieldTextModified>(Handle);
            publisher.Subscribe<CourseFrameworkConnected>(Handle);
            publisher.Subscribe<CourseGradebookConnected>(Handle);
            publisher.Subscribe<CourseEnrollmentRemoved>(Handle);
            publisher.Subscribe<CourseMessageConnected>(Handle);
            publisher.Subscribe<CourseActivitiesResequenced>(Handle);
            publisher.Subscribe<CourseActivityAdded>(Handle);
            publisher.Subscribe<CourseActivityMoved>(Handle);
            publisher.Subscribe<CourseActivityAssessmentFormConnected>(Handle);
            publisher.Subscribe<CourseActivityCompetenciesAdded>(Handle);
            publisher.Subscribe<CourseActivityCompetenciesRemoved>(Handle);
            publisher.Subscribe<CourseActivityContentModified>(Handle);
            publisher.Subscribe<CourseActivityFieldBoolModified>(Handle);
            publisher.Subscribe<CourseActivityFieldGuidModified>(Handle);
            publisher.Subscribe<CourseActivityFieldIntModified>(Handle);
            publisher.Subscribe<CourseActivityFieldTextModified>(Handle);
            publisher.Subscribe<CourseActivityFieldDateModified>(Handle);
            publisher.Subscribe<CourseActivityGradeItemConnected>(Handle);
            publisher.Subscribe<CourseActivityLegacyPrerequisiteConnected>(Handle);
            publisher.Subscribe<CourseActivityPrerequisiteAdded>(Handle);
            publisher.Subscribe<CourseActivityPrerequisiteRemoved>(Handle);
            publisher.Subscribe<CourseActivityQuizConnected>(Handle);
            publisher.Subscribe<CourseActivityRemoved>(Handle);
            publisher.Subscribe<CourseActivitySurveyFormConnected>(Handle);
            publisher.Subscribe<CourseActivityTimestampsModified>(Handle);
            publisher.Subscribe<CourseActivityTypeModified>(Handle);
            publisher.Subscribe<CourseActivityUrlModified>(Handle);
            publisher.Subscribe<CourseModuleSourceModified>(Handle);
            publisher.Subscribe<CourseModuleTimestampsModified>(Handle);
            publisher.Subscribe<CourseModuleAdaptiveModified>(Handle);
            publisher.Subscribe<CourseModuleAdded>(Handle);
            publisher.Subscribe<CourseModuleMoved>(Handle);
            publisher.Subscribe<CourseModuleCodeModified>(Handle);
            publisher.Subscribe<CourseModuleContentModified>(Handle);
            publisher.Subscribe<CourseModuleImageModified>(Handle);
            publisher.Subscribe<CourseModulePrerequisiteAdded>(Handle);
            publisher.Subscribe<CourseModulePrerequisiteDeterminerModified>(Handle);
            publisher.Subscribe<CourseModulePrerequisiteRemoved>(Handle);
            publisher.Subscribe<CourseModuleRemoved>(Handle);
            publisher.Subscribe<CourseModuleRenamed>(Handle);
            publisher.Subscribe<CourseModuleSequenceModified>(Handle);
            publisher.Subscribe<CourseUnitAdaptiveModified>(Handle);
            publisher.Subscribe<CourseUnitAdded>(Handle);
            publisher.Subscribe<CourseUnitCodeModified>(Handle);
            publisher.Subscribe<CourseUnitContentModified>(Handle);
            publisher.Subscribe<CourseUnitPrerequisiteAdded>(Handle);
            publisher.Subscribe<CourseUnitPrerequisiteDeterminerModified>(Handle);
            publisher.Subscribe<CourseUnitPrerequisiteRemoved>(Handle);
            publisher.Subscribe<CourseUnitRemoved>(Handle);
            publisher.Subscribe<CourseUnitRenamed>(Handle);
            publisher.Subscribe<CourseUnitSequenceModified>(Handle);
            publisher.Subscribe<CourseUnitSourceModified>(Handle);
            publisher.Subscribe<CourseUnitTimestampsModified>(Handle);
        }

        public void Handle(CourseCreated e)
            => _courseStore.InsertCourse(e);

        public void Handle(CourseDeleted e)
            => _courseStore.RemoveCourse(e);

        public void Handle(CourseTimestampsModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                x.Created = e.Created;
                x.CreatedBy = e.CreatedBy;
                x.Modified = e.Modified;
                x.ModifiedBy = e.ModifiedBy;
            });
        }

        public void Handle(CourseCatalogConnected e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                x.CatalogIdentifier = e.CatalogId;
            });
        }

        public void Handle(CourseEnrollmentCompleted e)
        {
            _courseStore.ModifyEnrollment(e, e.CourseEnrollmentId, x =>
            {
                x.CourseCompleted = e.Completed;
            });
        }

        public void Handle(CourseCompletionActivityConfigured e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                x.CompletionActivityIdentifier = e.ActivityId;
            });
        }

        public void Handle(CourseContentModified e)
        {
            _courseStore.ModifyCourse(e, e.CourseContent, null);
        }

        public void Handle(CourseEnrollmentAdded e)
        {
            _courseStore.InsertEnrollment(e);
        }

        public void Handle(CourseEnrollmentRemoved e)
        {
            _courseStore.RemoveEnrollment(e);
        }

        public void Handle(CourseEnrollmentIncreased e)
        {
            _courseStore.ModifyEnrollment(e, e.CourseEnrollmentId, x =>
            {
                switch (e.Message)
                {
                    case CourseEnrollmentMessageType.Stalled:
                        x.MessageStalledSentCount++;
                        break;
                    case CourseEnrollmentMessageType.Completed:
                        x.MessageCompletedSentCount++;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported message: ${e.Message}");
                }
            });
        }

        public void Handle(CourseEnrollmentModified e)
        {
            _courseStore.ModifyEnrollment(e, e.CourseEnrollmentId, x =>
            {
                switch (e.Message)
                {
                    case CourseEnrollmentMessageType.Stalled:
                        x.MessageStalledSentCount = e.SentCount;
                        break;
                    case CourseEnrollmentMessageType.Completed:
                        x.MessageCompletedSentCount = e.SentCount;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported message: ${e.Message}");
                }
            });
        }

        public void Handle(CourseFieldBoolModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.CourseField)
                {
                    case CourseField.AllowDiscussion:
                        x.AllowDiscussion = e.Value.Value;
                        break;
                    case CourseField.CourseIsHidden:
                        x.CourseIsHidden = e.Value.Value;
                        break;
                    case CourseField.IsMultipleUnitsEnabled:
                        x.IsMultipleUnitsEnabled = e.Value.Value;
                        break;
                    case CourseField.IsProgressReportEnabled:
                        x.IsProgressReportEnabled = e.Value.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported bool field: ${e.CourseField}");
                }
            });
        }

        public void Handle(CourseFieldDateTimeOffsetModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.CourseField)
                {
                    case CourseField.Closed:
                        x.Closed = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported DateTimeOffset field: ${e.CourseField}");
                }
            });
        }

        public void Handle(CourseFieldGuidModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.CourseField)
                {
                    case CourseField.SourceIdentifier:
                        x.SourceIdentifier = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported Guid field: ${e.CourseField}");
                }
            });
        }

        public void Handle(CourseFieldIntModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.CourseField)
                {
                    case CourseField.CourseSequence:
                        x.CourseSequence = e.Value;
                        break;
                    case CourseField.OutlineWidth:
                        x.OutlineWidth = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported int field: ${e.CourseField}");
                }
            });
        }

        public void Handle(CourseFieldTextModified e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.CourseField)
                {
                    case CourseField.CourseCode:
                        x.CourseCode = e.Value;
                        break;
                    case CourseField.CourseDescription:
                        x.CourseDescription = e.Value;
                        break;
                    case CourseField.CourseHook:
                        x.CourseHook = e.Value;
                        break;
                    case CourseField.CourseIcon:
                        x.CourseIcon = e.Value;
                        break;
                    case CourseField.CourseImage:
                        x.CourseImage = e.Value;
                        break;
                    case CourseField.CourseLabel:
                        x.CourseLabel = e.Value;
                        break;
                    case CourseField.CourseLevel:
                        x.CourseLevel = e.Value;
                        break;
                    case CourseField.CourseName:
                        x.CourseName = e.Value;
                        break;
                    case CourseField.CoursePlatform:
                        x.CoursePlatform = e.Value;
                        break;
                    case CourseField.CourseProgram:
                        x.CourseProgram = e.Value;
                        break;
                    case CourseField.CourseSlug:
                        x.CourseSlug = e.Value;
                        break;
                    case CourseField.CourseFlagColor:
                        x.CourseFlagColor = e.Value;
                        break;
                    case CourseField.CourseFlagText:
                        x.CourseFlagText = e.Value;
                        break;
                    case CourseField.CourseStyle:
                        x.CourseStyle = e.Value;
                        break;
                    case CourseField.CourseUnit:
                        x.CourseUnit = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported text field: ${e.CourseField}");
                }
            });
        }

        public void Handle(CourseFrameworkConnected e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                x.FrameworkStandardIdentifier = e.FrameworkStandardId;
            });
        }

        public void Handle(CourseGradebookConnected e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                x.GradebookIdentifier = e.GradebookId;
            });
        }

        public void Handle(CourseMessageConnected e)
        {
            _courseStore.ModifyCourse(e, null, x =>
            {
                switch (e.MessageType)
                {
                    case CourseMessageType.StalledToLearner:
                        x.StalledToLearnerMessageIdentifier = e.MessageId;
                        x.SendMessageStalledAfterDays = e.AfterDays;
                        x.SendMessageStalledMaxCount = e.MaxCount;
                        break;
                    case CourseMessageType.StalledToAdministrator:
                        x.StalledToAdministratorMessageIdentifier =  e.MessageId;
                        break;
                    case CourseMessageType.CompletedToLearner:
                        x.CompletedToLearnerMessageIdentifier = e.MessageId;
                        break;
                    case CourseMessageType.CompletedToAdministrator:
                        x.CompletedToAdministratorMessageIdentifier = e.MessageId;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported message: {e.MessageType}");
                }
            });
        }

        public void Handle(CourseActivitiesResequenced e)
        {
            foreach (var activityAndSequence in e.Activities)
            {
                _courseStore.ModifyActivity(e, activityAndSequence.ActivityId, null, x =>
                {
                    x.ActivitySequence = activityAndSequence.Sequence;
                });
            }
        }

        public void Handle(CourseActivityAdded e)
        {
            _courseStore.InsertActivity(e);
        }

        public void Handle(CourseActivityMoved e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.ModuleIdentifier = e.MoveToModuleId;
            });
        }

        public void Handle(CourseActivityAssessmentFormConnected e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.AssessmentFormIdentifier = e.AssessmentFormId;
            });
        }

        public void Handle(CourseActivityCompetenciesAdded e)
        {
            _courseStore.InsertCompetencies(e);
        }

        public void Handle(CourseActivityCompetenciesRemoved e)
        {
            _courseStore.RemoveCompetencies(e);
        }

        public void Handle(CourseActivityContentModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, e.ActivityContent, null);
        }

        public void Handle(CourseActivityFieldBoolModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                switch (e.ActivityField)
                {
                    case ActivityField.ActivityIsMultilingual:
                        x.ActivityIsMultilingual = e.Value.Value;
                        break;
                    case ActivityField.ActivityIsAdaptive:
                        x.ActivityIsAdaptive = e.Value.Value;
                        break;
                    case ActivityField.ActivityIsDispatch:
                        x.ActivityIsDispatch = e.Value.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported field: ${e.ActivityField}");
                }
            });
        }

        public void Handle(CourseActivityFieldGuidModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                switch (e.ActivityField)
                {
                    case ActivityField.SourceIdentifier:
                        x.SourceIdentifier = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported field: ${e.ActivityField}");
                }
            });
        }

        public void Handle(CourseActivityFieldIntModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                switch (e.ActivityField)
                {
                    case ActivityField.ActivityMinutes:
                        x.ActivityMinutes = e.Value;
                        break;
                    case ActivityField.ActivitySequence:
                        x.ActivitySequence = e.Value.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported field: ${e.ActivityField}");
                }
            });
        }

        public void Handle(CourseActivityFieldTextModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                switch (e.ActivityField)
                {
                    case ActivityField.ActivityAuthorName:
                        x.ActivityAuthorName = e.Value;
                        break;
                    case ActivityField.ActivityCode:
                        x.ActivityCode = e.Value;
                        break;
                    case ActivityField.ActivityHook:
                        x.ActivityHook = e.Value;
                        break;
                    case ActivityField.ActivityImage:
                        x.ActivityImage = e.Value;
                        break;
                    case ActivityField.ActivityMode:
                        x.ActivityMode = e.Value;
                        break;
                    case ActivityField.ActivityName:
                        x.ActivityName = e.Value;
                        break;
                    case ActivityField.ActivityPlatform:
                        x.ActivityPlatform = e.Value;
                        break;
                    case ActivityField.PrerequisiteDeterminer:
                        x.PrerequisiteDeterminer = e.Value;
                        break;
                    case ActivityField.RequirementCondition:
                        x.RequirementCondition = e.Value;
                        break;
                    case ActivityField.DoneButtonText:
                        x.DoneButtonText = e.Value;
                        break;
                    case ActivityField.DoneButtonInstructions:
                        x.DoneButtonInstructions = e.Value;
                        break;
                    case ActivityField.DoneMarkedInstructions:
                        x.DoneMarkedInstructions = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported field: ${e.ActivityField}");
                }
            });
        }

        public void Handle(CourseActivityFieldDateModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                switch (e.ActivityField)
                {
                    case ActivityField.ActivityAuthorDate:
                        x.ActivityAuthorDate = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported field: ${e.ActivityField}");
                }
            });
        }

        public void Handle(CourseActivityGradeItemConnected e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.GradeItemIdentifier = e.GradeItemId;
            });
        }

        public void Handle(CourseActivityLegacyPrerequisiteConnected e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.PrerequisiteActivityIdentifier = e.PrerequisiteActivityId;
            });
        }

        public void Handle(CourseActivityPrerequisiteAdded e)
        {
            _courseStore.InsertPrerequisite(e, e.Prerequisite, e.ActivityId, PrerequisiteObjectType.Activity);
        }

        public void Handle(CourseActivityPrerequisiteRemoved e)
        {
            _courseStore.RemovePrerequisite(e, e.PrerequisiteId);
        }

        public void Handle(CourseActivityQuizConnected e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.QuizIdentifier = e.QuizId;
            });
        }

        public void Handle(CourseActivityRemoved e)
        {
            _courseStore.RemoveActivity(e);
        }

        public void Handle(CourseActivitySurveyFormConnected e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.SurveyFormIdentifier = e.SurveyFormId;
            });
        }

        public void Handle(CourseActivityTimestampsModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.Created = e.Created;
                x.CreatedBy = e.CreatedBy;
                x.Modified = e.Modified;
                x.ModifiedBy = e.ModifiedBy;
            });
        }

        public void Handle(CourseActivityTypeModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.ActivityType = e.Type.ToString();
            });
        }

        public void Handle(CourseActivityUrlModified e)
        {
            _courseStore.ModifyActivity(e, e.ActivityId, null, x =>
            {
                x.ActivityUrl = e.Url;
                x.ActivityUrlTarget = e.Target;
                x.ActivityUrlType = e.Type;
            });
        }

        public void Handle(CourseModuleAdded e)
        {
            _courseStore.InsertModule(e);
        }

        public void Handle(CourseModuleMoved e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.UnitIdentifier = e.MoveToUnitId;
            });
        }

        public void Handle(CourseModuleSourceModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.SourceIdentifier = e.Source;
            });
        }

        public void Handle(CourseModuleTimestampsModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.Created = e.Created;
                x.CreatedBy = e.CreatedBy;
                x.Modified = e.Modified;
                x.ModifiedBy = e.ModifiedBy;
            });
        }

        public void Handle(CourseModuleAdaptiveModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.ModuleIsAdaptive = e.IsAdaptive;
            });
        }

        public void Handle(CourseModuleCodeModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.ModuleCode = e.ModuleCode;
            });
        }

        public void Handle(CourseModuleContentModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, e.ModuleContent, null);
        }

        public void Handle(CourseModuleImageModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.ModuleImage = e.Image;
            });
        }

        public void Handle(CourseModulePrerequisiteAdded e)
        {
            _courseStore.InsertPrerequisite(e, e.Prerequisite, e.ModuleId, PrerequisiteObjectType.Module);
        }

        public void Handle(CourseModulePrerequisiteDeterminerModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.PrerequisiteDeterminer = e.PrerequisiteDeterminer;
            });
        }

        public void Handle(CourseModulePrerequisiteRemoved e)
        {
            _courseStore.RemovePrerequisite(e, e.PrerequisiteId);
        }

        public void Handle(CourseModuleRemoved e)
        {
            _courseStore.RemoveModule(e);
        }

        public void Handle(CourseModuleRenamed e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.ModuleName = e.ModuleName;
            });
        }

        public void Handle(CourseModuleSequenceModified e)
        {
            _courseStore.ModifyModule(e, e.ModuleId, null, x =>
            {
                x.ModuleSequence = e.ModuleSequence;
            });
        }

        public void Handle(CourseUnitAdded e)
        {
            _courseStore.InsertUnit(e);
        }

        public void Handle(CourseUnitAdaptiveModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.UnitIsAdaptive = e.IsAdaptive;
            });
        }

        public void Handle(CourseUnitCodeModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.UnitCode = e.UnitCode;
            });
        }

        public void Handle(CourseUnitContentModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, e.UnitContent, null);
        }

        public void Handle(CourseUnitPrerequisiteAdded e)
        {
            _courseStore.InsertPrerequisite(e, e.Prerequisite, e.UnitId, PrerequisiteObjectType.Unit);
        }

        public void Handle(CourseUnitPrerequisiteDeterminerModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.PrerequisiteDeterminer = e.PrerequisiteDeterminer;
            });
        }

        public void Handle(CourseUnitPrerequisiteRemoved e)
        {
            _courseStore.RemovePrerequisite(e, e.PrerequisiteId);
        }

        public void Handle(CourseUnitRemoved e)
        {
            _courseStore.RemoveUnit(e);
        }

        public void Handle(CourseUnitRenamed e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.UnitName = e.UnitName;
            });
        }

        public void Handle(CourseUnitSequenceModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.UnitSequence = e.UnitSequence;
            });
        }

        public void Handle(CourseUnitSourceModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.SourceIdentifier = e.Source;
            });
        }

        public void Handle(CourseUnitTimestampsModified e)
        {
            _courseStore.ModifyUnit(e, e.UnitId, null, x =>
            {
                x.Created = e.Created;
                x.CreatedBy = e.CreatedBy;
                x.Modified = e.Modified;
                x.ModifiedBy = e.ModifiedBy;
            });
        }
    }
}
