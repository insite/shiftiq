using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Shift.Utility.Assessments.Models;

namespace Shift.Toolbox.Assessments
{
    internal class AssessmentListOptionItem : IComponent
    {
        private OptionItem _option;
        private Image _correct;
        private Image _incorrect;
        private Image _empty;

        public AssessmentListOptionItem(OptionItem option, Image correct, Image incorrect, Image empty)
        {
            _option = option;
            _correct = correct;
            _incorrect = incorrect;
            _empty = empty;
        }

        public void Compose(IContainer container)
        {
            container.Row(row =>
            {
                var isCorrect = _option.IsSelected == true
                    ? _option.IsTrue.HasValue && _option.IsTrue.Value || !_option.IsTrue.HasValue && _option.HasPoints
                    : (bool?)null;

                row.AutoItem()
                    .AlignMiddle()
                    .PaddingLeft(20)
                    .Width(10)
                    .Image(GetAnswerOptionIcon(isCorrect)).FitArea();

                row.RelativeItem()
                    .PaddingLeft(10)
                    .ShrinkVertical()
                    .Text($"{_option.Text}")
                    .Style(QuestPdfText.NormalText);
            });
        }

        private Image GetAnswerOptionIcon(bool? isCorrect)
        {
            if (!isCorrect.HasValue)
                return _empty;
            else if (isCorrect.Value)
                return _correct;
            else
                return _incorrect;
        }
    }
}
