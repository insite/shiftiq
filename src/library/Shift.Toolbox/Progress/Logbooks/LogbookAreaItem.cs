using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Progress
{
    internal class LogbookAreaItem : IComponent
    {
        private readonly Area _area;
        private readonly bool _showSkillRating;

        public LogbookAreaItem(Area area, bool showSkillRating)
        {
            _area = area;
            _showSkillRating = showSkillRating;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                .Background(Colors.Grey.Lighten3)
                .PaddingTop(10)
                .PaddingLeft(5)
                .PaddingBottom(10)
                .AlignMiddle()
                .Text(text =>
                {
                    text.Span(_area.Name).Style(QuestPdfText.BoldText);
                    text.Span($" ({_area.HoursCompleted:p0} Completed)").Style(QuestPdfText.NormalText);
                });

                foreach (var item in _area.Competencies)
                    column.Item().PaddingBottom(10).Component(new LogbookCompetencyItem(item, _showSkillRating));
            });
        }

    }
}