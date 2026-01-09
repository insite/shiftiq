using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class CourseListItem
    {
        public CourseListItem()
        {
            Modules = new List<ModuleListItem>();
        }

        public CourseListItem(Guid identifier, string name) : this()
        {
            Identifier = identifier;
            Name = name;
        }

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public List<ModuleListItem> Modules { get; set; }
    }
}
