using System;

namespace Shift.Common
{
    [Serializable]
    public class ListItem : IComparable, IReadOnlyListItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public bool Selected { get; set; }
        public bool? Enabled { get; set; }
        public string Icon { get; set; }

        public ListItem Clone()
        {
            return (ListItem)MemberwiseClone();
        }

        public int CompareTo(object o)
        {
            var other = (ListItem)o;

            return string.Compare(Text, other.Text, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public interface IReadOnlyListItem
    {
        string Value { get; }
        string Text { get; }
        string Color { get; }
        string Description { get; }
        int Index { get; }
        bool Selected { get; }
        bool? Enabled { get; }
        string Icon { get; }
    }
}
