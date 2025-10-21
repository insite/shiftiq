<%@ Page Language="C#" CodeBehind="Authenticate.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.MultifactorAuthentication" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="MFASelectMode" Src="Controls/MFASelectMode.ascx" %>
<%@ Register TagPrefix="uc" TagName="MFAAuthenticatorApp" Src="Controls/MFAAuthenticatorApp.ascx" %>
<%@ Register TagPrefix="uc" TagName="MFAText" Src="Controls/MFAText.ascx" %>
<%@ Register TagPrefix="uc" TagName="MFAEmail" Src="Controls/MFAEmail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="MFA" />

    <insite:Alert runat="server" ID="EditorStatus" />

    <uc:MFASelectMode runat="server" ID="MFASelectMode" />
    <uc:MFAAuthenticatorApp runat="server" ID="MFAAuthenticatorApp" />
    <uc:MFAText runat="server" ID="MFAText" />
    <uc:MFAEmail runat="server" ID="MFAEmail" />


</asp:Content>
