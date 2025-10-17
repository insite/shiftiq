using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Assessments
{
    internal class AssessmentEssayItem : IComponent
    {
        private string _text;
        private Image _correctSqr;
        private Image _incorrectSqr;
        private decimal? _answerPoints;


        public AssessmentEssayItem(string text, Image correctSqr, Image incorrectSqr, decimal? answerPoints)
        {
            _text = text;
            _incorrectSqr = incorrectSqr;
            _correctSqr = correctSqr;
            _answerPoints = answerPoints;
        }

        public void Compose(IContainer container)
        {
            container.Row(row =>
            {
                row.AutoItem()
                .AlignMiddle()
                .AlignLeft()
                .PaddingLeft(20)
                .Text(_text)
                .Style(QuestPdfText.NormalText);

                if (!_answerPoints.HasValue)
                    return;

                row.RelativeItem()
                    .AlignMiddle()
                    .AlignRight()
                    .PaddingRight(20)
                    .ShrinkVertical()
                    .Width(20)
                    .Image((_answerPoints.Value > 0 ? _correctSqr : _incorrectSqr));

            });
        }
    }
}
