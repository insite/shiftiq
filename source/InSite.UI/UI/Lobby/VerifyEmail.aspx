<%@ Page Language="C#" CodeBehind="VerifyEmail.aspx.cs" Inherits="InSite.UI.Lobby.VerifyEmail" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

<div class="row">
	<div class="col-lg-5 col-md-6">

        <div class="view show" id="signin-view">
            <h2 class="mb-4"><insite:Literal runat="server" Text="Email Verification" /></h2>
            <insite:Alert runat="server" ID="ScreenStatus" />

            <p runat="server" id="SendMessage" class="mb-3" visible="false">
            </p>

            <div>
                <insite:Button runat="server" ID="SendButton" ButtonStyle="Primary" Icon="fas fa-paper-plane" Visible="false" />
                <insite:CloseButton runat="server" ID="CloseButton" CssClass="right" />
            </div>
        </div>

    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        header,
        footer {
            display: none;
        }
    </style>
</insite:PageHeadContent>

</asp:Content>