using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Module : BaseObject
    {
        private static string[] SupportedTypes = new[] { "Lesson", "Assessment", "Survey", "Document", "Link", "Video", "Quiz" };

        public int Asset { get; set; }

        public Unit Unit { get; set; }
        public BaseObject GradeItem { get; set; }

        public List<Activity> Activities { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        public List<BaseObject> PrivacyGroups { get; set; }

        public ContentContainer Content { get; set; }

        public bool HasPrerequisites => Prerequisites.Count > 0;
        public bool IsAdaptive { get; set; }

        public DeterminerType PrerequisiteDeterminer { get; set; }

        public string FullCode
        {
            get
            {
                return !string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Unit?.Code)
                    ? $"{Unit.Code}.{Code}"
                    : Code;
            }
        }

        public Module()
        {
            Activities = new List<Activity>();
            Prerequisites = new List<Prerequisite>();
            PrivacyGroups = new List<BaseObject>();
        }

        public List<Activity> GetSupportedActivites()
        {
            return Activities.Where(x => SupportedTypes.Contains(x.Type)).ToList();
        }
    }
}
