<%@ Page Language="C#" CodeBehind="VerifyEmail.aspx.cs" Inherits="InSite.UI.Lobby.VerifyEmail" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

<div class="row">
	<div class="col-lg-5 col-md-6">

        <div class="view show" id="signin-view">
            <h2 class="mb-4"><insite:Literal runat="server" Text="Email Verification" /></h2>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <div>
                <insite:CloseButton runat="server" ID="CloseLink" CssClass="right" />
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