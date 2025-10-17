using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Progress
{
    internal class LogbookExperienceItem : IComponent
    {
        private Experience _experience;
        public LogbookExperienceItem(Experience experience)
        {
            _experience = experience;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Element(ComposeExperienceHeader);
                column.Item().PaddingBottom(10);
                foreach (var item in _experience.ExperienceFields)
                    column.Item().PaddingLeft(10).Text(text =>
                    {
                        text.Line(item.Title).Style(QuestPdfText.BoldText);
                        text.Line(item.Value).Style(QuestPdfText.NormalText);
                    });
            });
        }

        void ComposeExperienceHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(100)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Text(_experience.Sequence.ToString())
                    .Style(QuestPdfText.BoldText);

                row.RelativeItem()
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Text(_experience.ExperienceCreated.ToString())
                    .Style(QuestPdfText.BoldText);

                row.RelativeItem()
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Text(_experience.Status)
                    .Style(QuestPdfText.BoldText);
            });
        }

    }
}
