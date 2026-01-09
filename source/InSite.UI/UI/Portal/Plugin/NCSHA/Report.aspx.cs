using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Microsoft.Reporting.WebForms;

using Shift.Sdk.UI;

namespace InSite.UI.Variant.NCSHA.Reports
{
    public partial class Preview : PortalBasePage
    {
        private readonly string ErrorSessionExpired = "Your report session has expired. Please close this browser tab and sign in again.";

        private static readonly HashSet<string> VisibleExportOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "WORDOPENXML",  // Word
            "EXCELOPENXML", // Excel
            "PPTX",         // PowerPoint
            "PDF"           // PDF
        };

        private static readonly List<ExportFormatDescriptor> ExportFormats = new List<ExportFormatDescriptor>
        {
            new ExportFormatDescriptor("WORDOPENXML", "far fa-file-word", "Word Document", "docx"),
            new ExportFormatDescriptor("EXCELOPENXML", "far fa-file-excel", "Excel Worksheet", "xlsx"),
            new ExportFormatDescriptor("PPTX", "far fa-file-powerpoint", "PowerPoint Presentation", "pptx"),
            new ExportFormatDescriptor("PDF", "far fa-file-pdf", "PDF Document", "pdf")
        };

        private NcshaReport SelectedReport => NcshaReportHelper.GetReport(ReportTable.SelectedValue);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportTable.AutoPostBack = true;
            ReportTable.SelectedIndexChanged += (x, y) => BindModelToControls();

            ReportYear.AutoPostBack = true;
            ReportYear.DataSource = BindYearData();
            ReportYear.DataBind();
            ReportYear.SelectedIndexChanged += (x, y) => BindModelToControls();

            ExportButtonRepeater.ItemCommand += ExportButtonClicked;

            Viewer.PreRender += Viewer_PreRender;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            try
            {
                ReportTable.DataValueField = "Code";
                ReportTable.DataTextField = "Code";
                ReportTable.DataSource = NcshaReportHelper.GetReports();
                ReportTable.DataBind();

                if (Request.QueryString["report"] != null)
                {
                    var item = ReportTable.Items.FindByValue(Request.QueryString["report"]);
                    if (item != null)
                        item.Selected = true;
                }

                ReportViewerSection.Visible = false;
                ExportButtonRepeater.Visible = false;

                BindModelToControls();
                MultiView.SetActiveView(NormalView);
            }
            catch (Exception ex)
            {
                ErrorOccurred(ex.Message);
            }
        }

        private List<ListItem> BindYearData()
        {
            var showLastYearToEveryone = Organization.Toolkits.NCSHA?.ShowLastYearToEveryone ?? false;

            var lastYear = showLastYearToEveryone || CurrentSessionState.Identity.Person.IsAdministrator
                ? DateTime.Now.Year - 1
                : DateTime.Now.Year - 2;

            var results = new List<ListItem>();
            for (var i = lastYear; i >= 2005; i--)
                results.Add(new ListItem() { Text = i.ToString(), Value = i.ToString() });

            if (results.Count > 0)
                results.FirstOrDefault(x => x.Value == "2022").Selected = true;

            return results;
        }

        private void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);
            PageHelper.HideSideContent(this);
            PortalMaster.SidebarVisible(false);

            var environment = ServiceLocator.AppSettings.Environment.Name;
            var ssrs = ServiceLocator.AppSettings.Integration.SSRS;

            Viewer.Reset();

            Viewer.ServerReport.ReportServerUrl = new Uri(ssrs.Url);
            Viewer.ServerReport.ReportPath = $"/{environment}/NCSHA/{SelectedReport.Code}";
            Viewer.ServerReport.ReportServerCredentials = new ReportServerCredentials();

            var parameters = new List<ReportParameter>
            {
                new ReportParameter("Year", ReportYear.SelectedValue.ToString())
            };
            Viewer.ServerReport.SetParameters(parameters);
            Viewer.ServerReport.Refresh();

            ReportViewerSection.Visible = true;

            ExportButtonRepeater.Visible = true;
            ExportButtonRepeater.DataSource = ExportFormats;
            ExportButtonRepeater.DataBind();
        }

        private void Viewer_PreRender(object sender, EventArgs e)
        {
            var type = typeof(RenderingExtension);
            var field = type.GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var extension in Viewer.ServerReport.ListRenderingExtensions())
                field.SetValue(extension, VisibleExportOptions.Contains(extension.Name));
        }

        private void ExportButtonClicked(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Export")
            {
                try
                {
                    var id = (string)e.CommandArgument;
                    var format = ExportFormats.FirstOrDefault(x => x.Id == id);
                    if (format == null)
                        ErrorOccurred("Unexpected export file format ID: " + id);

                    if (SelectedReport == null)
                        return;

                    var data = Viewer.ServerReport.Render(format.Id, null, out var mimeType, out var encoding, out var fileNameExtension, out var streams, out var warnings);
                    FileDownloader.DownloadFile(Page.Response, $"{SelectedReport.Code}.{fileNameExtension}", s => s.Write(data, 0, data.Length), data.Length, mimeType);
                }
                catch (MissingReportSourceException)
                {
                    ErrorOccurred(ErrorSessionExpired);
                }
            }
        }

        private void ErrorOccurred(string error)
        {
            ErrorPanel.InnerHtml = error;
            MultiView.SetActiveView(ErrorView);
        }
    }
}