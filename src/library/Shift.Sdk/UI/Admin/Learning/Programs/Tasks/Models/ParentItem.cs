using System;

namespace Shift.Sdk.UI
{
    public class ParentItem
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public ParentItem(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}
