<%@ Page Language="C#" CodeBehind="ResetPassword.aspx.cs" Inherits="InSite.UI.Lobby.ResetPassword" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>
<%@ Register TagPrefix="uc" TagName="PasswordStrength" Src="Controls/PasswordStrength.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

<div class="row">
<div class="col-lg-4 col-md-6 offset-lg-1">
<div class="view show" id="signin-view">

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>

            <insite:Alert runat="server" ID="StatusAlert" />

            <insite:ValidationSummary runat="server" ValidationGroup="Change" />
                    
            <asp:MultiView runat="server" ID="ScreenViews">

                <asp:View runat="server" ID="ViewRequest">

                    <h1 class="h2"><insite:Literal runat="server" Text="Reset Password" /></h1>

                    <p><insite:Literal runat="server" Text="Password Reset Help" /></p>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Email" />
                            <insite:RequiredValidator runat="server" ControlToValidate="RequestEmail" Display="None" ValidationGroup="Request" />
                            <insite:EmailValidator runat="server" ControlToValidate="RequestEmail" Display="None" ValidationGroup="Request" />
                        </label>
                        <insite:TextBox runat="server" ID="RequestEmail" autocomplete="off" />
                    </div>

                    <div class="form-group">
                        <insite:Button runat="server" ID="RequestButton" ButtonStyle="Success" Icon="fas fa-paper-plane" ValidationGroup="Request" />
                    </div>

                </asp:View>

                <asp:View runat="server" ID="ViewChange">

                    <h1>
                        <insite:Literal runat="server" Text="Create New Password" />
                    </h1>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="New Password" />
                            <insite:RequiredValidator runat="server" ControlToValidate="Password" Display="None" ValidationGroup="Change" />
                        </label>
                        <div class="password-toggle">
                            <insite:TextBox runat="server" ID="Password" TextMode="Password" TabIndex="2" />
                            <label class="password-toggle-btn" aria-label="Show/Hide password">
                                <input class="password-toggle-check" type="checkbox"><span class="password-toggle-indicator"></span>
                            </label>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Confirm Password" />
                            <insite:RequiredValidator runat="server" ControlToValidate="PasswordConfirm" Display="None" ValidationGroup="Change" />
                            <insite:CompareValidator runat="server" ControlToValidate="PasswordConfirm" Display="None" ControlToCompare="Password" ValidationGroup="Change" ErrorMessage="Passwords do not match" />
                        </label>
                        <div class="password-toggle">
                            <insite:TextBox runat="server" ID="PasswordConfirm" TextMode="Password" TabIndex="3" />
                            <label class="password-toggle-btn" aria-label="Show/Hide password">
                                <input class="password-toggle-check" type="checkbox"><span class="password-toggle-indicator"></span>
                            </label>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Password Strength" />
                        </label>
                        <div>
                            <uc:PasswordStrength runat="server" ID="PasswordStrength" ControlID="Password" />
                        </div>
                    </div>

                    <div class="form-group mb-4">
                        <insite:Button runat="server" ID="ChangeButton" ButtonStyle="Success" Icon="fas fa-cloud-upload" ValidationGroup="Change" />
                        <insite:CancelButton runat="server" ID="CancelButton" />
                    </div>

                    <div class="alert alert-info">
                        <insite:Literal runat="server" Mode="Markdown" Text="Password Strength Help" />
                    </div>

                </asp:View>

            </asp:MultiView>

        </ContentTemplate>
    </insite:UpdatePanel>

</div>
</div>
</div>

</asp:Content>