using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionGradeItemChanged2 : Change
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public Guid? GradeItem { get; set; }

        public QuestionGradeItemChanged2(Guid form, Guid question, Guid? gradeItem)
        {
            Form = form;
            Question = question;
            GradeItem = gradeItem;
        }
    }
}
