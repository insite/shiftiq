using System;
using System.Collections.Generic;
using System.Text;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class UserList : List<User>
    {
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
