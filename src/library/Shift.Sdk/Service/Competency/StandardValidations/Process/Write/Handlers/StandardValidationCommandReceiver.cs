using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Application.StandardValidations.Write;
using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class StandardValidationCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IStandardValidationSearch _validationSearch;

        public StandardValidationCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IStandardValidationSearch validationSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _validationSearch = validationSearch;

            commander.Subscribe<CreateStandardValidation>(Handle);
            commander.Subscribe<RemoveStandardValidation>(Handle);
            commander.Subscribe<RemoveStandardValidationLog>(Handle);
            commander.Subscribe<ModifyStandardValidationTimestamps>(Handle);
            commander.Subscribe<ModifyStandardValidationFieldText>(Handle);
            commander.Subscribe<ModifyStandardValidationFieldDateOffset>(Handle);
            commander.Subscribe<ModifyStandardValidationFieldBool>(Handle);
            commander.Subscribe<ModifyStandardValidationFieldGuid>(Handle);
            commander.Subscribe<ModifyStandardValidationFields>(Handle);
            commander.Subscribe<SelfValidateStandardValidation>(Handle);
            commander.Subscribe<SubmitForValidationStandardValidation>(Handle);
            commander.Subscribe<ValidateStandardValidation>(Handle);
            commander.Subscribe<ExpireStandardValidation>(Handle);
            commander.Subscribe<NotifyStandardValidation>(Handle);
            commander.Subscribe<AddStandardValidationLog>(Handle);
            commander.Subscribe<ModifyStandardValidationLog>(Handle);
            commander.Subscribe<ModifyStandardValidationStatus>(Handle);
        }

        private void Commit(StandardValidationAggregate aggregate)
        {
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        private void HandleCommand(Command c, Action<StandardValidationAggregate> action)
        {
            _repository.LockAndRun<StandardValidationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LockAndRun(c, () =>
                {
                    action(aggregate);
                });
                Commit(aggregate);
            });
        }

        public void Handle(CreateStandardValidation c)
        {
            if (_repository.Exists<StandardValidationAggregate>(c.AggregateIdentifier))
                throw CreateDuplicateException(c.AggregateIdentifier);

            if (_validationSearch.Exists(c.StandardId, c.UserId))
                throw CreateDuplicateException(c.StandardId, c.UserId);

            var aggregate = new StandardValidationAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.LockAndRun(c, () =>
            {
                aggregate.CreateStandardValidation(c.StandardId, c.UserId);
            });
            Commit(aggregate);
        }

        public void Handle(RemoveStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardValidation();
            });
        }

        public void Handle(RemoveStandardValidationLog c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardValidationLog(c.LogId);
            });
        }

        public void Handle(ModifyStandardValidationTimestamps c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationTimestamps(c.Created, c.CreatedBy, c.Modified, c.ModifiedBy);
            });
        }

        public void Handle(ModifyStandardValidationFieldText c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationFieldText(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardValidationFieldDateOffset c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationFieldDateOffset(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardValidationFieldBool c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationFieldBool(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardValidationFieldGuid c)
        {
            HandleCommand(c, aggregate =>
            {
                Guid? standardId = null;
                Guid? userId = null;

                if (c.Field == StandardValidationField.UserIdentifier)
                {
                    standardId = aggregate.Data.GetGuidValue(StandardValidationField.StandardIdentifier);
                    userId = c.Value;
                }
                else if (c.Field == StandardValidationField.StandardIdentifier)
                {
                    standardId = c.Value;
                    userId = aggregate.Data.GetGuidValue(StandardValidationField.UserIdentifier);
                }

                if (standardId.HasValue && userId.HasValue && _validationSearch.Exists(standardId.Value, userId.Value, c.AggregateIdentifier))
                    throw CreateDuplicateException(standardId.Value, userId.Value);

                aggregate.ModifyStandardValidationFieldGuid(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardValidationFields c)
        {
            HandleCommand(c, aggregate =>
            {
                var hasStandardId = c.Fields.ContainsKey(StandardValidationField.StandardIdentifier);
                var hasUserId = c.Fields.ContainsKey(StandardValidationField.UserIdentifier);

                if (hasStandardId || hasUserId)
                {
                    var standardId = hasStandardId
                        ? (Guid?)c.Fields[StandardValidationField.StandardIdentifier]
                        : aggregate.Data.GetGuidValue(StandardValidationField.StandardIdentifier);
                    var userId = hasUserId
                        ? (Guid?)c.Fields[StandardValidationField.UserIdentifier]
                        : aggregate.Data.GetGuidValue(StandardValidationField.UserIdentifier);

                    if (standardId.HasValue && userId.HasValue && _validationSearch.Exists(standardId.Value, userId.Value, c.AggregateIdentifier))
                        throw CreateDuplicateException(standardId.Value, userId.Value);
                }

                aggregate.ModifyStandardValidationFields(c.Fields);
            });
        }

        public void Handle(SelfValidateStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.SelfValidateStandardValidation(c.LogId, c.Status);
            });
        }

        public void Handle(SubmitForValidationStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.SubmitForValidationStandardValidation(c.LogId);
            });
        }

        public void Handle(ValidateStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ValidateStandardValidation(c.LogId, c.IsValidated, c.Status, c.Comment);
            });
        }

        public void Handle(ExpireStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ExpireStandardValidation(c.LogId, c.Comment);
            });
        }

        public void Handle(NotifyStandardValidation c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.NotifyStandardValidation(c.Date);
            });
        }

        public void Handle(AddStandardValidationLog c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardValidationLog(c.Logs);
            });
        }

        public void Handle(ModifyStandardValidationLog c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationLog(c.Log);
            });
        }

        public void Handle(ModifyStandardValidationStatus c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardValidationStatus(c.LogId, c.IsValidated, c.SelfAssessmentStatus, c.ValidationStatus, c.ValidationComment);
            });
        }

        #region Helpers

        private static AggregateException CreateDuplicateException(Guid aggregateId)
        {
            return new AggregateException($"StandardValidationIdentifier is already used: {aggregateId}");
        }

        private static AggregateException CreateDuplicateException(Guid standardId, Guid userId)
        {
            return new AggregateException($"Validation record for (StandardId={standardId}, UserId={userId}) already exists");
        }

        #endregion
    }
}
