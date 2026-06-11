using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class AddStandardGroup : Command
    {
        public StandardGroup[] Groups { get; set; }

        public AddStandardGroup(Guid standardId, Guid groupId, DateTimeOffset? assigned = null)
            : this(standardId, new[] { new StandardGroup(groupId, assigned) })
        {
        }

        public AddStandardGroup(Guid standardId, StandardGroup[] groups)
        {
            AggregateIdentifier = standardId;
            Groups = groups;
        }
    }
}
