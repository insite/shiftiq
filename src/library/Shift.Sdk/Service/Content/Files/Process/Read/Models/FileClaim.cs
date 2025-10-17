using System;
using System.Collections.Generic;

namespace InSite.Application.Files.Read
{
    [Serializable]
    public class FileClaim
    {
        public Guid ObjectIdentifier { get; set; }
        public FileClaimObjectType ObjectType { get; set; }

        public FileClaim Clone() => (FileClaim)MemberwiseClone();

        public static IEnumerable<FileClaim> CloneList(IEnumerable<FileClaim> list)
        {
            var clone = new List<FileClaim>();

            if (list != null)
            {
                foreach (var claim in list)
                    clone.Add(claim.Clone());
            }

            return clone;
        }
    }
}
