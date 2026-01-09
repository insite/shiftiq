using System.ComponentModel;

namespace Shift.Constant.Enumerations
{
    public enum PersonCaseType
    {
        None = 0,
        [Description("Open Case Assignee")]
        OpenCaseAssignee,

        [Description("Closed Case Assignee")]
        ClosedCaseAssignee,

        [Description("Open Case Administrator")]
        OpenCaseAdministrator,

        [Description("Closed Case Administrator")]
        ClosedCaseAdministrator
    }
}