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
            icon.Visible = isTimeSensitive;

            if (isTimeSensitive)
                icon.ToolTip = GetTimeSensitiveText(validForUnit, validForCount);
        }

        public static void SetTimeSensitiveImage(bool isTimeSensitive, string validForUnit, int? validForCount, HtmlGenericControl span)
        {
            span.Visible = isTimeSensitive;

            if (isTimeSensitive)
                span.InnerHtml = @"<i class='far fa-clock'></i> " + GetTimeSensitiveText(validForUnit, validForCount);
        }

        public static string GetTimeSensitiveText(string validForUnit, int? validForCount)
        {
            return validForUnit.IsNotEmpty() && validForCount.HasValue
                ? "Valid for " + validForUnit.ToQuantity(validForCount.Value)
                : "Time-Sensitive";
        }
    }
}
