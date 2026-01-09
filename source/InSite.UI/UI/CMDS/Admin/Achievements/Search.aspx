<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="SearchCriteria.ascx" TagName="AchievementSearchCriteria" TagPrefix="uc" %>
<%@ Register Src="SearchResults.ascx" TagName="AchievementSearchResults" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:AchievementSearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:AchievementSearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>
