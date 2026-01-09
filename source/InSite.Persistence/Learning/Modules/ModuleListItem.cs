using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class ModuleListItem
    {
        public ModuleListItem() 
        {
            Activities = new List<ActivityListItem>();
        }

        public ModuleListItem(Guid identifier, string name) : this()
        {
            Identifier = identifier;
            Name = name;
        }

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public List<ActivityListItem> Activities { get; set; }
    }
}
