using System;

namespace Shift.Sdk.UI
{
    public class AssignLearnerItem
    {
        public Guid UserIdentifier { get; set; }
        public string FullName { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public int? LifetimeMonths { get; set; }
        public string Action { get; set; }
        public bool IsRequired { get; set; }
        public bool IsPlanned { get; set; }

        public string ActionHtml
        {
            get
            {
                if (Action == "New")
                    return "<span class='badge bg-success'><i class='fa-solid fa-plus me-1'></i>New</span>";

                if (Action == "Update")
                    return "<span class='badge bg-info'><i class='fa-solid fa-edit me-1'></i>Update</span>";

                if (Action.StartsWith("Add"))
                    return $"<span class='badge bg-warning'><i class='fa-solid fa-plus me-1'></i>Assign</span>";

                if (Action.StartsWith("Make unplanned"))
                    return $"<span class='badge bg-danger'><i class='fa-solid fa-times me-1'></i>Unassign</span>";

                return $"<span class='badge bg-danger'><i class='fa-solid fa-trash-alt me-1'></i>Delete</span>";
            }
        }
    }
}