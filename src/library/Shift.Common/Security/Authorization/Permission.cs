using System.Collections.Generic;

using Shift.Common;

namespace Shift.Common
{
    public class Permission
    {
        public Access Access { get; set; }
        public Resource Resource { get; set; }
        public Role Role { get; set; }
    }

    public class Requirement
    {
        public string Policy { get; set; }

        public List<string> Allows { get; set; } = new List<string>();

        public Requirement(string policy)
        {
            if (AccessImpliesRead(policy))
                Policy = ReplaceLastPathSegment(policy, DataAccess.Read);

            else if (AccessImpliesWrite(policy))
                Policy = ReplaceLastPathSegment(policy, DataAccess.Write);

            else if (AccessImpliesDelete(policy))
                Policy = ReplaceLastPathSegment(policy, DataAccess.Delete);

            else
                Policy = policy;

            if (!StringHelper.Equals(Policy, policy))
                Allows.Add(policy);
        }

        private bool AccessImpliesRead(string path)
        {
            return StringHelper.EndsWithAny(path, new string[] { "Assert", "Collect", "Count", "Retrieve", "Search", "Export" });
        }

        private bool AccessImpliesWrite(string path)
        {
            return StringHelper.EndsWithAny(path, new string[] { "Create", "Import", "Modify" });
        }

        private bool AccessImpliesDelete(string path)
        {
            return StringHelper.EndsWithAny(path, new string[] { "Delete", "Purge" });
        }

        private string ReplaceLastPathSegment(string name, DataAccess access)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var lastDot = name.LastIndexOf('.');

            if (lastDot == -1 || lastDot == name.Length - 1)
                return name;

            return name.Substring(0, lastDot) + $".{access}";
        }
    }
}