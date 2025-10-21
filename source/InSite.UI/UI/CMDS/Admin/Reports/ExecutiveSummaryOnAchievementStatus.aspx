<%@ Page Language="C#" CodeBehind="ExecutiveSummaryOnAchievementStatus.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.Forms.ExecutiveSummaryOnAchievementStatus" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/ExecutiveSummaryOnAchievementStatusCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ExecutiveSummaryOnAchievementStatusResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
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
