using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Responses.Write;
using InSite.Domain.Surveys.Sessions;

namespace InSite.Application.Surveys.Write.Handlers
{
    public class ResponseCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public ResponseCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddResponseOptions>(Handle);
            commander.Subscribe<AddResponseAnswer>(Handle);
            commander.Subscribe<ChangeResponseAnswer>(Handle);
            commander.Subscribe<ChangeResponseGroup>(Handle);
            commander.Subscribe<ChangeResponsePeriod>(Handle);
            commander.Subscribe<ChangeResponseUser>(Handle);
            commander.Subscribe<CompleteResponseSession>(Handle);
            commander.Subscribe<ConfirmResponseSession>(Handle);
            commander.Subscribe<CreateResponseSession>(Handle);
            commander.Subscribe<LockResponseSession>(Handle);
            commander.Subscribe<ReviewResponseSession>(Handle);
            commander.Subscribe<SelectResponseOption>(Handle);
            commander.Subscribe<StartResponseSession>(Handle);
            commander.Subscribe<UnlockResponseSession>(Handle);
            commander.Subscribe<UnselectResponseOption>(Handle);
            commander.Subscribe<DeleteResponseSession>(Handle);
            commander.Subscribe<TermsConsentResponseSession>(Handle);
        }

        private void Commit(ResponseAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(AddResponseOptions c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddResponseOptions(c.Question, c.Items);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddResponseAnswer c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddResponseAnswer(c.Question);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeResponseAnswer c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeResponseAnswer(c.Question, c.Answer);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeResponseGroup c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeResponseGroup(c.Group);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeResponsePeriod c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeResponsePeriod(c.Period);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeResponseUser c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeResponseUser(c.User);
                Commit(aggregate, c);
            });
        }

        public void Handle(CompleteResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CompleteResponse(c.Completed);
                Commit(aggregate, c);
            });
        }

        public void Handle(ConfirmResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ConfirmResponse();
                Commit(aggregate, c);
            });
        }

        public void Handle(CreateResponseSession c)
        {
            var aggregate = new ResponseAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Form };
            aggregate.CreateResponse(c.Source, c.Tenant, c.Form, c.User);
            Commit(aggregate, c);
        }

        public void Handle(LockResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LockResponse();
                Commit(aggregate, c);
            });
        }

        public void Handle(TermsConsentResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TermsConsent(c.AggregateIdentifier, c.OrganizationIdentifier, c.QuestionIdentifier, c.UserIdentifier);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReviewResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReviewResponse();
                Commit(aggregate, c);
            });
        }

        public void Handle(SelectResponseOption c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.SelectResponseOption(c.Item);
                Commit(aggregate, c);
            });
        }

        public void Handle(StartResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.StartResponse(c.Started, c.NoStatusChange);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnlockResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnlockResponse();
                Commit(aggregate, c);
            });
        }

        public void Handle(UnselectResponseOption c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnselectResponseOption(c.Item);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteResponseSession c)
        {
            _repository.LockAndRun<ResponseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteResponse();
                Commit(aggregate, c);
            });
        }
    }
}