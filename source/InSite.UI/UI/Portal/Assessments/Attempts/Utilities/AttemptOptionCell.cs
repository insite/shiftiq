using Shift.Common;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AttemptOptionCell
    {
        public AttemptOptionCell(QuestionTable.CellData cell)
        {
            CssClass = "text";
            if (cell.CssClass.IsNotEmpty())
                CssClass += " " + cell.CssClass;

            AlignmentStyle = $"text-align:{cell.Alignment.ToString().ToLower()}";
            Text = cell.Text;
        }

        public string CssClass { get; set; }
        public string AlignmentStyle { get; set; }
        public string Text { get; set; }
    }
}