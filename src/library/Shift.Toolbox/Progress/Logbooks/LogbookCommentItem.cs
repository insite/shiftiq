using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Progress
{
    internal class LogbookCommentItem : IComponent
    {
        private Comment _comment;
        public LogbookCommentItem(Comment comment)
        {
            _comment = comment;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingLeft(10).Text(text =>
                {
                    text.Line(_comment.Title).Style(QuestPdfText.BoldText);
                    text.Line(_comment.Text).Style(QuestPdfText.NormalText);
                    text.Line(_comment.PostedOn).Style(QuestPdfText.NormalText);
                    text.Line(_comment.AuthorName).Style(QuestPdfText.SmallItalicText);
                });
                column.Item().Padding(5).PaddingBottom(10).LineHorizontal(1).LineColor(QuestPdfStyle.CardBorderColor);
            });
        }
    }
}