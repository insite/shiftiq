<%@ Page Language="C#" CodeBehind="TakerReport.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.TakerReport" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/TakerReportCriteria.ascx" TagName="ReportCriteria" TagPrefix="uc" %>
<%@ Register Src="./Controls/TakerReportOptions.ascx" TagName="ReportOptions" TagPrefix="uc" %>
<%@ Register Src="./Controls/TakerReportGrid.ascx" TagName="ReportGrid" TagPrefix="uc" %>

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
                        <div class="w-50">
                            <uc:ReportOptions runat="server" ID="ReportOptions" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div runat="server" id="NoAttemptPanel" class="card border-0 shadow-lg" visible="false">
            <div class="card-body">
                <insite:Alert runat="server" Icon="fas fa-exclamation-triangle" Indicator="Warning" Text="No attempts related to 'Field of Practice' framework found for these criteria" />
            </div>
        </div>

        <div runat="server" id="AttemptPanel" class="card border-0 shadow-lg" visible="false">
            <div class="card-body">
                <%-- See TakerReportGrid JS for code that enables/disables buttons --%>
                <div class="mb-3 report-buttons">
                    <insite:DownloadButton runat="server" ID="SaveToCaseButton" Text="Save to Case" ButtonStyle="Success" />
                </div>

                <div style="height:0; overflow:hidden;">
                    <asp:HiddenField runat="server" ID="ReportType" />
                    <insite:FindCase runat="server" ID="SelectedCase" ModalHeader="Save to Case" AllowClear="false" />
                </div>

                <uc:ReportGrid runat="server" ID="ReportGrid" />
            </div>
        </div>

    </section>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                document.getElementById("<%= SaveToCaseButton.ClientID %>").addEventListener("click", saveReport);

                function saveReport(e) {
                    e.preventDefault();

                    setTimeout(function () {
                        document.getElementById('<%= SelectedCase.ClientID %>').show();
                    }, 0);
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>