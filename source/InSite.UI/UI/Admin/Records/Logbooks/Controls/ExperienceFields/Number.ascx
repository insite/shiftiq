<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Number.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields.Number" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="NumberValue" ErrorMessage="This is a required value" Display="None" />
    </label>
    <div>
        <insite:NumericBox runat="server" ID="NumberValue" Width="250px" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
