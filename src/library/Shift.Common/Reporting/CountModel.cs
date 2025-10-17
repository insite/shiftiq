namespace Shift.Common
{
    public class CountModel
    {
        public string Icon { get; set; }
        public string Name => Humanizer.Pluralize(Count == 1 ? (Label ?? Type) : ((Label ?? Type)));
        public string Type { get; set; }
        public string Label { get; set; }

        public int Count { get; set; }

        public CountModel()
        {

        }

        public CountModel(string type, string label, string icon, int count)
        {
            Type = type;
            Label = label;
            Icon = icon;
            Count = count;
        }
    }
}