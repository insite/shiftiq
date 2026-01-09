using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public class CommandBroker
    {
        private readonly ICommander _commander;

        public CommandBroker(ICommander commander)
        {
            _commander = commander;
        }

        public void Bookmark(IChange cause, ICommand effect, DateTimeOffset at)
        {
            Identify(cause, effect);
            _commander.Bookmark(effect, at);
        }

        public static ProcessState CreateProcess(string description, Indicator indicator = Indicator.Default, string[] errors = null)
            => new ProcessState { Description = description, Execution = ExecutionState.Created, Indicator = indicator, Errors = errors };

        private void Identify(IChange cause, ICommand effect)
        {
            effect.OriginOrganization = cause.OriginOrganization;
            effect.OriginUser = cause.OriginUser;
        }

        public void Send(IChange cause, ICommand effect)
        {
            Identify(cause, effect);
            _commander.Send(effect);
        }
    }
}