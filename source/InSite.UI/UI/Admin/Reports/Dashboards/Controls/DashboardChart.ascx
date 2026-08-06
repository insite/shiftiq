<%@ Control Language="C#" CodeBehind="DashboardChart.ascx.cs" Inherits="InSite.UI.Admin.Reports.Dashboards.DashboardChart" %>

<div class="dashboard-chart">

    <insite:DynamicControl runat="server" ID="Container" />

    <div runat="server" id="NoDataMessage" visible="false" class="text-body-secondary text-center py-5">
        No data
    </div>

</div>
