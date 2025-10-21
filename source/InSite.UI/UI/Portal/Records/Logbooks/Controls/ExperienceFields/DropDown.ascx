<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropDown.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields.DropDown" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="DropDownValue" Display="None" />
    </label>
    <div>
        <insite:TrainingTypeComboBox runat="server" ID="DropDownValue" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>

