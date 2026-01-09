<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="HeadContent" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitHeadContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="SideContent" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitSideContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitSearchCriteria.ascx" %> 
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitSearchResults.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <uc:HeadContent runat="server" ID="HeadContent" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:SideContent runat="server" ID="SideContent" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:Breadcrumbs runat="server" ID="Breadcrumbs" />

    <h3 runat="server" id="ResultsTitle" class="mb-0"></h3>
    <p><small class="text-body-secondary"><asp:Literal runat="server" ID="ResponseCount" /></small></p>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <uc:SearchResults runat="server" ID="SearchResults" />

    <insite:Container runat="server" Visible="false">
        <uc:SearchCriteria runat="server" ID="SearchCriteria" />
    </insite:Container>
</asp:Content>
