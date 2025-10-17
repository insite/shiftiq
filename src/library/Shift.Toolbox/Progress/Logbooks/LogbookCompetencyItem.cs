using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Progress
{
    internal class LogbookCompetencyItem : IComponent
    {
        private readonly Competency _competency;
        private readonly bool _showSkillRating;

        public LogbookCompetencyItem(Competency competency, bool showSkillRating)
        {
            _competency = competency;
            _showSkillRating = showSkillRating;
        }

        public void Compose(IContainer container)
        {
            var bgColor = (_competency.Sequence % 2 == 0) ? Colors.Grey.Lighten4 : Colors.White;
            var firstColumnSize = _showSkillRating ? 4 : 5;

            container.Row(row =>
            {
                row.RelativeItem(firstColumnSize)
                    .Background(bgColor)
                    .Padding(10)
                    .Text(_competency.Name)
                    .Style(QuestPdfText.NormalText);

                row.RelativeItem(1)
                    .Background(bgColor)
                    .Padding(10)
                    .Text(_competency.Hours)
                    .Style(QuestPdfText.NormalText);

                row.RelativeItem(1)
                    .Background(bgColor)
                    .Padding(10)
                    .Text(_competency.JournalItems)
                    .Style(QuestPdfText.NormalText);

                if (_showSkillRating)
                    row.RelativeItem(1)
                        .Background(bgColor)
                        .Padding(10)
                        .Text(_competency.SkillRating)
                        .Style(QuestPdfText.NormalText);
            });
        }

    }
}