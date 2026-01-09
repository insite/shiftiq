using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionLikertRowGradeItemChanged : Change
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public Guid LikertRow { get; set; }
        public Guid? GradeItem { get; set; }

        public QuestionLikertRowGradeItemChanged(Guid form, Guid question, Guid likertRow, Guid? gradeItem)
        {
            Form = form;
            Question = question;
            LikertRow = likertRow;
            GradeItem = gradeItem;
        }
    }
}
