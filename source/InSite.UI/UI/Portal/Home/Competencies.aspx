<%@ Page Language="C#" CodeBehind="Competencies.aspx.cs" Inherits="InSite.UI.Portal.Home.Competencies" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="./Controls/CmdsCompetencyDashboard.ascx" TagName="CmdsCompetencyDashboard" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:CmdsCompetencyDashboard runat="server" ID="CmdsCompetencyDashboard" />
</asp:Content>