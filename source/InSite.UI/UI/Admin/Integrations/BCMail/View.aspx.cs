using System;
using System.Text.RegularExpressions;

using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.Persistence.Integration.BCMail;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Integrations.BCMail
{
    public partial class View : AdminBasePage
    {
        private Guid? EventIdentifier => Guid.TryParse(Request.QueryString["event"], out var guid) ? guid : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            var exam = GetExamEvent();
            CheckCurrentStatus(exam);

            var requests = GetRequestLog(exam);
            BindModelToControls(exam, requests);

            base.OnLoad(e);
        }

        private QEvent GetExamEvent()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value)
                : null;

            if (@event == null)
                HttpResponseHelper.Redirect("/ui/admin/integrations/bcmail/search", true);

            return @event;
        }

        private void CheckCurrentStatus(QEvent exam)
        {
            var track = new TrackDistribution(exam.EventIdentifier, exam.ExamType == EventExamType.Test.Value);
            ServiceLocator.SendCommand(track);
        }

        private ExamDistributionRequest[] GetRequestLog(QEvent exam)
        {
            var requests = ExamDistributionRequestSearch.GetRequests(exam.DistributionCode);
            foreach (var request in requests)
                if (request.JobErrors != null && IsHtml(request.JobErrors))
                    request.JobErrors = $"<textarea style='width:100%;height:200px'>{request.JobErrors}</textarea>";
            return requests;
        }

        public static bool IsHtml(string text)
        {
            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            return tagRegex.IsMatch(text);
        }

        private void BindModelToControls(QEvent exam, ExamDistributionRequest[] requests)
        {
            PageHelper.AutoBindHeader(this);

            DistributionStatusTime.InnerHtml = DateTimeOffset.UtcNow.Format(User.TimeZone, true);
            DistributionStatus.InnerHtml = exam.DistributionStatus;
            DistributionErrors.InnerHtml = exam.DistributionErrors;

            HistoryRepeater.DataSource = requests;
            HistoryRepeater.DataBind();
        }

        protected string Localize(object o)
        {
            return TimeZones.Format((DateTimeOffset)o, User.TimeZone, true);
        }
    }
}
