using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Events.Read;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Registrations;

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
    public class RcabcRegistrationChangeProcessor
    {
        private static class Groups
        {
            public static readonly Guid REFApprentice = Guid.Parse("40d66a7f-8cf0-463f-bdfe-acab002b2fc1");
            public static readonly Guid REFStudent = Guid.Parse("46061ade-88b2-48e9-8f44-acab002b2fc1");
            public static readonly Guid MasterRoofer = Guid.Parse("80A8F12D-3605-4670-ADA3-12E8C622AB07");
        }

        private static readonly Dictionary<Guid, Guid> _achievementToGroup = new Dictionary<Guid, Guid>
        {
            { Guid.Parse("b8469342-7d01-4bf0-a6eb-acab0035a59b"), Groups.REFApprentice }, // ASMW Level 1
            { Guid.Parse("54b2f9a6-22d3-49bd-b981-acab0035a59b"), Groups.REFApprentice }, // ASMW Level 2
            { Guid.Parse("bce8d3d7-e93c-4e15-8254-acab0035a59f"), Groups.REFApprentice }, // ASMW Level 3
            { Guid.Parse("e8987ad0-2668-457e-9716-acab0035cb8a"), Groups.REFStudent }, // CSTS Workers
            { Guid.Parse("5fca3e5d-9ace-4891-a28c-acab0035cb98"), Groups.REFStudent }, // Fall Protection / Safety Monitor
            { Guid.Parse("29ca0c24-c074-437b-87ae-ad1f0182ab14"), Groups.REFStudent }, // Introduction to Roofing Trades
            { Guid.Parse("122987ec-350d-461e-896c-ad08011a5de8"), Groups.MasterRoofer }, // RCABC Master Roofer Designation
            { Guid.Parse("d5b78973-8207-4d71-88aa-acab0035a57b"), Groups.REFApprentice }, // Residential Level 1
            { Guid.Parse("b204137c-82c5-4fe7-bb9e-acab0035a56f"), Groups.REFApprentice }, // Roofer Level 1
            { Guid.Parse("5f678b5d-6e98-460f-81c0-acab0035a573"), Groups.REFApprentice }, // Roofer Level 2
            { Guid.Parse("ddfc2d26-1554-4169-aada-acab0035a573"), Groups.REFApprentice }, // Roofer Level 3
            { Guid.Parse("405e1c66-16f9-4251-9f65-acab0035a585"), Groups.REFApprentice }, // RSR Technical Training Level 1
            { Guid.Parse("c0dab4fc-5afc-472c-9d37-acab0035a585"), Groups.REFApprentice }, // RSR Technical Training Level 2
            { Guid.Parse("fd5691bc-2237-4aea-8416-acab0035a58b"), Groups.REFApprentice }, // RSR Technical Training Level 3
            { Guid.Parse("b86e4880-12d4-4608-a086-acab0035cbb0"), Groups.REFStudent }, // SiteReadyBC incl. WHMIS
            { Guid.Parse("b63a59a8-7179-4128-a032-acab0035a5a4"), Groups.REFStudent }, // Technical Training Level 1
            { Guid.Parse("7956dfbf-f4b0-455e-b8af-acab0035cbb4"), Groups.REFStudent }, // Torch Safety Certificate
            { Guid.Parse("c5c5d2a8-e41c-4910-960d-acab0035cbbe"), Groups.REFStudent }, // WHMIS
        };

        private readonly ICommander _commander;
        private readonly IChangeRepository _changes;

        private readonly IEventSearch _events;
        private readonly IRecordSearch _records;

        public RcabcRegistrationChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IChangeRepository changes,
            IEventSearch events,
            IRecordSearch records)
        {
            _commander = commander;
            _changes = changes;
            _events = events;
            _records = records;

            var rcabc = Shift.Constant.OrganizationIdentifiers.RCABC;

            publisher.Extend<AttendanceTaken>(Handle, rcabc);
            publisher.Extend<RegistrationRequested>(Handle, rcabc);
        }

        public void Handle(AttendanceTaken e)
        {
            var registration = GetRegistrationState(e.AggregateIdentifier);
            var gradebooks = _records.GetGradebooks(new QGradebookFilter
            {
                StudentIdentifier = registration.CandidateIdentifier,
                GradebookEventIdentifier = registration.EventIdentifier
            });

            foreach (var gradebook in gradebooks)
            {
                if (gradebook.IsLocked)
                    continue;

                var data = _records.GetGradebookState(gradebook.GradebookIdentifier);
                if (data.RootItems.Count == 0)
                    continue;

                var itemKey = data.RootItems[0].Identifier;

                var progress = _records.GetProgress(gradebook.GradebookIdentifier, itemKey, registration.CandidateIdentifier);
                if (progress == null)
                    continue;

                var percent = e.Status.HasValue() ? null : progress.ProgressPercent;

                Send(e, new ChangeProgressPercent(progress.ProgressIdentifier, percent, progress.ProgressGraded));
                Send(e, new ChangeProgressText(progress.ProgressIdentifier, e.Status, progress.ProgressGraded));
            }
        }

        public void Handle(RegistrationRequested e)
        {
            var @event = _events.GetEvent(e.Event);
            if (@event.AchievementIdentifier == null || !_achievementToGroup.TryGetValue(@event.AchievementIdentifier.Value, out var groupId))
                return;

            if (MembershipSearch.Exists(x => x.GroupIdentifier == groupId && x.UserIdentifier == e.Candidate))
                return;

            MembershipStore.Save(MembershipFactory.Create(e.Candidate, groupId, Shift.Constant.OrganizationIdentifiers.RCABC));
        }

        private Registration GetRegistrationState(Guid id)
        {
            return _changes.Get<RegistrationAggregate>(id).Data;
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
