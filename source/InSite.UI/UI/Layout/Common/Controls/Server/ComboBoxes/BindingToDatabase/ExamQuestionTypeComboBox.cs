using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ExamQuestionTypeComboBox : ComboBox
    {
        private class DataItem : ListItem
        {
            public DataItem(QuestionItemType value)
            {
                Value = value.GetName();
                Icon = "fas fa-" + value.GetIconClass();
                Text = value.GetDescription();
            }
        }

        private static readonly DataItem[] _items = new[]
        {
            new DataItem(QuestionItemType.SingleCorrect),
            new DataItem(QuestionItemType.TrueOrFalse),
            new DataItem(QuestionItemType.MultipleCorrect),
            new DataItem(QuestionItemType.ComposedEssay),
            new DataItem(QuestionItemType.ComposedVoice),
            new DataItem(QuestionItemType.BooleanTable),
            new DataItem(QuestionItemType.Matching),
            new DataItem(QuestionItemType.Likert),
            new DataItem(QuestionItemType.HotspotStandard),
            new DataItem(QuestionItemType.HotspotImageCaptcha),
            new DataItem(QuestionItemType.HotspotMultipleChoice),
            new DataItem(QuestionItemType.HotspotMultipleAnswer),
            new DataItem(QuestionItemType.HotspotCustom),
            new DataItem(QuestionItemType.Ordering),
        };

        public QuestionItemType? ValueAsEnum
        {
            get => Value.ToEnumNullable<QuestionItemType>();
            set => Value = value?.GetName();
        }

        protected override ListItemArray CreateDataSource() => new ListItemArray(_items);
    }
}