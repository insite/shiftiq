<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Learners.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="CertificateRepeater" Src="../../../Learning/Controls/CertificateRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./Controls/SearchCriteria.ascx" %> 
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./Controls/SearchResults.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddCertificate" Src="./Controls/AddCertificate.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="DummyAlert" Visible="false" />
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem ID="SearchResultsTab" runat="server" Title="Results" Icon="fas fa-database">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem ID="SearchCriteriaTab" runat="server" Title="Criteria" Icon="fas fa-filter">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem ID="DownloadsTab" runat="server" Title="Downloads" Icon="fas fa-download">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
        <insite:navitem id="AddNewTab" runat="server" title="Add New Certificate" icon="fas fa-circle-plus">
            <uc:AddCertificate runat="server" ID="AddCertificate" />
        </insite:navitem>
    </insite:Nav>

</asp:Content>
