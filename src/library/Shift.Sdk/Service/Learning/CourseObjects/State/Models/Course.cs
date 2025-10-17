using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Shift.Common;
namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Course : BaseObject
    {
        public Guid? Catalog { get; set; }
        public Guid? Framework { get; set; }
        public Guid Organization { get; set; }
                
        public string Hook { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Label { get; set; }
        public string Slug { get; set; }
        public string Style { get; set; }
        public string FlagColor { get; set; }
        public string FlagText { get; set; }
        public int Asset { get; set; }
        public int? Sequence { get; set; }
        public Guid? CompletionActivityIdentifier { get; set; }

        public bool AllowDiscussion { get; set; }
        public bool IsHidden { get; set; }

        [DefaultValue(true)]
        public bool AllowMultipleUnits { get; set; } = true;
        public bool IsProgressReportEnabled { get; set; }
        public int? OutlineWidth { get; set; }

        public Guid? CourseMessageStalledToLearner { get; set; }
        public Guid? CourseMessageStalledToAdministrator { get; set; }
        public Guid? CourseMessageCompletedToLearner { get; set; }
        public Guid? CourseMessageCompletedToAdministrator { get; set; }
        public int? SendMessageStalledAfterDays { get; set; }
        public int? SendMessageStalledMaxCount { get; set; }
        public DateTimeOffset? Closed { get; set; }

        public List<Unit> Units { get; set; }
        public List<PrivacyGroup> PrivacyGroups { get; set; }

        public Gradebook Gradebook { get; set; }

        public ContentContainer Content { get; set; }

        public Course(Guid identifier)
        {
            Identifier = identifier;
            Units = new List<Unit>();
            PrivacyGroups = new List<PrivacyGroup>();
        }

        public List<Module> GetModules()
        {
            return Units.SelectMany(u => u.Modules).ToList();
        }

        private List<Activity> _allActivities;
        public List<Activity> GetAllActivities()
        {
            if (_allActivities == null)
                _allActivities = Units.SelectMany(u => u.Modules.SelectMany(m => m.Activities)).ToList();
            return _allActivities;
        }

        private List<Activity> _allSupportedActivities;
        public List<Activity> GetAllSupportedActivities()
        {
            if (_allSupportedActivities == null)
                _allSupportedActivities = Units.SelectMany(u => u.Modules.SelectMany(m => m.GetSupportedActivites())).ToList();
            return _allSupportedActivities;
        }

        public Activity GetActivity(int index)
        {
            return GetAllActivities()[index];
        }

        public Activity GetActivity(Guid activity)
        {
            var activities = GetAllActivities();
            return activities.FirstOrDefault(x => x.Identifier == activity);
        }

        public int GetActivityIndex(Guid activity)
        {
            var activities = GetAllActivities();
            return 1 + activities.IndexOf(activities.FirstOrDefault(x => x.Identifier == activity));
        }

        public Module FindModule(Guid moduleId)
        {
            return GetModules().FirstOrDefault(x => x.Identifier == moduleId);
        }

        public Unit FindUnit(Guid unitId)
        {
            return Units.FirstOrDefault(x => x.Identifier == unitId);
        }
    }
}