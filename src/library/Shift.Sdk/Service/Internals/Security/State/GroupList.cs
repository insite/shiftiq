using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class GroupList : List<Group>
    {
        public GroupList() { }

        public GroupList(Group[] groups)
        {
            Clear();
            foreach (var group in groups)
                Add(group);
        }

        public bool Contains(string name)
            => this.Any(x => string.Compare(x.Name, name, true) == 0);

        public string GetIdentifiersAsCsv()
        {
            var sb = new StringBuilder();
            foreach (var item in this)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(item.Identifier);
            }
            return sb.ToString();
        }
    }
}
