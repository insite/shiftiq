using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Persistence.Plugin.RCABC
{
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows.
    /// From an implementation perspective, a process manager is a state machine that is driven forward by incoming 
    /// events (which may come from many aggregates). Some states will have side effects, such as sending commands, 
    /// talking to external web services, or sending emails.
    /// </remarks>
    public class RcabcGradebookChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IChangeRepository _changes;
        private readonly IRegistrationSearch _registrations;
        private readonly IRecordSearch _records;

        public RcabcGradebookChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IChangeRepository changes,
            IRegistrationSearch registrations,
            IRecordSearch records
            )
        {
            _commander = commander;
            _changes = changes;
            _registrations = registrations;
            _records = records;

            var rcabc = Shift.Constant.OrganizationIdentifiers.RCABC;

            publisher.Extend<GradebookCalculated>(Handle, rcabc);
        }

        public void Handle(GradebookCalculated e)
        {
            var record = _changes.GetClone<GradebookAggregate>(e.AggregateIdentifier).Data;

            if (record.PrimaryEvent == null || record.RootItems.Count == 0)
                return;

            var registrations = _registrations.GetRegistrationsByEvent(record.PrimaryEvent.Value);

            var itemKey = record.RootItems[0].Identifier;
            var progresses = _records.GetGradebookScores(new QProgressFilter { GradebookIdentifier = e.AggregateIdentifier, GradeItemIdentifier = itemKey });

            foreach (var student in record.Enrollments)
            {
                var registration = registrations.Find(x => x.CandidateIdentifier == student.Learner);
                if (registration != null && registration.AttendanceStatus.HasValue())
                {
                    var progress = progresses.Find(x => x.UserIdentifier == student.Learner);
                    if (progress != null)
                    {
                        if (progress.ProgressPercent.HasValue)
                            Send(e, new ChangeProgressPercent(progress.ProgressIdentifier, null, null));

                        if (progress.ProgressNumber.HasValue)
                            Send(e, new ChangeProgressNumber(progress.ProgressIdentifier, null, null));

                        Send(e, new ChangeProgressText(progress.ProgressIdentifier, registration.AttendanceStatus, progress.ProgressGraded));
                    }
                }
            }
        }

        private void Send(IChange cause, ICommand effect)
        {
            Identify(cause, effect);
            _commander.Send(effect);
        }

        private void Identify(IChange cause, ICommand effect)
        {
            effect.OriginOrganization = cause.OriginOrganization;
            effect.OriginUser = cause.OriginUser;
        }
    }
}
