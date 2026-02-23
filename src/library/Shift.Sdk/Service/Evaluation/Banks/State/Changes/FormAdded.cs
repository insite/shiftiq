using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormAdded : Change
    {
        public Guid Specification { get; set; }
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Asset { get; set; }
        public int? TimeLimit { get; set; }

        public FormAdded(Guid spec, Guid identifier, string name, int asset, int? timeLimit)
        {
            Specification = spec;
            Identifier = identifier;
            Name = name;
            Asset = asset;
            TimeLimit = timeLimit;
        }
    }
}