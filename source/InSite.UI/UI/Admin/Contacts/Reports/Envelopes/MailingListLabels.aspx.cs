using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using Microsoft.Reporting.WebForms;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Contacts.Reports.Envelopes
{
    public partial class MailingListLabels : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var encoded = CurrentSessionState.ContactGroupEnvelopes;

            var json = StringHelper.DecodeBase64Url(encoded);

            var list = JsonConvert.DeserializeObject<List<MailingListLabel>>(json);

            var bytes = PrintToPdf(list);

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"mailing-list-labels.pdf\"");
            Response.AddHeader("Content-Length", bytes.Length.ToString());
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }

        public static byte[] PrintToPdf(List<MailingListLabel> list)
        {
            const string reportPath = "~/UI/Admin/Contacts/Reports/Envelopes/MailingListLabels.rdlc";

            const string deviceInfo = @"
<DeviceInfo>
    <OutputFormat>PDF</OutputFormat>
    <PageWidth>8.5in</PageWidth>
    <PageHeight>11in</PageHeight>
    <MarginTop>0in</MarginTop>
    <MarginLeft>0in</MarginLeft>
    <MarginRight>0in</MarginRight>
    <MarginBottom>0in</MarginBottom>
</DeviceInfo>";

            var report = new LocalReport { ReportPath = HttpContext.Current.Server.MapPath(reportPath) };

            var dataSource = new ReportDataSource("Labels", list);
            report.DataSources.Add(dataSource);

            return report.Render(
                "PDF",
                deviceInfo,
                out var mimeType,
                out var encoding,
                out var filenameExtension,
                out var streamids,
                out var warnings);
        }
    }
}