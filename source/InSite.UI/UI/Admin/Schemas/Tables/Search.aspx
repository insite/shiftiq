<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Settings.Tables.Search" %>

<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./Controls/SearchCriteria.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./Controls/SearchResults.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

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
    </insite:Nav>

</asp:Content>
