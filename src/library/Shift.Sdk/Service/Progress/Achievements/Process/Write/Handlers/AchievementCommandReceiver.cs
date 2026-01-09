using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Achievements.Write;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class AchievementCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public AchievementCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<CreateAchievement>(Handle);
            commander.Subscribe<DescribeAchievement>(Handle);
            commander.Subscribe<ChangeAchievementExpiry>(Handle);
            commander.Subscribe<ChangeAchievementOrganization>(Handle);
            commander.Subscribe<ChangeAchievementType>(Handle);
            commander.Subscribe<ChangeCertificateLayout>(Handle);
            commander.Subscribe<UnlockAchievement>(Handle);
            commander.Subscribe<LockAchievement>(Handle);
            commander.Subscribe<DeleteAchievement>(Handle);
            commander.Subscribe<ChangeAchievementBadgeImageUrl>(Handle);
            commander.Subscribe<EnableAchievementBadgeImage>(Handle);
            commander.Subscribe<DisableAchievementBadgeImage>(Handle);
            commander.Subscribe<EnableAchievementReporting>(Handle);
            commander.Subscribe<DisableAchievementReporting>(Handle);
            commander.Subscribe<AddAchievementPrerequisite>(Handle);
            commander.Subscribe<DeleteAchievementPrerequisite>(Handle);
        }

        private void Commit(AchievementAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(CreateAchievement c)
        {
            if (string.IsNullOrWhiteSpace(c.Title))
                throw new AchievementException("The new achievement has empty title.");

            var aggregate = new AchievementAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.CreateAchievement(c.Tenant, c.Label, c.Title, c.Description, c.Expiration, c.Source);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAchievementBadgeImageUrl c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.ChangeAchievementBadgeImageUrl(c.BadgeImageUrl);
            Commit(aggregate, c);
        }

        public void Handle(EnableAchievementBadgeImage c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.EnableAchievementCustomBadgeImage();
            Commit(aggregate, c);
        }

        public void Handle(DisableAchievementBadgeImage c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.DisableAchievementCustomBadgeImage();
            Commit(aggregate, c);
        }

        public void Handle(DescribeAchievement c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.DescribeAchievement(c.Label, c.Title, c.Description, c.AllowSelfDeclared);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAchievementExpiry c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.ChangeAchievementExpiry(c.Expiration);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAchievementOrganization c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.ChangeAchievementOrganization(c.Organization);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAchievementType c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);

            if (aggregate.Data.Type != c.Type)
            {
                aggregate.ChangeAchievementType(c.Type);
                Commit(aggregate, c);
            }
        }

        public void Handle(ChangeCertificateLayout c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.ChangeCertificateLayout(c.Code);
            Commit(aggregate, c);
        }

        public void Handle(UnlockAchievement c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.UnlockAchievement();
            Commit(aggregate, c);
        }

        public void Handle(LockAchievement c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.LockAchievement();
            Commit(aggregate, c);
        }

        public void Handle(DeleteAchievement c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.DeleteAchievement(c.Cascade);
            Commit(aggregate, c);
        }

        public void Handle(EnableAchievementReporting c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.EnableAchievementReporting();
            Commit(aggregate, c);
        }

        public void Handle(DisableAchievementReporting c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.DisableAchievementReporting();
            Commit(aggregate, c);
        }

        public void Handle(AddAchievementPrerequisite c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.AddAchievementPrerequisite(c.Prerequisite, c.Conditions);
            Commit(aggregate, c);
        }

        public void Handle(DeleteAchievementPrerequisite c)
        {
            var aggregate = _repository.Get<AchievementAggregate>(c.AggregateIdentifier);
            aggregate.DeleteAchievementPrerequisite(c.Prerequisite);
            Commit(aggregate, c);
        }
    }
}
