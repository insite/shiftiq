using System;
using System.Collections.Generic;

using Shift.Common;

using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Activity : BaseObject
    {
        public string Hook { get; set; }

        public Link Link { get; set; }

        public string ContentDeliveryPlatform { get; set; }

        public int Asset { get; set; }
        public int? DurationMinutes { get; set; }

        public bool IsAdaptive { get; set; }
        public bool IsMultilingual { get; set; }

        public DeterminerType PrerequisiteDeterminer { get; set; }
        public RequirementType Requirement { get; set; }

        public string DoneButtonText { get; set; }
        public string DoneButtonInstructions { get; set; }
        public string DoneMarkedInstructions { get; set; }

        public Module Module { get; set; }
        public ActivityAssessment Assessment { get; set; }
        public GradeItem GradeItem { get; set; }
        public ActivitySurvey Survey { get; set; }
        public Guid? QuizIdentifier { get; set; }
        
        public ContentContainer Content { get; set; }

        public List<Prerequisite> Prerequisites { get; set; }
        public List<PrivacyGroup> PrivacyGroups { get; set; }

        public bool HasPrerequisites => Prerequisites.Count > 0;

        public string FullCode
        {
            get
            {
                return !string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Module?.FullCode)
                    ? $"{Module.FullCode}.{Code}"
                    : Code;
            }
        }

        public Activity()
        {
            Prerequisites = new List<Prerequisite>();
            PrivacyGroups = new List<PrivacyGroup>();
        }
    }
}