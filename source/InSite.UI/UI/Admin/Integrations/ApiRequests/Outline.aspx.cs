using System;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Integration;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Reports.ApiRequests.Forms
{
    public partial class Read : ViewerController
    {
        private Guid RequestKey => Guid.TryParse(Request["request"], out var result) ? result : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();
            }
        }

        private void Open()
        {
            var request = ApiRequestSearch.Select(RequestKey);
            if (request == null)
                RedirectToSearch();

            Bind(request);
        }

        private void Bind(ApiRequest request)
        {
            var uri = new Uri(request.RequestUri);

            PageHelper.AutoBindHeader(this, null, request.RequestMethod + " " + uri.AbsolutePath);

            RequestUrl.Text = request.RequestUri;
            RequestHeaders.Text = request.RequestHeaders;
            RequestStarted.Text = GetLocalTime(request.RequestStarted);
            ValidationStatus.Text = request.ValidationStatus;
            ValidationErrorsField.Visible = !string.IsNullOrEmpty(request.ValidationErrors);
            ValidationErrors.Text = request.ValidationErrors;
            RequestStatus.Text = request.RequestStatus;

            if (!string.IsNullOrEmpty(request.RequestContentData))
                InputData.InnerHtml = request.RequestContentData.Replace("<", "&lt;").Replace(">", "&gt;");

            ResponseStatus.Text = $"{request.ResponseStatusNumber} {request.ResponseStatusName}";
            ResponseTime.Text = string.Format("{0:n0} milliseconds", request.ResponseTime);

            ResponseCompleted.Text = request.ResponseCompleted.HasValue
                ? GetLocalTime(request.ResponseCompleted.Value) + " (" + request.ResponseCompleted.Value.Humanize() + ")"
                : "Not Completed";

            ExecutionErrorsField.Visible = !string.IsNullOrEmpty(request.ExecutionErrors);
            ExecutionErrors.Text = request.ExecutionErrors;

            if (!string.IsNullOrEmpty(request.ResponseContentData))
                OutputData.InnerHtml = request.ResponseContentData.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private static string GetLocalTime(DateTimeOffset requestStarted)
        {
            return requestStarted.Format(User.TimeZone, true, false, true);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/integrations/api-requests/search", true);

    }
}
