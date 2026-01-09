using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Application.Rubrics.Write;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class RubricCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IRubricStore _rubricStore;

        public RubricCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IRubricStore rubricStore
            )
        {
            _publisher = publisher;
            _repository = repository;
            _rubricStore = rubricStore;

            commander.Subscribe<RunCommands>(Handle);

            commander.Subscribe<CreateRubric>(Handle);
            commander.Subscribe<DeleteRubric>(Handle);
            commander.Subscribe<RenameRubric>(Handle);
            commander.Subscribe<DescribeRubric>(Handle);
            commander.Subscribe<TranslateRubric>(Handle);
            commander.Subscribe<ModifyRubricTimestamp>(Handle);

            commander.Subscribe<AddRubricCriterion>(Handle);
            commander.Subscribe<RemoveRubricCriterion>(Handle);
            commander.Subscribe<RenameRubricCriterion>(Handle);
            commander.Subscribe<DescribeRubricCriterion>(Handle);
            commander.Subscribe<TranslateRubricCriterion>(Handle);
            commander.Subscribe<ModifyRubricCriterionIsRange>(Handle);

            commander.Subscribe<AddRubricCriterionRating>(Handle);
            commander.Subscribe<RemoveRubricCriterionRating>(Handle);
            commander.Subscribe<RenameRubricCriterionRating>(Handle);
            commander.Subscribe<DescribeRubricCriterionRating>(Handle);
            commander.Subscribe<TranslateRubricCriterionRating>(Handle);
            commander.Subscribe<ModifyRubricCriterionRatingPoints>(Handle);
        }

        private void Handle(RunCommands runCommands)
        {
            if (runCommands.Commands == null || runCommands.Commands.Length == 0)
                return;

            if (runCommands.Commands[0] is IHasAggregate create)
            {
                if (((IHasRun)create).Run(null))
                    RunCommands(create.Aggregate, runCommands, 1);
            }
            else
            {
                _repository.LockAndRun<RubricAggregate>(runCommands.AggregateIdentifier, rubric =>
                {
                    RunCommands(rubric, runCommands, 0);
                });
            }
        }

        private bool RunCommands(RubricAggregate aggregate, RunCommands runCommands, int startIndex)
        {
            for (int i = startIndex; i < runCommands.Commands.Length; i++)
            {
                var command = runCommands.Commands[i];

                if (command.AggregateIdentifier != aggregate.AggregateIdentifier)
                    throw new ArgumentException($"The command has wrong AggregateIdentifier: {command.AggregateIdentifier}");

                if (!((IHasRun)command).Run(aggregate))
                    return false;
            }

            var transactionId = _rubricStore.StartTransaction(aggregate.AggregateIdentifier);
            try
            {
                Commit(aggregate, runCommands, transactionId);
                _rubricStore.CommitTransaction(transactionId);
                return true;
            }
            catch
            {
                _rubricStore.CancelTransaction(transactionId);
                throw;
            }
        }

        private void Handle<T>(T c) where T : Command, IHasRun
        {
            if (c is IHasAggregate create)
            {
                if (c.Run(null))
                    Commit(create.Aggregate, c, null);

                return;
            }

            _repository.LockAndRun<RubricAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.Run(aggregate))
                    Commit(aggregate, c, null);
            });
        }

        private void Commit(RubricAggregate aggregate, ICommand c, Guid? changeTransactionId)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (Change change in changes)
            {
                change.AggregateState = aggregate.State;
                change.ChangeTransactionId = changeTransactionId;
                _publisher.Publish(change);
            }
        }
    }
}
