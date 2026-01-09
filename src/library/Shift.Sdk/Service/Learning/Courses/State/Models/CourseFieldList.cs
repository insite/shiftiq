using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Courses
{
    internal static class CourseFieldList
    {
        private static readonly Dictionary<CourseField, IStateFieldMeta> _fields = new Dictionary<CourseField, IStateFieldMeta>
        {
            // Guid
            { CourseField.FrameworkStandardIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.GradebookIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.CatalogIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.CompletionActivityIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.CompletedToAdministratorMessageIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.CompletedToLearnerMessageIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.StalledToAdministratorMessageIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.StalledToLearnerMessageIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false } },
            { CourseField.CreatedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false, Required = true } },
            { CourseField.ModifiedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, DirectlyModifiable = false, Required = true } },
            { CourseField.SourceIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },

            // string
            { CourseField.CourseCode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseDescription, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseHook, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseIcon, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseImage, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseLabel, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseLevel, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true } },
            { CourseField.CoursePlatform, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseProgram, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseSlug, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseFlagColor, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseFlagText, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseStyle, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { CourseField.CourseUnit, new StateFieldMeta { FieldType = StateFieldType.Text } },

            // bool
            { CourseField.AllowDiscussion, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { CourseField.CourseIsHidden, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { CourseField.IsMultipleUnitsEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { CourseField.IsProgressReportEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },

            // int
            { CourseField.CourseAsset, new StateFieldMeta { FieldType = StateFieldType.Int, DirectlyModifiable = false, Required = true } },
            { CourseField.CourseSequence, new StateFieldMeta { FieldType = StateFieldType.Int, Required = true } },
            { CourseField.OutlineWidth, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { CourseField.SendMessageStalledAfterDays, new StateFieldMeta { FieldType = StateFieldType.Int, DirectlyModifiable = false } },
            { CourseField.SendMessageStalledMaxCount, new StateFieldMeta { FieldType = StateFieldType.Int, DirectlyModifiable = false } },

            // DateTimeOffset
            { CourseField.Created, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false, Required = true } },
            { CourseField.Modified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false, Required = true } },
            { CourseField.Closed, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = true, Required = false } },
        };

        public static IStateFieldMeta GetField(CourseField courseField) => _fields[courseField];

        public static IReadOnlyDictionary<CourseField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<CourseField, IStateFieldMeta>(_fields);
    }
}
