<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" CodeBehind="SignInMFA.aspx.cs" Inherits="InSite.UI.Lobby.SignInPages.SignInMFA" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="row">
        <div class="col-lg-4 col-md-6 offset-lg-1">
            <div class="view show">

                <h1 class="h2">
                    <insite:Literal runat="server" Text="Multi-factor authentication" />
                </h1>

                <div style="text-align: center; padding-top: 20px;">
                    <insite:Literal runat="server" ID="MfaText" />
                </div>
                <div runat="server" id="MfaPhone" style="text-align: center; padding-bottom: 20px;"></div>
                <br />
                <div runat="server" id="errorView" class="alert d-flex alert-danger mb-4" role="alert" visible="false">
                    <i class='far fa-stop-circle fs-4 mt-1 me-3'></i>
                    <div runat="server" id="ErrorViewMessage">
                    </div>
                </div>

                <div runat="server" id="resentmessageview" class="alert d-flex alert-info mb-4" role="banner" visible="false">
                    <i class='far fa-info-square fs-4 mt-1 me-3'></i>
                    <div>
                        We have resent the authentication code to you
                    </div>
                </div>
                <asp:hiddenfield runat="server"  ID="lastSent" />
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Code" />
                        <insite:RequiredValidator runat="server" ControlToValidate="MfaCode" Display="None" ValidationGroup="MFA" />
                    </label>
                    <insite:TextBox runat="server" ID="MfaCode" MaxLength="6" autocomplete="off" />
                </div>

                <div class="m-t-md" style="text-align: center;">
                    <insite:Button runat="server" ID="MfaButton" Text="Submit" ButtonStyle="Success" ValidationGroup="MFA" Icon="fas fa-sign-in-alt" />
                    <insite:Button runat="server" ID="ResendButton" Text="Resend" ButtonStyle="Default" Visible="false" Icon="fas fa-redo" />
                </div>

            </div>
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            var pageInitTime = Date.now();
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= MfaCode.ClientID %>').on('keydown', onMfaFieldKeydown);
                });

                function onMfaFieldKeydown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        $('#<%= MfaButton.ClientID %>')[0].click();
                    }
                }
            })();
            function clientClick() {
                let currentTime = Date.now();
                if ((currentTime - pageInitTime) / 1000 > 60) {
                    return true;
                }
                alert('Please wait one minute before requesting another code.');
                return false;
            }
        </script>

    </insite:PageFooterContent>
</asp:Content>
