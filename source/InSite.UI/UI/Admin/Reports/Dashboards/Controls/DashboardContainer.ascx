<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardContainer.ascx.cs" Inherits="InSite.UI.Admin.Reports.Dashboards.DashboardContainer" %>
<%@ Register Src="./DashboardGrid.ascx" TagName="DashboardGrid" TagPrefix="uc" %>

<style type="text/css">
    .dashboard-selector td { padding-right: 20px; }
</style>

<div class="dashboard-selector mb-4">
    <asp:RadioButtonList runat="server" ID="DashboardList" AutoPostBack="true" RepeatDirection="Horizontal" />
</div>

<insite:Alert runat="server" ID="DashboardStatus" />
<asp:Panel runat="server" ID="DashboardPanels" CssClass="row" />