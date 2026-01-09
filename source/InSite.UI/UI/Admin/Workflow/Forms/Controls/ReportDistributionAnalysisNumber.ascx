<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDistributionAnalysisNumber.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDistributionAnalysisNumber" %>

<table class="table">
    <thead>
        <tr>
            <th class="text-end">Count</th>
            <th class="text-end">Min</th>
            <th class="text-end">Max</th>
            <th class="text-end">Sum</th>
            <th class="text-end">Avg</th>
            <th class="text-end">Stdev</th>
            <th class="text-end">Var</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralCount" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralMinimum" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralMaximum" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralSum" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralAverage" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralStandardDeviation" /></td>
            <td class="text-end"><asp:Literal runat="server" ID="LiteralVariance" /></td>
        </tr>
    </tbody>
</table>