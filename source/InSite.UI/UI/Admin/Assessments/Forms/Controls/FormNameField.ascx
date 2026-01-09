<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormNameField.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormNameField" %>

<div class="form-group mb-3">
    <label class="form-label">
        Form Name
        <insite:RequiredValidator runat="server" ID="FormNameValidator" ControlToValidate="FormName" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="FormName" MaxLength="128" Width="100%" />
    </div>
    <div class="form-text">
        This is the internal name used to uniquely identify this exam form for filing purposes.
    </div>
</div>