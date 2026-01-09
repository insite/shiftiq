<%@ Page Language="C#" CodeBehind="Submit.aspx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.SubmitPage" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="HeadContent" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitHeadContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="SideContent" Src="~/UI/Portal/Workflow/Forms/Controls/SubmitSideContent.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <uc:HeadContent runat="server" ID="HeadContent" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:SideContent runat="server" ID="SideContent" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <div class="lesson col-full-height" style="z-index:auto;">
        <asp:PlaceHolder runat="server" ID="VerbPlaceholder" />
    </div>
</asp:Content>
