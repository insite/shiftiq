using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionLikertRowGradeItem : Command
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public Guid LikertRow { get; set; }
        public Guid? GradeItem { get; set; }

        public ChangeQuestionLikertRowGradeItem(Guid bank, Guid form, Guid question, Guid likertRow, Guid? gradeItem)
        {
            AggregateIdentifier = bank;
            Form = form;
            Question = question;
            LikertRow = likertRow;
            GradeItem = gradeItem;
        }
    }
}
