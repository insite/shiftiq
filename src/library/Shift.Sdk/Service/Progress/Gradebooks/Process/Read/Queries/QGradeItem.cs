using System;
using System.Collections.Generic;

using InSite.Application.Courses.Read;

namespace InSite.Application.Records.Read
{
    public class QGradeItem
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid? ParentGradeItemIdentifier { get; set; }
        public Guid? ProgressCompletedMessageIdentifier { get; set; }

        public string AchievementElseCommand { get; set; }
        public string AchievementThenCommand { get; set; }
        public string AchievementWhenChange { get; set; }
        public string AchievementWhenGrade { get; set; }

        public string GradeItemCode { get; set; }
        public string GradeItemFormat { get; set; }
        public string GradeItemHook { get; set; }
        public string GradeItemName { get; set; }
        public string GradeItemReference { get; set; }
        public string GradeItemShortName { get; set; }
        public string GradeItemType { get; set; }
        public string GradeItemWeighting { get; set; }
        
        public string StatementWhenObject { get; set; }
        public string StatementWhenVerb { get; set; }
        public string StatementThenCommand { get; set; }

        public bool GradeItemIsReported { get; set; }

        public int GradeItemSequence { get; set; }

        public decimal? GradeItemMaxPoints { get; set; }
        public decimal? GradeItemPassPercent { get; set; }

        public DateTimeOffset? AchievementFixedDate { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual QGradeItem Parent { get; set; }
        public virtual QAchievement Achievement { get; set; }

        public virtual ICollection<QActivity> Activities { get; set; } = new HashSet<QActivity>();
        public virtual ICollection<QGradeItem> Children { get; set; } = new HashSet<QGradeItem>();
        public virtual ICollection<QProgress> Progresses { get; set; } = new HashSet<QProgress>();
        public virtual ICollection<QGradeItemCompetency> Standards { get; set; } = new HashSet<QGradeItemCompetency>();
    }
}