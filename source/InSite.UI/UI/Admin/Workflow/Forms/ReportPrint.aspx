<%@ Page Language="C#" CodeBehind="ReportPrint.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.ReportPrint" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/ReportDistributionSearchResults.ascx" TagName="DistributionSearchResults" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <asp:Panel runat="server" ID="SearchResultsPanel" CssClass="section-panel section-results">
        <uc:DistributionSearchResults runat="server" ID="SearchResults" />
    </asp:Panel>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            body {
                margin: 0 0 0 0 !important;
            }
    
            nav.navbar,
            .cmd-selection-analysis,
            .table-text-analysis > tbody > tr > td:first-child,
            #subfooter.content {
                display: none;
            }
    
            #content.content {
                margin-top: 0 !important;
            }
    
            .chartjs-size-monitor-expand > div {
               position: fixed !important; 
            }
            .chartjs-size-monitor,
            .chartjs-size-monitor-shrink,
            .chartjs-size-monitor-expand,
            .chartjs-size-monitor-expand > div {
               position: fixed !important; 
            }
    
        </style>
    </insite:PageHeadContent>
    
    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                $('.section-results .row-distr-result').each(function (i) {
                    if (i > 0)
                        $(this).css('page-break-before', 'always');
                });
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
