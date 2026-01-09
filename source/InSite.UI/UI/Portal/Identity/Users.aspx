<%@ Page Language="C#" CodeBehind="Users.aspx.cs" Inherits="InSite.UI.Desktops.Design.Users.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./Controls/SearchCriteria.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./Controls/SearchResults.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem ID="SearchResultsTab" runat="server" Title="Results" Icon="fas fa-database">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem ID="SearchCriteriaTab" runat="server" Title="Search" Icon="fas fa-search">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem ID="DownloadsTab" runat="server" Title="Downloads" Icon="fas fa-download">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
    </insite:Nav>

</asp:Content>
