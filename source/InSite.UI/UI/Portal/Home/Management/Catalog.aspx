<%@ Page Language="C#" CodeBehind="Catalog.aspx.cs" Inherits="InSite.UI.Portal.Home.Management.Catalog" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CatalogDetail.ascx" TagName="CatalogDetail" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CartDetail.ascx" TagName="CartDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
     <uc:CatalogDetail runat="server" ID="CatalogDetail" />
</asp:Content>