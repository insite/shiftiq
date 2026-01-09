using QuestPDF.Fluent;

namespace Shift.Toolbox.Progress
{
    public class LogbookResultCreator
    {
        public byte[] CreatePdf(LogbookModel logbookModel)
        {
            return new LogbookResultDocument(logbookModel).GeneratePdf();
        }
    }
}
