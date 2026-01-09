using System;
using System.Collections.Generic;
using System.Text;

namespace InSite.Persistence.Plugin.CMDS
{
    [Obsolete("Replace with VCmdsAchievementDependencyList")]
    public class ResourceDependencyList
    {
        public int Count => Items.Count;

        public List<ResourceDependency> Items { get; set; }

        public ResourceDependencyList()
        {
            Items = new List<ResourceDependency>();
        }

        public void Add(string type, string name)
        {
            var dependency = new ResourceDependency
            {
                Type = type,
                Name = name
            };
            Items.Add(dependency);
        }

        public object ToHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in Items)
                sb.Append($"<li><span class='badge bg-custom-default'>{item.Type}</span> {item.Name}</li>");
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}
