using System;

namespace InSite.UI.Portal.Billing.Models
{
    public class PriceSelectionModel

    {
        public enum PriceSelectionMode { ALaCarte, Package, Subscribe, Select }

        public sealed class PriceSelectionChangedEventArgs : EventArgs
        {
            public PriceSelectionMode Mode { get; }
            public Guid? PackageProductIdentifier { get; }
            public PriceSelectionChangedEventArgs(PriceSelectionMode mode, Guid? pkgId)
            { 
                Mode = mode; 
                PackageProductIdentifier = pkgId; 
            }
        }
    }
}