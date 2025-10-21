<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./Controls/SearchCriteria.ascx" %> 
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./Controls/SearchResults.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<section class="container mb-2 mb-sm-0 pb-sm-5">

    <h2 runat="server" id="PageTitle" class="mb-4"></h2>

    <insite:Alert runat="server" ID="DummyAlert" Visible="false" />
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem ID="SearchResultsTab" runat="server" Title="Results" Icon="fas fa-database">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem ID="SearchCriteriaTab" runat="server" Title="Search" Icon="fas fa-search">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
    </insite:Nav>

</section>
</asp:Content>
