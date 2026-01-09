using System.Web.UI.HtmlControls;

using Humanizer;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.Cmds
{
    public static class TimeSensitivityHelper
    {
        public static void SetTimeSensitiveIcon(bool isTimeSensitive, string validForUnit, int? validForCount, Icon icon)
        {
            if (icon == null)
                return;

            icon.Visible = isTimeSensitive;

            if (isTimeSensitive)
                icon.ToolTip = GetTimeSensitiveText(validForUnit, validForCount);
        }

        public static void SetTimeSensitiveImage(bool isTimeSensitive, string validForUnit, int? validForCount, HtmlGenericControl span)
        {
            if (span == null)
                return;

            span.Visible = isTimeSensitive;

            if (isTimeSensitive)
                span.InnerHtml = @"<i class='fa-solid fa-alarm-clock'></i> " + GetTimeSensitiveText(validForUnit, validForCount);
        }

        public static string GetTimeSensitiveText(string validForUnit, int? validForCount)
        {
            return validForUnit.IsNotEmpty() && validForCount.HasValue
                ? "Valid for " + validForUnit.ToQuantity(validForCount.Value)
                : "Time-Sensitive";
        }

        public static string GetLabelsHtml(bool? isRequired, bool? isPlanned, int lifetime)
        {
            var html = string.Empty;

            if (isRequired.HasValue)
                html += isRequired.Value
                    ? "<span class='badge bg-info fs-xs me-1'><i class='fa-solid fa-asterisk me-1'></i>Required</span>"
                    : "<span class='badge bg-secondary fs-xs me-1'><i class='fa-regular fa-circle me-1'></i>Optional</span>";

            if (isPlanned.HasValue)
                html += !isPlanned.Value
                    ? $"<span class='badge bg-danger fs-xs me-1'><i class='fa-solid fa-do-not-enter me-1'></i>Not in training plan</span>"
                    : "";

            html += lifetime > 0
                ? $"<span class='badge bg-primary fs-xs me-1'><i class='fa-solid fa-alarm-clock me-1'></i>Valid for {Shift.Common.Humanizer.ToQuantity(lifetime, "month")}</span>"
                : "";

            return html;
        }
    }
}
