using System;

namespace Shift.Sdk.UI
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } = decimal.Zero;
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public string ProductUrl { get; set; }
    }
}
