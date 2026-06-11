using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionDeleted : Change
    {
        public Guid Question { get; set; }
        public bool RemoveAllVersions { get; set; }

        public QuestionDeleted(Guid question, bool removeAllVersions)
        {
            Question = question;
            RemoveAllVersions = removeAllVersions;
        }
    }
}
