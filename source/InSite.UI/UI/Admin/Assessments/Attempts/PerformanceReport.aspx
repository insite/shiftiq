<%@ Page Language="C#" CodeBehind="PerformanceReport.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.PerformanceReport" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/PerformanceReportCriteria.ascx" TagName="ReportCriteria" TagPrefix="uc" %>
<%@ Register Src="./Controls/PerformanceReportOptions.ascx" TagName="ReportOptions" TagPrefix="uc" %>
<%@ Register Src="./Controls/PerformanceReportGrid.ascx" TagName="PerformanceReportGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />
    <insite:Alert runat="server" ID="ErrorAlert" />

    <section>
        <insite:Alert runat="server" ID="FileUploadedAlert" />

        <div class="row mb-3">
            <div class="col-lg-4">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:ReportCriteria runat="server" ID="ReportCriteria" />

                        <insite:SearchButton runat="server" ID="SearchButton" ValidationGroup="Report" CausesValidation="true" />
                    </div>
                </div>
            </div>
            <div class="col-lg-8">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:ReportOptions runat="server" ID="ReportOptions" />
                    </div>
                </div>
            </div>
        </div>

        <div runat="server" id="NoAttemptPanel" class="card border-0 shadow-lg" visible="false">
            <div class="card-body">
                <insite:Alert runat="server" Icon="fas fa-exclamation-triangle" Indicator="Warning" Text="No attempts found for these criteria" />
            </div>
        </div>

        <div runat="server" id="AttemptPanel" class="card border-0 shadow-lg" visible="false">
            <div class="card-body">
                <%-- See PerformanceReportGrid JS for code that enables/disables buttons --%>
                <div class="mb-3 report-buttons">
                    <insite:DownloadButton runat="server" ID="DownloadAssessmentAuditAlternateButton" Text="Assessment Audit (HCA Framework)" ValidationGroup="Report" />
                    <insite:DownloadButton runat="server" ID="DownloadAssessmentAuditButton" Text="Assessment Audit" ValidationGroup="Report" />
                    <insite:DownloadButton runat="server" ID="DownloadAlternateScoresButton" Text="Item Scores (Alternate Framework)" ValidationGroup="Report" />
                    <insite:DownloadButton runat="server" ID="DownloadScoresButton" Text="Item Scores" ValidationGroup="Report" />
                    <insite:DownloadButton runat="server" ID="SaveToCaseAlternateButton" Text="Save to Case (Alternate Framework)" />
                    <insite:DownloadButton runat="server" ID="SaveToCaseButton" Text="Save to Case" ButtonStyle="Success" />
                </div>

                <div style="height:0; overflow:hidden;">
                    <asp:HiddenField runat="server" ID="ReportType" />
                    <insite:FindCase runat="server" ID="SelectedCase" ModalHeader="Save to Case" AllowClear="false" />
                </div>

                <uc:PerformanceReportGrid runat="server" ID="ReportGrid" />
            </div>
        </div>

    </section>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                document.getElementById("<%= SaveToCaseAlternateButton.ClientID %>").addEventListener("click", e => saveReport(e, "Alternate"));
                document.getElementById("<%= SaveToCaseButton.ClientID %>").addEventListener("click", e => saveReport(e));

                function saveReport(e, reportType) {
                    e.preventDefault();

                    setTimeout(function () {
                        document.getElementById('<%= ReportType.ClientID %>').value = reportType;
                        document.getElementById('<%= SelectedCase.ClientID %>').show();
                    }, 0);
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>