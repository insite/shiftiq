using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Application.Registrations.Read;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI;
using InSite.Web.Helpers;

using Shift.Constant;

namespace InSite.Admin.Events.Candidates.Controls
{
    public partial class ScrapsPrint : UserControl
    {
        private class CandidateInfo
        {
            public string Name { get; }
            public string Code { get; }
            public string Password { get; }
            public bool IsLast { get; set; }

            public CandidateInfo(QRegistration entity)
            {
                Name = entity.Candidate.UserFullName;
                Code = entity.Candidate.PersonCode;
                Password = entity.RegistrationPassword;
                IsLast = false;
            }
        }

        protected string Logo { get; set; }

        private void LoadData(OrganizationState organization, CandidateInfo[] candidates)
        {
            var styleUrl = PathHelper.ToAbsoluteUrl("/library/fonts/font-awesome/7.1.0/css/all.min.css");
            StyleLink.Text = $"<link href='{styleUrl}' rel='stylesheet' type='text/css' media='all' />";

            Logo = PathHelper.ToAbsoluteUrl(organization.PlatformCustomization?.PlatformUrl?.Logo);

            candidates[candidates.Length - 1].IsLast = true;

            CandidateRepeater.DataSource = candidates;
            CandidateRepeater.DataBind();
        }

        public static byte[] RenderPdf(Guid eventId)
        {
            var @event = ServiceLocator.EventSearch.GetEvent(eventId);
            if (@event == null)
                return null;

            var candidates = ServiceLocator.RegistrationSearch
                .GetRegistrationsByEvent(eventId, null, null, null, false, false, true)
                .Where(x => !string.Equals(x.ApprovalStatus, "Not Eligible", StringComparison.OrdinalIgnoreCase))
                .Select(x => new CandidateInfo(x))
                .ToArray();

            if (candidates.Length == 0)
                return null;

            var organization = OrganizationSearch.Select(@event.OrganizationIdentifier);

            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (ScrapsPrint)page.LoadControl("~/UI/Admin/Events/Candidates/Controls/ScrapsPrint.ascx");

                report.LoadData(organization, candidates);

                var html = new StringBuilder();
                using (var writer = new StringWriter(html))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    MarginTop = 10f,
                    MarginBottom = 0,
                    PageSize = PageSizeType.Letter
                };

                return HtmlConverter.HtmlToPdf(html.ToString(), settings);
            }
        }
    }
}