using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Application.StandardValidations.Write;
using InSite.Domain.Standards;

using Shift.Common;

namespace InSite.Application.Standards.Write
{
    public class StandardValidationCommandCreator
    {
        private readonly QStandardValidation _entity;

        private List<Command> _result = new List<Command>();

        private StandardValidationCommandCreator(QStandardValidation entity)
        {
            _entity = entity;
        }

        public static List<Command> Insert(QStandardValidation entity)
        {
            var creator = new StandardValidationCommandCreator(entity);
            creator.AddCreateCommand();
            creator.AddModifyCommand();
            return creator._result;
        }

        public static List<Command> Update(QStandardValidation entity)
        {
            var creator = new StandardValidationCommandCreator(entity);
            creator.AddModifyCommand();
            return creator._result;
        }

        public static List<Command> Remove(QStandardValidation entity)
        {
            var creator = new StandardValidationCommandCreator(entity);
            creator.AddRemoveCommand();
            return creator._result;
        }

        private void AddCreateCommand()
        {
            _result.Add(new CreateStandardValidation(_entity.StandardValidationIdentifier, _entity.StandardValidationIdentifier, _entity.UserIdentifier));
        }

        private void AddModifyCommand()
        {
            var fieldsCommand = new ModifyStandardValidationFields(_entity.StandardValidationIdentifier);

            foreach (var kv in FieldMapping)
                fieldsCommand.Fields[kv.Key] = kv.Value(_entity);

            if (fieldsCommand.Fields.IsNotEmpty())
                _result.Add(fieldsCommand);
        }

        private void AddRemoveCommand()
        {
            _result.Add(new RemoveStandardValidation(_entity.StandardValidationIdentifier));
        }

        #region Helpers

        private static readonly IReadOnlyDictionary<StandardValidationField, Func<QStandardValidation, object>> FieldMapping;

        static StandardValidationCommandCreator()
        {
            var mapping = new Dictionary<StandardValidationField, Func<QStandardValidation, object>>
            {
                // Guid
                { StandardValidationField.StandardIdentifier, e => e.StandardIdentifier },
                { StandardValidationField.UserIdentifier, e => e.UserIdentifier },
                { StandardValidationField.ValidatorUserIdentifier, e => e.ValidatorUserIdentifier },

                // DateTimeOffset
                { StandardValidationField.Expired, e => e.Expired },
                { StandardValidationField.Notified, e => e.Notified },
                { StandardValidationField.SelfAssessmentDate, e => e.SelfAssessmentDate },
                { StandardValidationField.ValidationDate, e => e.ValidationDate },

                // string
                { StandardValidationField.SelfAssessmentStatus, e => e.SelfAssessmentStatus },
                { StandardValidationField.ValidationComment, e => e.ValidationComment },
                { StandardValidationField.ValidationStatus, e => e.ValidationStatus },

                // bool
                { StandardValidationField.IsValidated, e => e.IsValidated },
            };

            FieldMapping = new ReadOnlyDictionary<StandardValidationField, Func<QStandardValidation, object>>(mapping);
        }

        #endregion
    }
}
