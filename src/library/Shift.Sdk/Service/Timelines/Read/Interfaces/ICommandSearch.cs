using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Logs.Read
{
    public interface ICommandSearch
    {
        int Count(CommandFilter filter);
        
        SerializedCommand Get(Guid id);
        List<SerializedCommand> Get(CommandFilter filter);

        ICommand GetCommand(Guid id);
    }
}