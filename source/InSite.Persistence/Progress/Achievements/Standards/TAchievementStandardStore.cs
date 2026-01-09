using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

namespace InSite.Persistence
{
    public static class TAchievementStandardStore
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        public static void Insert(IEnumerable<TAchievementStandard> list)
        {
            var commands = new List<ICommand>();
            foreach (var group in list.GroupBy(x => x.StandardIdentifier))
                commands.Add(new AddStandardAchievement(group.Key, group.Select(x => x.AchievementIdentifier).ToArray()));

            _sendCommands(commands);
        }

        public static void Delete(IEnumerable<TAchievementStandard> list)
        {
            var commands = new List<ICommand>();
            foreach (var group in list.GroupBy(x => x.StandardIdentifier))
                commands.Add(new RemoveStandardAchievement(group.Key, group.Select(x => x.AchievementIdentifier).ToArray()));

            _sendCommands(commands);
        }
    }
}
