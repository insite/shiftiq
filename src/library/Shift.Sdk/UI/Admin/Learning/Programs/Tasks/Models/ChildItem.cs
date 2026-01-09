using System;

namespace Shift.Sdk.UI
{
    public class ChildItem
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public ChildItem(string name, string parentName, Guid id, Guid parentId)
        {
            Name = name;
            ParentName = parentName;
            ParentId = parentId;
            Id = id;
        }
    }
}
