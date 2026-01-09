<%@ Page Language="C#" CodeBehind="Password.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.ChangePassword" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="PasswordStrength" Src="~/UI/Lobby/Controls/PasswordStrength.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .form-control.is-invalid {
            background-position: right calc(2.5em + 0.28125rem) center;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:UserPasswordCheck runat="server" />
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Change" />

    <div runat="server" ID="ContentPanel" class="row">

        <div class="col-lg-6">

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
                <div class="form-text">
                    <insite:Literal runat="server" Text="ChangePassword New Password Hint" />
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

        </div>

        <div class="col-lg-6">

            <div class="alert alert-info">
                <insite:Literal runat="server" Mode="Markdown" Text="Password Strength Help" />
            </div>

        </div>

    </div>

    <div class="row mt-3" runat="server" id="ButtonPanel">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Change" tabindex="4" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <div class="row" runat="server" id="HomePanel" visible="false">
        <div class="col-lg-6">
            <insite:Button runat="server" ID="HomeButton" Text="Continue" ButtonStyle="Primary" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" />
        </div>
    </div>

    <insite:PageHeadContent runat="server" ID="PasswordExpiredHeadContent" Visible="false">
        <style type="text/css">
            header.navbar {
                display: none;
            }

            body {
                margin-bottom: 0 !important;
            }
        </style>
    </insite:PageHeadContent>
    <insite:PageFooterContent runat="server" ID="PasswordExpiredFooterContent" Visible="false">
        <script type="text/javascript">
            (function () {
                var header = document.querySelector('header');
                if (header)
                    header.remove();

                var footer = document.querySelectorAll('footer');
                if (footer.length > 0)
                    footer[footer.length - 1].remove();
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
