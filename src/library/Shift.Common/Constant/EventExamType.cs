using System.Linq;

namespace Shift.Common
{
    public static class EventExamType
    {
        public static IReadOnlyListItem Class => _class;
        public static IReadOnlyListItem Individual => _individual;
        public static IReadOnlyListItem IndividualA => _individualA;
        public static IReadOnlyListItem IndividualN => _individualN;
        public static IReadOnlyListItem Sitting => _sitting;
        public static IReadOnlyListItem Test => _test;
        public static IReadOnlyListItem Arc => _arc;

        private static readonly ListItem _class = new ListItem { Value = "Class", Text = "Class" };
        private static readonly ListItem _individual = new ListItem { Value = "Individual", Text = "(Do not use) Individual" };
        private static readonly ListItem _individualA = new ListItem { Value = "Individual (A)", Text = "Individual (Accommodated)" };
        private static readonly ListItem _individualN = new ListItem { Value = "Individual (N)", Text = "Individual (Not Accommodated)" };
        private static readonly ListItem _sitting = new ListItem { Value = "Sitting", Text = "Sitting" };
        private static readonly ListItem _test = new ListItem { Value = "Test", Text = "Test" };
        private static readonly ListItem _arc = new ListItem { Value = "Accessibility and Resource Centre (ARC)", Text = "Accessibility and Resource Centre (ARC)" };

        private static readonly ListItemArray _all = new ListItemArray { _class, _individual, _individualA, _individualN, _sitting, _test, _arc };

        public static ListItemArray GetDataSource() => new ListItemArray(_all.Select(x => x.Clone()));

        public static bool ContainsValue(string value) => _all.ContainsValue(value);
    }
}