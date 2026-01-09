using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddForm : Command
    {
        public Guid Specification { get; set; }
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Asset { get; set; }
        public int? TimeLimit { get; set; }

        public AddForm(Guid bank, Guid spec, Guid identifier, string name, int asset, int? timeLimit)
        {
            AggregateIdentifier = bank;

            Specification = spec;
            Identifier = identifier;
            Name = name;
            Asset = asset;
            TimeLimit = timeLimit;
        }
    }
}