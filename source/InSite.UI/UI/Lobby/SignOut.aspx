<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master"
    CodeBehind="SignOut.aspx.cs" Inherits="InSite.UI.Lobby.SignOut" %>

<asp:Content runat="server" ContentPlaceHolderID ="HeadContent">

    <meta http-equiv='Cache-Control' content='no-cache, no-store, must-revalidate' />
    <meta http-equiv='Pragma' content='no-cache' />
    <meta http-equiv='Expires' content='0' />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="col-lg-4 col-md-6 offset-lg-1">

    <h1 class="h2">Signing Out</h1>

    <p>
        You are being signed out. Please wait a moment...
    </p>

    <p class="fs-sm text-muted">(I am emptying the cookie jar...)</p>

    </div>

</asp:Content>
