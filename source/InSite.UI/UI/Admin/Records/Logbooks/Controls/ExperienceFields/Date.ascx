<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Date.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields.Date" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="DateValue" ErrorMessage="This is a required value" Display="None" />
    </label>
    <div>
        <insite:DateSelector runat="server" ID="DateValue" Width="250px" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
