<%@ Page Language="C#" CodeBehind="Respond.aspx.cs" Inherits="InSite.UI.Portal.Surveys.RespondPage" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="HeadContent" Src="Controls/RespondHeadContent.ascx" %>
<%@ Register TagPrefix="uc" TagName="SideContent" Src="Controls/RespondSideContent.ascx" %>

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
