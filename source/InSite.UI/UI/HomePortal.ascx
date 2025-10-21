<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomePortal.ascx.cs" Inherits="InSite.UI.HomePortal" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Controls/DashboardPrototype.ascx" TagName="DashboardPrototype" TagPrefix="uc" %>

<insite:UserLicenseCheck runat="server" />
<insite:UserPasswordCheck runat="server" />
<uc:AnnouncementToast runat="server" ID="AnnouncementToast" />
<uc:MaintenanceToast runat="server" ID="MaintenanceToast" ShowOnEachRequest="true" />

<uc:DashboardPrototype runat="server" ID="DashboardPrototype" Visible="false" />

<h1 runat="server" id="HomeTitle" visible="false"></h1>
<div class="mb-4" runat="server" id="HomeBody" visible="false"></div>

<insite:Nav runat="server" ID="Nav">
</insite:Nav>

<insite:Modal runat="server" ID="ChangePasswordWindow"
    Title="Change Password"
    EnableCloseButton="false"
    EnableStaticBackdrop="true"
    EnalbeCloseOnEscape="false"
    EnableAnimation="false" />
