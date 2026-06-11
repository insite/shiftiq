using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Application.Courses.Read
{
    public class QActivity
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid? AssessmentFormIdentifier { get; set; }
        public Guid? GradeItemIdentifier { get; set; }
        public Guid ModuleIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PrerequisiteActivityIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? QuizIdentifier { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid? SourceIdentifier { get; set; }

        public string ActivityAuthorName { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityHook { get; set; }
        public string ActivityImage { get; set; }
        public string ActivityMode { get; set; }
        public string ActivityName { get; set; }
        public string ActivityPlatform { get; set; }
        public string ActivityType { get; set; }
        public string ActivityUrl { get; set; }
        public string ActivityUrlTarget { get; set; }
        public string ActivityUrlType { get; set; }
        public string PrerequisiteDeterminer { get; set; }
        public string RequirementCondition { get; set; }

        public string DoneButtonText { get; set; }
        public string DoneButtonInstructions { get; set; }
        public string DoneMarkedInstructions { get; set; }

        public int ActivityAsset { get; set; }
        public int? ActivityMinutes { get; set; }
        public int ActivitySequence { get; set; } = -1;

        public bool ActivityIsMultilingual { get; set; }
        public bool ActivityIsAdaptive { get; set; }
        public bool ActivityIsDispatch { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public DateTime? ActivityAuthorDate { get; set; }

        public string ActivitySlug
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ActivityCode))
                    return StringHelper.Sanitize(ActivityCode, '-', false);

                if (!string.IsNullOrWhiteSpace(ActivityName))
                    return StringHelper.Sanitize(ActivityName, '-', false);

                return ActivityIdentifier.ToString().ToLower();
            }
        }

        public virtual QGradeItem GradeItem { get; set; }
        public virtual QModule Module { get; set; }

        public virtual ICollection<QActivityCompetency> Competencies { get; set; } = new HashSet<QActivityCompetency>();

        public const string DefaultDoneButtonText = "Done";
        public const string DefaultDoneButtonInstructions = "To complete this activity you must mark it **Done** by clicking this button.";
        public const string DefaultDoneMarkedInstructions = "You marked this activity done on {0}.";
    }
}
