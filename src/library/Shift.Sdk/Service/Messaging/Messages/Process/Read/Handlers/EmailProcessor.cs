using Shift.Common.Timeline.Changes;

using InSite.Domain.Messages;

namespace InSite.Application.Messages.Read
{
    /// <summary>
    /// Implements the process manager for Email events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class EmailProcessor
    {
        private readonly IEmailStore _store;
        private readonly IEmailOutbox _outbox;

        public EmailProcessor(IChangeQueue publisher, IEmailStore store, IEmailOutbox outbox)
        {
            publisher.Subscribe<EmailScheduled>(Handle);

            _store = store;
            _outbox = outbox;
        }

        public void Handle(EmailScheduled e)
        {
            _outbox.Send(e.Email);
            _store.Insert(e.Email);
        }
    }
}