using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;

using Shift.Constant;

namespace InSite.Persistence
{
    public class ProgramService : IProgramService
    {
        private readonly IContactSearch _contactSearch;
        private readonly IAchievementSearch _achievementSearch;
        private readonly IProgramSearch _programSearch;
        private readonly IProgramStore _programStore;
        private static ICommander _commander;

        public ProgramService(
            IContactSearch contactSearch,
            IAchievementSearch achievementSearch,
            IProgramSearch programSearch,
            IProgramStore programStore
            )
        {
            _contactSearch = contactSearch;
            _achievementSearch = achievementSearch;
            _programSearch = programSearch;
            _programStore = programStore;
        }

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public void CompletionOfProgramAchievement(Guid ProgramIdentifier, Guid LearnerIdentifier, Guid OrganizationIdentifier)
        {
            var program = ProgramSearch.GetProgram(ProgramIdentifier);
            if (program == null)
                return;

            var isProgramCompleted =
            (
                program.CompletionTaskIdentifier.HasValue
                && _programSearch.IsTaskCompletedByLearner(program.CompletionTaskIdentifier.Value, LearnerIdentifier)
            )
            || _programSearch.IsProgramFullyCompletedByLearner(program.ProgramIdentifier, LearnerIdentifier);

            var completion = isProgramCompleted ? DateTimeOffset.UtcNow : (DateTimeOffset?)null;

            ProgramStore.InsertEnrollment(OrganizationIdentifier, program.ProgramIdentifier, LearnerIdentifier, LearnerIdentifier, completion);

            if (!isProgramCompleted)
                return;

            if (!program.AchievementIdentifier.HasValue)
                return;

            if(_achievementSearch.GetCredential(program.AchievementIdentifier.Value, LearnerIdentifier) == null)
                SendGrantCommands(TriggerEffectCommand.Grant, OrganizationIdentifier, program.AchievementIdentifier.Value, LearnerIdentifier);

            _programStore.TaskCompleted(LearnerIdentifier, OrganizationIdentifier, program.AchievementIdentifier.Value);
        }

        private void SendGrantCommands(TriggerEffectCommand effect, Guid organization, Guid achievement, Guid user)
        {
            var commands = new List<Command>();

            ProgramEnrollment.BuildCommands(effect, organization, achievement, user, commands, _achievementSearch, _contactSearch);

            SendCommands(commands);
        }

        private void SendCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
                _commander.Send(command);
        }
    }
}
