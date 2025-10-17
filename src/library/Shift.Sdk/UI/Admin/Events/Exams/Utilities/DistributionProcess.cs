using System.Linq;

namespace Shift.Sdk.UI
{
    public static class DistributionProcess
    {
        public class Item
        {
            public int Number { get; }
            public string Text { get; }
            public string Value { get; }

            public Item(int number, string text, string value)
            {
                Number = number;
                Text = text;
                Value = value;
            }
        }

        public static readonly Item[] Items = new [] 
        { 
            new Item(1, "BC Mail", "Standard"),
            new Item(2, "Late Addition - Training Provider", "Late Addition - Training Provider"),
            new Item(3, "Late Addition - Internal", "Late Addition - Internal"),
            new Item(4, "Escalation", "Escalation")
        };

        public static string GetText(string value) =>
            string.IsNullOrEmpty(value) ? null : Items.Where(x => string.Equals(x.Value, value)).FirstOrDefault()?.Text;

        public static string GetValue(string text) =>
            string.IsNullOrEmpty(text) ? null : Items.Where(x => string.Equals(x.Text, text)).FirstOrDefault()?.Value;
    }
}