<%@ Control Language="C#" CodeBehind="DashboardChart.ascx.cs" Inherits="InSite.UI.Admin.Reports.Dashboards.DashboardChart" %>

<div class="dashboard-chart">

    <div class="mb-1">
        <asp:LinkButton runat="server" ID="DownloadXlsx" Visible="false"><i class="fas fa-file-excel me-1" title="Download Excel"></i></asp:LinkButton>
        <span runat="server" id="DownloadCount" class="text-body-secondary me-1"></span>
    </div>

    <insite:Alert runat="server" ID="ControlStatus" />

    <div>
        <insite:DynamicControl runat="server" ID="Container" />
    </div>

    <div runat="server" id="NoDataMessage" visible="false" class="text-body-secondary text-center py-5">
        No data
    </div>

</div>
