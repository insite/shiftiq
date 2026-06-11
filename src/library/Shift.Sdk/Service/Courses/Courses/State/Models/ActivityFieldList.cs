using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Courses
{
    internal static class ActivityFieldList
    {
        private static readonly Dictionary<ActivityField, IStateFieldMeta> _fields = new Dictionary<ActivityField, IStateFieldMeta>
        {
            // Guid
            { ActivityField.AssessmentFormIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { ActivityField.GradeItemIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { ActivityField.PrerequisiteActivityIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { ActivityField.SurveyFormIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { ActivityField.QuizIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { ActivityField.CreatedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false, Required = true } },
            { ActivityField.ModifiedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false, Required = true } },
            { ActivityField.SourceIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },

            // string
            { ActivityField.ActivityAuthorName, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityCode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityHook, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityImage, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityMode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true } },
            { ActivityField.ActivityPlatform, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.ActivityUrl, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { ActivityField.ActivityUrlTarget, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { ActivityField.ActivityUrlType, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { ActivityField.PrerequisiteDeterminer, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.RequirementCondition, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.DoneButtonText, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.DoneButtonInstructions, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { ActivityField.DoneMarkedInstructions, new StateFieldMeta { FieldType = StateFieldType.Text } },

            // bool
            { ActivityField.ActivityIsMultilingual, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { ActivityField.ActivityIsAdaptive, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { ActivityField.ActivityIsDispatch, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },

            // int
            { ActivityField.ActivityAsset, new StateFieldMeta { FieldType = StateFieldType.Int, DirectlyModifiable = false, Required = true } },
            { ActivityField.ActivityMinutes, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { ActivityField.ActivitySequence, new StateFieldMeta { FieldType = StateFieldType.Int, Required = true } },

            // DateTimeOffset
            { ActivityField.Created, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { ActivityField.Modified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },

            // DateTime
            { ActivityField.ActivityAuthorDate, new StateFieldMeta { FieldType = StateFieldType.Date } },
        };

        public static IStateFieldMeta GetField(ActivityField activityField) => _fields[activityField];

        public static IReadOnlyDictionary<ActivityField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<ActivityField, IStateFieldMeta>(_fields);
    }
}
