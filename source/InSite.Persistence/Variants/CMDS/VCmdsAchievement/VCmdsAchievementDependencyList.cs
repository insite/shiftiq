using System.Collections.Generic;
using System.Text;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementDependencyList
    {
        public int Count => Items.Count;

        public List<VCmdsAchievementDependency> Items { get; set; }

        public VCmdsAchievementDependencyList()
        {
            Items = new List<VCmdsAchievementDependency>();
        }

        public void Add(string type, string name)
        {
            var dependency = new VCmdsAchievementDependency
            {
                Type = type,
                Name = name
            };
            Items.Add(dependency);
        }

        public object ToHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<ul class='mt-3'>");
            foreach (var item in Items)
                sb.Append($"<li><span class='badge bg-custom-default'>{item.Type}</span> {item.Name}</li>");
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}
