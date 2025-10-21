<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" CodeBehind="SignIn.aspx.cs" Inherits="InSite.UI.Lobby.SignIn" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .myhr {
                border: none;
                border-top: 3px solid black;
                color: black;
                overflow: visible;
                text-align: center;
                height: 5px;
                opacity: 0.25;
                margin: 1rem 0;
            }

                .myhr:after {
                    background: #fff;
                    content: 'or';
                    padding: 0 4px;
                    position: relative;
                    top: -13px;
                }
        </style>
    </insite:PageHeadContent>

    <uc:MaintenanceToast runat="server" ID="MaintenanceToast" />

    <div class="row">
        <div class="col-lg-4 col-md-6 offset-lg-1">
            <div class="view show position-relative" id="signin-view">

                <table runat="server" id="SignInRegistration" visible="false" class="login-toggle mb-4">
                    <tr>
                        <td class="selected">
                            <insite:Literal runat="server" Text="Returning Users" />
                        </td>
                        <td class="unselected">
                            <a runat="server" id="RegisterLink">
                                <insite:Literal runat="server" Text="New Users" />
                            </a>
                        </td>
                    </tr>
                </table>

                <h1 class="h2">
                    <insite:Literal runat="server" Text="Sign In" />
                </h1>

                <insite:Alert runat="server" ID="SignInStatus" />

                <div runat="server" id="SignInIeWarning" class="alert alert-danger" visible="false">
                    <table>
                        <tr>
                            <td style="width: 20px; padding-right: 10px; padding-top: 2px; vertical-align: top;">
                                <i class="fas fa-exclamation-triangle"></i>
                            </td>
                            <td>
                                <strong>
                                    <insite:Literal runat="server" Text="Microsoft Internet Explorer is no longer supported." /></strong>
                                <insite:Literal runat="server" Text="IE Warning" />
                            </td>
                        </tr>
                    </table>
                </div>

                <div runat="server" id="SignInBrowserRecommendation" class="alert alert-warning" visible="false">
                    <table>
                        <tr>
                            <td style="width: 20px; padding-right: 10px; padding-top: 2px; vertical-align: top;">
                                <i class="fas fa-exclamation-triangle"></i>
                            </td>
                            <td>
                                <insite:Literal runat="server" Text="We recommend using the latest version of Chrome or Firefox or Edge for the best experience." />
                            </td>
                        </tr>
                    </table>
                </div>

                <insite:ValidationSummary runat="server" ValidationGroup="SignIn" />

                <div runat="server" id="SignInReferrer" visible="false" enableviewstate="false" class="alert alert-info mb-4" role="alert"></div>

                <div runat="server" id="SignInError" class="alert d-flex alert-danger mb-4" role="alert"></div>

                <insite:Container runat="server" ID="PasswordExpiredToast" Visible="false">
                    <div class="toast position-absolute w-100 show mt-1" style="z-index:1;" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="toast-header bg-danger text-white">
                            <strong class="me-auto">
                                <insite:Literal runat="server" Text="Please Note" />
                            </strong>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                        <div class="toast-body text-danger">
                            <insite:Literal runat="server">
                                You have been prompted 3 times to change your password, and you have not yet done so, 
                                therefore the system has logged out your session.
                                Please sign in again and change your password now.
                            </insite:Literal>
                        </div>
                    </div>
                </insite:Container>

                <div runat="server" id="SignInPanel">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Email" />
                            <insite:RequiredValidator runat="server" ControlToValidate="SignInUserName" Display="None" ValidationGroup="SignIn" />
                            <insite:EmailValidator runat="server" Display="None" ControlToValidate="SignInUserName" ValidationGroup="SignIn" FieldName="Email" />
                        </label>
                        <insite:TextBox runat="server" ID="SignInUserName" MaxLength="128" autocomplete="off" TabIndex="1" />
                    </div>

                    <div class="form-group mb-3">
                        <div class="float-end fs-xs">
                            <a runat="server" id="ResetPasswordAnchor">
                                <insite:Literal runat="server" Text="Forgot your password?" />
                            </a>
                        </div>
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Password" />
                            <insite:RequiredValidator runat="server" ControlToValidate="SignInPassword" Display="None" ValidationGroup="SignIn" />
                        </label>
                        <insite:TextBox runat="server" ID="SignInPassword" TextMode="Password" TabIndex="2" />
                    </div>

                    <div runat="server" id="SignInCommands" class="mb-4 w-100" visible="false">

                        <insite:Button runat="server" ID="SignInButton" Text="Login with Email" ButtonStyle="Success" Icon="fas fa-sign-in-alt" tabindex="3" ValidationGroup="SignIn" CssClass="d-block w-100 mb-2" />

                        <div runat="server" id="SignInButtonAlternatives">

                            <hr class="myhr" />

                            <insite:Button runat="server" Style="margin-top: 10px;" ID="MSSignInButton" Text="Login with Microsoft" ButtonStyle="info" Icon="fab fa-microsoft" tabindex="3" CssClass="d-block w-100 mb-2" />
                            <insite:Button runat="server" Style="margin-top: 10px;" ID="GoogleSignInButton" Text="Login with Google" ButtonStyle="info" Icon="fab fa-google" tabindex="3" CssClass="d-block w-100 mb-2" />

                        </div>

                    </div>

                    <div class="alert d-flex alert-warning">
                        <insite:Literal runat="server" Mode="Markdown" Text="User Sign In Help" />
                    </div>

                    <div class="form-text">
                        <p>
                            <insite:Literal runat="server" Text="User Sign In Warning" />
                        </p>
                        <p>
                            <insite:Literal runat="server" ID="SignInTerms" Text="User Sign In Terms" />
                        </p>
                        <p runat="server" id="SignInBrowser"></p>
                        <div runat="server" id="EnvironmentLinks"></div>
                    </div>

                </div>

            </div>
        </div>

        <div runat="server" id="CustomContentCard" class="col-lg-4 col-md-6 offset-lg-2">
            <div class="card shadow">
                <div class="card-body">
                    <asp:Literal runat="server" ID="CustomContentHtml" />
                </div>
            </div>
        </div>

    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= SignInUserName.ClientID %>,#<%= SignInPassword.ClientID %>').on('keydown', onSignInFieldKeydown);
                });

                function onSignInFieldKeydown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        $('#<%= SignInButton.ClientID %>')[0].click();
                    }
                }
            })();

            const updateOrAddQueryParameter = (key, value, pageURL) => {
                const url = new URL(pageURL);
                const searchParams = url.searchParams;
                if (searchParams.has(key)) {
                    searchParams.set(key, value);
                } else {
                    searchParams.append(key, value);
                }
                const newURL = url.toString();
                return newURL;
            }

        </script>

    </insite:PageFooterContent>

</asp:Content>