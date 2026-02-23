using System.Collections.Generic;
using System.Threading.Tasks;

using Shift.Common.Timeline.Commands;

namespace Shift.Sdk.Service
{
    public interface ICommanderAsync
    {
        Task SendCommandAsync(ICommand command);
        Task SendCommandsAsync(IEnumerable<ICommand> commands);
    }
}
