using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations.PerformanceReport
{
    [Serializable]
    public class ItemWeight
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }

        public bool IsEqual(ItemWeight other)
        {
            return Name.NullIfEmpty() == other.Name.NullIfEmpty()
                && Weight == other.Weight;
        }

        public static bool IsEqual(ICollection<ItemWeight> collection1, ICollection<ItemWeight> collection2)
        {
            return collection1.Count == collection2.Count
                && collection1.Zip(collection2, (a, b) => a.IsEqual(b)).All(x => x);
        }

        public ItemWeight Clone()
        {
            return new ItemWeight
            {
                Name = Name.NullIfEmpty(),
                Weight = Weight,
            };
        }
    }
}
