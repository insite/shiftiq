using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox
{
    public static class QuestPdfText
    {
        public const string FFN_Calibri = "Calibri";

        public static TextStyle ReportTitle => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(14).SemiBold();
        public static TextStyle NormalText => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(10);
        public static TextStyle BoldText => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(10).Bold();
        public static TextStyle ItalicText => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(10).Italic();
        public static TextStyle SmallText => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(8);
        public static TextStyle SmallItalicText => TextStyle.Default.FontFamily(FFN_Calibri).FontColor(Colors.Black).FontSize(8).Italic();
    }
}
