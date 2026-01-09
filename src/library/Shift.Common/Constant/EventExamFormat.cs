using System.Linq;

namespace Shift.Common
{
    public static class EventExamFormat
    {
        public static IReadOnlyListItem Online => _online;
        public static IReadOnlyListItem Paper => _paper;

        private static readonly ListItem _online = new ListItem { Value = "Online", Text = "Online" };
        private static readonly ListItem _paper = new ListItem { Value = "Paper", Text = "Paper" };

        private static readonly ListItemArray _all = new ListItemArray { _online, _paper };

        public static ListItemArray GetDataSource() => new ListItemArray(_all.Select(x => x.Clone()));

        public static bool ContainsValue(string value) => _all.ContainsValue(value);
    }
}
