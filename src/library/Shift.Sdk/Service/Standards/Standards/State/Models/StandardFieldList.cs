using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Standards
{
    internal static class StandardFieldList
    {
        private static readonly Dictionary<StandardField, IStateFieldMeta> _fields = new Dictionary<StandardField, IStateFieldMeta>
        {
            { StandardField.AssetNumber, new StateFieldMeta { FieldType = StateFieldType.Int, Required = true, DirectlyModifiable = false } },
            { StandardField.AuthorDate, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardField.AuthorName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false} },
            { StandardField.BankIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false} },
            { StandardField.BankSetIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false} },
            { StandardField.CalculationArgument, new StateFieldMeta { FieldType = StateFieldType.Int, Required = false } },
            { StandardField.CanvasIdentifier, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.CategoryItemIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardField.CertificationHoursPercentCore, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false } },
            { StandardField.CertificationHoursPercentNonCore, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false  } },
            { StandardField.Code, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.CompetencyScoreCalculationMethod, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.CompetencyScoreSummarizationMethod, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.ContentName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.ContentSettings, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.Created, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = true, DirectlyModifiable = false } },
            { StandardField.CreatedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardField.CreditHours, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false } },
            { StandardField.CreditIdentifier, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.DatePosted, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardField.DepartmentGroupIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardField.DocumentType, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.Icon, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.IndustryItemIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardField.IsCertificateEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsFeedbackEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsHidden, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsLocked, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsPractical, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsPublished, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsTemplate, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.IsTheory, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardField.Language, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.LevelCode, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.LevelType, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.MajorVersion, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.MasteryPoints, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false } },
            { StandardField.MinorVersion, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.Modified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = true, DirectlyModifiable = false } },
            { StandardField.ModifiedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardField.OrganizationIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardField.ParentStandardIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardField.PassingScore, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false } },
            { StandardField.PointsPossible, new StateFieldMeta { FieldType = StateFieldType.Decimal, Required = false } },
            { StandardField.Sequence, new StateFieldMeta { FieldType = StateFieldType.Int, Required = true } },
            { StandardField.SourceDescriptor, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardAlias, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardLabel, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardPrivacyScope, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardStatus, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardStatusLastUpdateTime, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardField.StandardStatusLastUpdateUser, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardField.StandardTier, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.StandardType, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true } },
            { StandardField.Tags, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardField.UtcPublished, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardField.StandardHook, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
        };

        public static IStateFieldMeta GetField(StandardField field) => _fields[field];

        public static IReadOnlyDictionary<StandardField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<StandardField, IStateFieldMeta>(_fields);

        public static void Validate<T>(StandardField field, T value, bool directlyModified)
        {
            var info = GetField(field);
            if (info.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid user field: {field}");

            Validate(info, field, value, directlyModified);
        }

        public static void Validate(StandardField field, object value, bool directlyModified)
        {
            var info = GetField(field);
            if (value != null && info.FieldType != StateFieldHelper.GetFieldType(value.GetType()))
                throw new ArgumentException($"Invalid user field: {field}");

            Validate(info, field, value, directlyModified);
        }

        private static void Validate(IStateFieldMeta fieldInfo, StandardField field, object value, bool directlyModified)
        {
            if (directlyModified && !fieldInfo.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {field}");

            if (fieldInfo.Required && value == null)
                throw new ArgumentNullException($"Field {field} is a required field");
        }
    }
}
