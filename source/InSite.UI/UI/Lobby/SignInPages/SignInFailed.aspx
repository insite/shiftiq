<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" CodeBehind="SignInFailed.aspx.cs" Inherits="InSite.UI.Lobby.SignInPages.SignInFailed" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <div class="row">
        <div class="col-lg-4 col-md-6 offset-lg-1">
            <div class="view show">

                <div runat="server" class="alert d-flex alert-danger mb-4" role="alert">
                    <i class='far fa-stop-circle fs-4 mt-1 me-3'></i>
                    <div runat="server" id="ErrorViewMessage">
                    </div>
                </div>

                <div class="m-t-md">
                    <insite:Button runat="server" ID="ErrorViewCloseButton" Text="Close" Icon="fas fa-ban" ButtonStyle="Default" CssClass="me-2" />
                </div>

            </div>
        </div>
    </div>
</asp:Content>
