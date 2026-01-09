using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class StandardCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IStandardSearch _standardSearch;
        private bool _isDbValidationEnabled;

        public StandardCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IStandardSearch standardSearch
            )
        {
            _publisher = publisher;
            _repository = repository;
            _standardSearch = standardSearch;
            _isDbValidationEnabled = true;

            commander.Subscribe<CreateStandard>(Handle);
            commander.Subscribe<RemoveStandard>(Handle);
            commander.Subscribe<ModifyStandardTimestamps>(Handle);
            commander.Subscribe<AddStandardCategory>(Handle);
            commander.Subscribe<RemoveStandardCategory>(Handle);
            commander.Subscribe<AddStandardConnection>(Handle);
            commander.Subscribe<RemoveStandardConnection>(Handle);
            commander.Subscribe<AddStandardContainment>(Handle);
            commander.Subscribe<RemoveStandardContainment>(Handle);
            commander.Subscribe<AddStandardOrganization>(Handle);
            commander.Subscribe<RemoveStandardOrganization>(Handle);
            commander.Subscribe<AddStandardAchievement>(Handle);
            commander.Subscribe<RemoveStandardAchievement>(Handle);
            commander.Subscribe<AddStandardGroup>(Handle);
            commander.Subscribe<RemoveStandardGroup>(Handle);
            commander.Subscribe<ModifyStandardContainment>(Handle);
            commander.Subscribe<ModifyStandardContent>(Handle);
            commander.Subscribe<ModifyStandardFieldText>(Handle);
            commander.Subscribe<ModifyStandardFieldDateOffset>(Handle);
            commander.Subscribe<ModifyStandardFieldBool>(Handle);
            commander.Subscribe<ModifyStandardFieldInt>(Handle);
            commander.Subscribe<ModifyStandardFieldDecimal>(Handle);
            commander.Subscribe<ModifyStandardFieldGuid>(Handle);
            commander.Subscribe<ModifyStandardFields>(Handle);
        }

        private void Commit(StandardAggregate aggregate)
        {
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        private void HandleCommand(Command c, Action<StandardAggregate> action)
        {
            _repository.LockAndRun<StandardAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LockAndRun(c, () =>
                {
                    action(aggregate);
                });
                Commit(aggregate);
            });
        }

        /// <summary>
        /// Used for bulk inserts in InSite.Maintenance
        /// </summary>
        public void DisableDbValidation()
        {
            _isDbValidationEnabled = false;
        }

        /// <summary>
        /// Used for bulk inserts in InSite.Maintenance
        /// </summary>
        public void EnableDbValidation()
        {
            _isDbValidationEnabled = true;
        }

        public void Handle(CreateStandard c)
        {
            if (_isDbValidationEnabled)
            {
                if (_repository.Exists<StandardAggregate>(c.AggregateIdentifier))
                    throw new AggregateException($"StandardIdentifier is already used: {c.AggregateIdentifier}");

                if (_standardSearch.Exists(c.OriginOrganization, c.AssetNumber))
                    throw new AggregateException($"AssetNumber is already assigned to another standard in this organization: {c.AssetNumber}");
            }

            var aggregate = new StandardAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.LockAndRun(c, () =>
            {
                aggregate.CreateStandard(c.StandardType, c.AssetNumber, c.Sequence, c.Content);
            });
            Commit(aggregate);
        }

        public void Handle(RemoveStandard c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandard();
            });
        }

        public void Handle(ModifyStandardTimestamps c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardTimestamps(c.Created, c.CreatedBy, c.Modified, c.ModifiedBy);
            });
        }

        public void Handle(AddStandardCategory c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardCategory(c.Categories);
            });
        }

        public void Handle(RemoveStandardCategory c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardCategory(c.CategoryIds);
            });
        }

        public void Handle(AddStandardConnection c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardConnection(c.Connections);
            });
        }

        public void Handle(RemoveStandardConnection c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardConnection(c.ToStandardIds);
            });
        }

        public void Handle(AddStandardContainment c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardContainment(c.Containments);
            });
        }

        public void Handle(RemoveStandardContainment c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardContainment(c.ChildStandardIds);
            });
        }

        public void Handle(AddStandardOrganization c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardOrganization(c.OrganizationIds);
            });
        }

        public void Handle(RemoveStandardOrganization c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardOrganization(c.OrganizationIds);
            });
        }

        public void Handle(AddStandardAchievement c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardAchievement(c.AchievementIds);
            });
        }

        public void Handle(RemoveStandardAchievement c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardAchievement(c.AchievementIds);
            });
        }

        public void Handle(AddStandardGroup c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.AddStandardGroup(c.Groups);
            });
        }

        public void Handle(RemoveStandardGroup c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.RemoveStandardGroup(c.GroupIds);
            });
        }

        public void Handle(ModifyStandardContainment c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardContainment(c.Containments);
            });
        }

        public void Handle(ModifyStandardContent c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardContent(c.Content);
            });
        }

        public void Handle(ModifyStandardFieldText c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldText(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFieldDateOffset c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldDateOffset(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFieldBool c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldBool(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFieldInt c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldInt(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFieldDecimal c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldDecimal(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFieldGuid c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFieldGuid(c.Field, c.Value);
            });
        }

        public void Handle(ModifyStandardFields c)
        {
            HandleCommand(c, aggregate =>
            {
                aggregate.ModifyStandardFields(c.Fields);
            });
        }
    }
}
