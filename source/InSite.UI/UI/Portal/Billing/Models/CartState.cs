using System;
using System.Collections.Generic;
using System.Linq;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

using Shift.Common;

namespace InSite.UI.Portal.Billing.Models
{
    [Serializable]
    public class CartState
    {
        public PriceSelectionMode Mode { get; private set; } = PriceSelectionMode.ALaCarte;
        public Guid? PackageProductId { get; private set; }
        public int? PackageQuantity { get; private set; }
        public Dictionary<Guid, int> Items { get; } = new Dictionary<Guid, int>();

        public int TotalSelected => Items.Values.Sum();
        public int Remaining => Mode == PriceSelectionMode.Package && PackageQuantity.HasValue
            ? Math.Max(PackageQuantity.Value - TotalSelected, 0)
            : 0;

        public void Reset(PriceSelectionMode mode, Guid? packageId = null, int? packageQty = null)
        {
            Items.Clear();
            Mode = mode;
            PackageProductId = packageId;
            PackageQuantity = packageQty;
        }

        public int Add(Guid productId, int qty, out int actuallyAdded)
        {
            actuallyAdded = 0;
            if (qty <= 0) return TotalSelected;

            if (Mode == PriceSelectionMode.Package && PackageQuantity.HasValue)
            {
                var remainder = Math.Max(PackageQuantity.Value - TotalSelected, 0);
                if (remainder <= 0) return TotalSelected;
                if (qty > remainder) qty = remainder;
            }

            var curQty = Items.GetOrDefault(productId, 0);
            var newQty = Number.CheckRange(curQty + qty, maxValue: 9999);

            Items[productId] = newQty;
            actuallyAdded = newQty - curQty;

            return TotalSelected;
        }
    }
}