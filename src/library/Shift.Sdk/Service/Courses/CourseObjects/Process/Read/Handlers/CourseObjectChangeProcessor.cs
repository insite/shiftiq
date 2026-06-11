using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Domain.CourseObjects;

namespace InSite.Application.Courses.Read
{
    /// <summary>
    /// Implements the process manager for Course events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class CourseObjectChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IProgressRestarter _restarter;

        public CourseObjectChangeProcessor(ICommander commander, IChangeQueue publisher, IProgressRestarter restarter)
        {
            _commander = commander;
            _restarter = restarter;

            publisher.Subscribe<CourseEnrollmentRestarted>(Handle);
        }

        public void Handle(CourseEnrollmentRestarted @event)
        {
            _restarter.Restart(@event.Learner, @event.Course, DateTimeOffset.Now);
        }

        public static void EnsureEnrollment(Action<ICommand> send, IRecordSearch records, Guid gradebook, Guid learner, DateTimeOffset? time)
        {
            if (records.EnrollmentExists(gradebook, learner))
                return;

            var create = records.CreateCommandToAddEnrollment(null, gradebook, learner, null, time, null);

            send(create);
        }
    }
}