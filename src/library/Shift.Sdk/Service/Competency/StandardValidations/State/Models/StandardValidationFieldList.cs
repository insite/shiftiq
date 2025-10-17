using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Standards
{
    public static class StandardValidationFieldList
    {
        private static readonly Dictionary<StandardValidationField, IStateFieldMeta> _fields = new Dictionary<StandardValidationField, IStateFieldMeta>
        {
            { StandardValidationField.StandardIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true } },
            { StandardValidationField.UserIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true } },
            { StandardValidationField.ValidatorUserIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = false } },
            { StandardValidationField.OrganizationIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardValidationField.Created, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = true, DirectlyModifiable = false } },
            { StandardValidationField.CreatedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardValidationField.Expired, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardValidationField.IsValidated, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { StandardValidationField.Modified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = true, DirectlyModifiable = false } },
            { StandardValidationField.ModifiedBy, new StateFieldMeta { FieldType = StateFieldType.Guid, Required = true, DirectlyModifiable = false } },
            { StandardValidationField.Notified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardValidationField.SelfAssessmentDate, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardValidationField.SelfAssessmentStatus, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardValidationField.ValidationComment, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } },
            { StandardValidationField.ValidationDate, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = false } },
            { StandardValidationField.ValidationStatus, new StateFieldMeta { FieldType = StateFieldType.Text, Required = false } }
        };

        public static IStateFieldMeta GetField(StandardValidationField field) => _fields[field];

        public static IReadOnlyDictionary<StandardValidationField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<StandardValidationField, IStateFieldMeta>(_fields);

        public static void Validate<T>(StandardValidationField field, T value, bool directlyModified)
        {
            Validate(GetField(field), field, value, directlyModified);
        }

        public static void Validate<T>(IStateFieldMeta info, StandardValidationField field, T value, bool directlyModified)
        {
            if (info.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid user field: {field}");

            Validate(info, field, value == null, directlyModified);
        }

        public static void Validate(StandardValidationField field, object value, bool directlyModified)
        {
            Validate(GetField(field), field, value, directlyModified);
        }

        public static void Validate(IStateFieldMeta info, StandardValidationField field, object value, bool directlyModified)
        {
            if (value != null && info.FieldType != StateFieldHelper.GetFieldType(value.GetType()))
                throw new ArgumentException($"Invalid user field: {field}");

            Validate(info, field, value == null, directlyModified);
        }

        private static void Validate(IStateFieldMeta info, StandardValidationField field, bool isNull, bool directlyModified)
        {
            if (directlyModified && !info.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {field}");

            if (info.Required && isNull)
                throw new ArgumentNullException($"Field {field} is a required field");
        }
    }
}
