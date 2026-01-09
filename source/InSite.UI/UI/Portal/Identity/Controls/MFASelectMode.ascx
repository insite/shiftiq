<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MFASelectMode.ascx.cs" Inherits="InSite.UI.Portal.Accounts.Users.Controls.MFASelectMode" %>

<div class="card mb-3">
    <div class="card-body">
        <insite:RadioButton ID="ModeNone" runat="server" Text="Disabled" GroupName="Mode" />
        <insite:RadioButton ID="ModeAuthenticatorApp" runat="server" Text="Authenticator app" GroupName="Mode" />
        <insite:RadioButton ID="ModeText" runat="server" Text="Text Message" SubText="(Canadian mobile numbers/carriers only)" GroupName="Mode" />
        <insite:RadioButton ID="ModeEmail" runat="server" Text="Email" GroupName="Mode" />
    </div>
</div>

<insite:Button runat="server" ID="ContinueButton" IconPosition="AfterText" Icon="fas fa-arrow-alt-right" Text="Continue" />
<insite:SaveButton runat="server" ID="SaveButton" />
<insite:CancelButton runat="server" ID="CancelButton" />
