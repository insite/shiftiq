<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Events.Registrations.Search" %>
<%@ Register Src="./Classes/Controls/SearchCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="./Classes/Controls/SearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="./Classes/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Messages/Messages/Controls/SendEmail.ascx" TagName="SendEmail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="MessagesTab" Icon="fas fa-envelopes-bulk" Title="Build Message">
            <uc:SendEmail runat="server" ID="SendEmail" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>