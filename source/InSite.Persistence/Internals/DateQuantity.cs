using System;

namespace InSite.Persistence
{
    public class DateQuantity
    {
        public DateTime Item { get; set; }
        public int Quantity { get; set; }

        public DateQuantity() { }

        public DateQuantity(DateTime item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
