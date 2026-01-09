<%@ Page Language="C#" CodeBehind="Report.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Report" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDistributionSearchCriteria.ascx" TagName="DistributionSearchCriteria" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDistributionSearchResults.ascx" TagName="DistributionSearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportCorrelationSearchCriteria.ascx" TagName="CorrelationSearchCriteria" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportCorrelationSearchResults.ascx" TagName="CorrelationSearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDownloadSection.ascx" TagName="ReportDownloadSection" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDateAnalysisChart.ascx" TagName="ReportDateAnalysisChart" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportInvitationAnalysis.ascx" TagName="ReportInvitationAnalysis" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDuplicateGrid.ascx" TagName="ReportDuplicateGrid" TagPrefix="uc" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportSubmissionSearchResults.ascx" TagName="ResponseSearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportSubmissionSearchCriteria.ascx" TagName="ResponseSearchCriteria" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">
        .toggle-panel {
            border: 1px solid #cccccc;
            border-radius: 6px;
            margin: 10px 0 30px 15px;
            padding: 15px;
            width: 550px;
        }

        .toggle-panel .toggle-buttons {
            margin-top: 15px;
        }

        .toggle-panel .section .field .title {
            width: 140px;
        }

        .ui-effects-wrapper .toggle-panel {
            width: 550px !important;
            height: auto !important;
        }

    </style>
</insite:PageHeadContent>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="DistributionReport" />
    <insite:ValidationSummary runat="server" ValidationGroup="CorrelationXAxis" />
    <insite:ValidationSummary runat="server" ValidationGroup="CorrelationYAxis" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Submissions">
            <uc:ResponseSearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:ResponseSearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="TimeSeriesButton" Title="Time Series" Icon="far fa-chart-bar" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mt-4 mb-3">Time Series</h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:ReportDateAnalysisChart runat="server" ID="DateAnalysisChart" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="FrequencyDistributionButton" Title="Frequency Distribution" Icon="fas fa-tachometer-alt me-2" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mt-4 mb-3">Frequency Distribution Analysis</h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <h3>Criteria</h3>
                                <uc:DistributionSearchCriteria runat="server" ID="DistributionCriteria" />
                            </div>
                        </div>
                    </div>
                </div>
                <asp:Panel ID="DistributionResultsPanel" runat="server" Visible="false">
                    <div class="row mt-3">
                        <div class="col-lg-12">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">
                                    <h3>Results</h3>
                                    <div class="text-end">
                                        <div class="row" style="padding-bottom: 18px;">
                                            <div class="col-md-12">
                                                <insite:Button runat="server" ID="PrintDistributionReport" ButtonStyle="Default" Text="Print" Icon="far fa-print" />
                                                <insite:Button runat="server"
                                                    ID="PrintDistributionReport2"
                                                    ButtonStyle="Default"
                                                    Text="<span>Print PDF</span>"
                                                    Icon="far fa-print"
                                                    Visible="false"
                                                />
                                            </div>
                                        </div>
                                    </div>

                                    <uc:DistributionSearchResults runat="server" ID="DistributionResults" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CorrelationAnalysisButton" Title="Correlation Analysis" Icon="fas fa-comment me-2" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mt-4 mb-3">Correlation Analysis</h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <h3>Criteria</h3>
                                <uc:CorrelationSearchCriteria runat="server" ID="CorrelationCriteria" />
                            </div>
                        </div>
                    </div>
                </div>
                <asp:Panel ID="CorrelationResultsPanel" runat="server" Visible="false">
                    <div class="row mt-3">
                        <div class="col-lg-12">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">
                                    <h3>Results</h3>
                                    <uc:CorrelationSearchResults runat="server" ID="CorrelationResults" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="InvitationAnalyticsButton" Title="Invitation Analytics" Icon="fas fa-ruler-triangle me-2" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mt-4 mb-3">Invitation Analytics</h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:ReportInvitationAnalysis runat="server" ID="SurveyInvitationAnalytics" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DownloadButton" Title="Download Data Set" Icon="fas fa-download me-2" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mt-4 mb-3">Download Data Set</h2>
                <div class="row">
                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:ReportDownloadSection runat="server" ID="DownloadSection" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script>
            "use strict";

            (() => {
                const reportUrl = '<%= GetReportPrintUrl() %>';
                const btn = document.getElementById('<%= PrintDistributionReport2.ClientID %>');

                if (!btn) {
                    return;
                }

                const btnTitle = btn.querySelector('span');

                btn.addEventListener('click', (e) => {
                    e.preventDefault();

                    addHtml2PdfScript();

                    const frame = addFrame();
                    frame.src = reportUrl;

                    btnTitle.innerHTML = 'Generating PDF...';
                    btn.disabled = true;
                    btn.classList.add('disabled');
                });

                function addFrame() {
                    let frame = document.getElementById('ReportFrame');
                    if (frame) {
                        return frame;
                    }

                    frame = document.createElement('iframe');
                    frame.id = 'ReportFrame';
                    frame.width = 1200;
                    frame.height = 0;

                    frame.addEventListener('load', () => {
                        const iframeDocument = frame.contentDocument || frame.contentWindow.document;

                        addDocumentStyle(iframeDocument);

                        printPdf(frame, iframeDocument);
                    });

                    document.body.appendChild(frame);

                    return frame;
                }

                function printPdf(frame, iframeDocument) {
                    const opt = {
                        margin: 0,
                        filename: 'form-report.pdf',
                        image: { type: 'jpeg', quality: 0.95 },
                        html2canvas: {
                            y: 0,
                            scrollY: 0,
                            windowWidth: frame.width,
                            windowHeight: 2000,
                            scale: 2,
                            useCORS: true
                        },
                        jsPDF: { unit: 'mm', format: 'a4', orientation: 'p' }
                    };

                    setTimeout(() => {
                        html2pdf()
                            .set(opt)
                            .from(iframeDocument.body)
                            .toPdf()
                            .get('pdf')
                            .then((pdf) => {
                                const totalPages = pdf.internal.getNumberOfPages();
                                for (let i = 1; i <= totalPages; i++) {
                                    pdf.setPage(i);
                                    pdf.setFontSize(10);
                                    pdf.text(`${i} / ${totalPages}`, (pdf.internal.pageSize.getWidth() - 14), (pdf.internal.pageSize.getHeight() - 5));
                                }
                            })
                            .save()
                            .then(() => {
                                btnTitle.innerHTML = 'Print PDF';
                                btn.disabled = false;
                                btn.classList.remove('disabled');
                            });
                    }, 0);
                }

                function addDocumentStyle(iframeDocument) {
                    iframeDocument.body.style.fontSize = 'smaller';

                    iframeDocument.querySelectorAll('.offcanvas-enabled-start')
                        .forEach(x => {
                            x.style.padding = '0px';
                            x.style.margin = '0px';
                            x.style.width = '100%';
                        });

                    iframeDocument.querySelectorAll('.offcanvas-enabled-start > .col-xxl-9')
                        .forEach(x => {
                            x.style.width = '100%';
                        });

                    iframeDocument.querySelectorAll('.row-distr-result')
                        .forEach(x => {
                            x.style.marginTop = '40px';
                        });
                }

                function addHtml2PdfScript() {
                    if (document.getElementById('Html2PdfScript')) {
                        return;
                    }

                    const script = document.createElement('script');
                    script.id = 'Html2PdfScript';
                    script.type = 'application/javascript';
                    script.src = '/UI/Layout/common/parts/plugins/html2pdf/html2pdf.bundle.min.js';

                    document.head.appendChild(script);
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
