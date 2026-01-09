<%@ Page Language="C#" CodeBehind="Catalog.aspx.cs" Inherits="InSite.UI.Portal.Billing.Catalog" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../../Portal/Home/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CatalogDetail.ascx" TagName="CatalogDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:CatalogDetail runat="server" ID="CatalogDetail" />
</asp:Content>
