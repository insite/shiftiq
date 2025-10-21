<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeAdmin.ascx.cs" Inherits="InSite.UI.HomeAdmin" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/HomeAdminPanel.ascx" TagName="HomeAdminPanel" TagPrefix="uc" %>
<%@ Register Src="~/UI/AdminDashboardPrototype.ascx" TagName="AdminDashboardPrototype" TagPrefix="uc" %>

<insite:UserLicenseCheck runat="server" />
<insite:UserPasswordCheck runat="server" />
<uc:AnnouncementToast runat="server" ID="AnnouncementToast" />
<uc:MaintenanceToast runat="server" ID="MaintenanceToast" />
<insite:Toast runat="server" ID="WarningToast" Icon="fas fa-brake-warning" Indicator="Warning" />

<uc:AdminDashboardPrototype runat="server" ID="AdminDashboardPrototype" Visible="false" />

<uc:HomeAdminPanel runat="server" />
