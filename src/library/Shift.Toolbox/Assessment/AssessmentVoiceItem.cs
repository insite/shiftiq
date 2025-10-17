using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Assessments
{
    internal class AssessmentVoiceItem : IComponent
    {
        private Image _audio;
        private Image _correctSqr;
        private Image _incorrectSqr;
        private decimal? _answerPoints;

        public AssessmentVoiceItem(Image audio, Image correctSqr, Image incorrectSqr, decimal? answerPoints)
        {
            _audio = audio;
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
                .Width(350)
                .PaddingLeft(20)
                .Image(_audio).FitArea();

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
