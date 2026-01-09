using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TDepartmentStandardStore
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

        public static void Insert(IEnumerable<TDepartmentStandard> list)
        {
            var commands = new List<ICommand>();

            foreach (var group in list.GroupBy(x => x.StandardIdentifier))
            {
                commands.Add(new AddStandardGroup(
                    group.Key,
                    group.Select(x => new InSite.Domain.Standards.StandardGroup
                    {
                        GroupId = x.DepartmentIdentifier
                    }).ToArray()
                ));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void Delete(IEnumerable<TDepartmentStandard> list)
        {
            var commands = new List<ICommand>();

            foreach (var group in list.GroupBy(x => x.StandardIdentifier))
            {
                commands.Add(new RemoveStandardGroup(
                    group.Key,
                    group.Select(x => x.DepartmentIdentifier).ToArray()
                ));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void InsertPermissions(Guid departmentId, IEnumerable<Guid> standardIds)
        {
            foreach (var standardId in standardIds)
                _sendCommand(new AddStandardGroup(standardId, departmentId));
        }

        public static void DeleteByDepartment(Guid departmentId, IEnumerable<Guid> standardIds)
        {
            foreach (var standardId in standardIds)
                _sendCommand(new RemoveStandardGroup(standardId, departmentId));
        }
    }
}
