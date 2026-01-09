<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageDetailsInfo.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.MessageDetailsInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Subject
    </label>
    <div>
        <a runat="server" Id="MessageLink">
            <asp:Literal runat="server" ID="MessageTitle" />
        </a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Message Type
    </label>
    <div>
        <asp:Literal runat="server" ID="MessageType" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Application Change Type
    </label>
    <div>
        <asp:Literal runat="server" ID="ApplicationChangeType" />
    </div>
</div>