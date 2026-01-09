
using System.Collections.Generic;

namespace Shift.Toolbox.Assessments.Models
{
    public class OptionItem
    {
        public int Sequence { get; set; }
        public int QuestionSequence { get; set; }
        public int? AnswerSequence { get; set; }
        public bool? IsSelected { get; set; }
        public bool? IsTrue { get; set; }
        public string Text { get; set; }
        public bool HasPoints { get; set; }

        public List<OptionCellItem> Cells { get; set; }
    }
}
