<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Text.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields.Text" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="TextValue" ErrorMessage="This is a required value" Display="None" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="TextValue" Width="100%" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
