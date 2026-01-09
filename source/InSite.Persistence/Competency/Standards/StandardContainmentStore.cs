using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardContainmentStore
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

        public static void CopyByFrom(Guid sourceStandardId, Guid destStandardId)
        {
            InSite.Domain.Standards.StandardContainment[] containments;
            using (var db = new InternalDbContext())
                containments = db.StandardContainments
                    .Where(x => x.ParentStandardIdentifier == sourceStandardId)
                    .Select(x => new InSite.Domain.Standards.StandardContainment
                    {
                        ChildStandardId = x.ChildStandardIdentifier,
                        CreditHours = x.CreditHours,
                        CreditType = x.CreditType
                    })
                    .ToArray();

            if (containments.IsNotEmpty())
                _sendCommand(new AddStandardContainment(destStandardId, containments));
        }

        public static void Insert(IEnumerable<StandardContainment> list)
        {
            var commands = new List<ICommand>();

            foreach (var group in list.GroupBy(x => x.ParentStandardIdentifier))
            {
                commands.Add(new AddStandardContainment(
                    group.Key,
                    group.Select(x => new InSite.Domain.Standards.StandardContainment
                    {
                        ChildStandardId = x.ChildStandardIdentifier,
                        ChildSequence = x.ChildSequence,
                        CreditHours = x.CreditHours,
                        CreditType = x.CreditType
                    }).ToArray()
                ));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void Insert(Guid from, Guid to)
        {
            _sendCommand(new AddStandardContainment(from, to, default, default, default));
        }

        public static void CopyChildCompetencies(Guid sourceStandardId, Guid destStandardId)
        {
            InSite.Domain.Standards.StandardContainment[] containments;
            using (var db = new InternalDbContext())
                containments = db.StandardContainments
                    .Where(x =>
                        x.ParentStandardIdentifier == sourceStandardId
                        && x.Child.StandardType == StandardType.Competency
                        && !db.StandardContainments.Any(y => y.ParentStandardIdentifier == destStandardId && y.ChildStandardIdentifier == x.ChildStandardIdentifier)
                    )
                    .Select(x => new InSite.Domain.Standards.StandardContainment
                    {
                        ChildStandardId = x.ChildStandardIdentifier
                    })
                    .ToArray();


            if (containments.IsNotEmpty())
                _sendCommand(new AddStandardContainment(destStandardId, containments));
        }

        public static void Update(IEnumerable<StandardContainment> list)
        {
            var commands = new List<ICommand>();

            foreach (var group in list.GroupBy(x => x.ParentStandardIdentifier))
            {
                commands.Add(new ModifyStandardContainment(
                    group.Key,
                    group.Select(x => new InSite.Domain.Standards.StandardContainment
                    {
                        ChildStandardId = x.ChildStandardIdentifier,
                        ChildSequence = x.ChildSequence,
                        CreditHours = x.CreditHours,
                        CreditType = x.CreditType
                    }).ToArray()
                ));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static StandardContainment Update(Guid parentId, Guid childId, Action<StandardContainment> change)
        {
            StandardContainment result = null;
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                result = db.StandardContainments.Single(
                    x => x.ParentStandardIdentifier == parentId && x.ChildStandardIdentifier == childId);

                change(result);

                commands.Add(new ModifyStandardContainment(
                    parentId,
                    result.ChildStandardIdentifier,
                    result.ChildSequence,
                    result.CreditHours,
                    result.CreditType
                ));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);

            return result;
        }

        public static void Delete(Guid parentStandardId, Guid childStandardId)
        {
            _sendCommand(new RemoveStandardContainment(parentStandardId, childStandardId));
        }

        public static void Delete(Expression<Func<StandardContainment, bool>> filter)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                var list = db.StandardContainments.Where(filter).Select(x => new
                {
                    x.ParentStandardIdentifier,
                    x.ChildStandardIdentifier
                }).ToArray();

                if (list.IsEmpty())
                    return;

                foreach (var group in list.GroupBy(x => x.ParentStandardIdentifier))
                {
                    commands.Add(new RemoveStandardContainment(
                        group.Key,
                        group.Select(x => x.ChildStandardIdentifier).ToArray()
                    ));
                }
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }
    }
}
