<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwoDates.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields.TwoDates" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator1" ControlToValidate="DateValue1" ErrorMessage="This is a required value" Display="None" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator2" ControlToValidate="DateValue2" ErrorMessage="This is a required value" Display="None" />
    </label>
    <div>
        <insite:DateSelector runat="server" ID="DateValue1" Width="250px" />
        <br /><br />
        <insite:DateSelector runat="server" ID="DateValue2" Width="250px" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
