using System;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class ActionNode 
    {
        public Guid Identifier { get; set; }
        public Guid? Parent { get; set; }
        public Guid? Permission { get; set; }

        public string Desktop { get; set; }
        public string Icon { get; set; }
        public string List { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Summary { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }

        public string Slug => "none";
    }
}
