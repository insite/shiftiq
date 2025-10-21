using System;
using System.Text;

using Humanizer;
using Humanizer.Localisation;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    static class AttemptReportHelper
    {
        public static string FormatScore(
            DateTimeOffset? attemptGraded,
            DateTimeOffset? attemptSubmitted,
            bool attemptIsPassing,
            decimal? attemptScore,
            decimal? attemptPoints,
            decimal? formPoints
        )
        {
            if (!attemptGraded.HasValue)
            {
                return attemptSubmitted.HasValue
                    ? "<span class='badge bg-warning'>Pending</span>"
                    : string.Empty;
            }

            var statusHtml = attemptIsPassing
                ? "<span class='badge bg-success'>Pass</span>"
                : "<span class='badge bg-danger'>Fail</span>";

            return $"<div>{attemptScore:p0}</div>"
                + $"<div class='form-text'>{attemptPoints} / {formPoints}</div>"
                + statusHtml;
        }

        public static string FormatTime(
            DateTimeOffset? attemptStarted,
            DateTimeOffset? attemptGraded,
            DateTimeOffset? attemptImported,
            decimal? attemptDuration
        )
        {
            var user = CurrentSessionState.Identity.User;

            var html = new StringBuilder();

            if (attemptStarted.HasValue)
                html.Append("<div>Started " + attemptStarted.Value.Format(user.TimeZone, true) + "</div>");

            if (attemptGraded.HasValue)
                html.Append("<div>Completed " + attemptGraded.Value.Format(user.TimeZone, true) + "</div>");

            if (attemptImported.HasValue)
                html.Append("<div class='form-text'>Imported " + attemptImported.Value.Format(user.TimeZone, true) + "</div>");

            if (attemptDuration.HasValue)
                html.Append("<div class='form-text'>Time Taken = " + ((double)attemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second) + "</div>");

            return html.ToString();
        }

        public static string GetFormAsset(
            int? formAssetVersion,
            DateTimeOffset? formFirstPublished,
            int? formAsset,
            DateTimeOffset? attemptStarted
        )
        {
            if (formAssetVersion == null)
                return string.Empty;

            var assetVersion = formAssetVersion;
            if (formFirstPublished.HasValue && attemptStarted.HasValue && attemptStarted.Value < formFirstPublished.Value)
                assetVersion = 0;

            return $"{formAsset}.{assetVersion}";
        }
    }
}