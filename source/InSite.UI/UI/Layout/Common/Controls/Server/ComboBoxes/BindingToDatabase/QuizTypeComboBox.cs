using Shift.Common;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class QuizTypeComboBox : ComboBox
    {
        private static readonly string[] _items = new[]
        {
            QuizType.TypingSpeed,
            QuizType.TypingAccuracy
        };

        protected override ListItemArray CreateDataSource() => new ListItemArray(_items);
    }
}