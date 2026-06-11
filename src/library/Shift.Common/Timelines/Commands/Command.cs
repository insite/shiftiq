using System;

namespace Shift.Common.Timeline.Commands
{
    /// <summary>
    /// Defines the base class for all commands.
    /// </summary>
    /// <remarks>
    /// A command is a request to change the domain. It is always are named with a verb in the imperative mood, such as 
    /// Confirm Order. Unlike an event, a command is not a statement of fact; it is only a request, and thus may be 
    /// refused. Commands are immutable because their expected usage is to be sent directly to the domain model for 
    /// processing. They do not need to change during their projected lifetime.
    /// </remarks>
    public class Command : ICommand
    {
        public Guid AggregateIdentifier { get; set; }
        public int? ExpectedVersion { get; set; }

        public Guid OriginOrganization { get; set; }
        public Guid OriginUser { get; set; }

        public Guid CommandIdentifier { get; set; }
        
        public Command() 
        {
            var generator = Services.ServiceLocator.Instance.GetService<Services.IGuidGenerator>();
            CommandIdentifier = generator.NewGuid(); 
        }
    }
}
