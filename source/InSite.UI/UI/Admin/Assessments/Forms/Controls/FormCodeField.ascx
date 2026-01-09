<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormCodeField.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormCodeField" %>

<div class="form-group mb-3">
    <label class="form-label">
        Code
        <insite:RequiredValidator runat="server" ID="CodeValidator" ControlToValidate="Code" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="Code" Text="N/A" MaxLength="40" />
    </div>
    <div class="form-text">
        This is the form's catalog code.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Source</label>
    <div>
        <insite:TextBox runat="server" ID="Source" MaxLength="80" />
    </div>
    <div class="form-text">
        Reference to the source of the content and/or configuration for this form.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Origin</label>
    <div>
        <insite:TextBox runat="server" ID="Origin" MaxLength="100" />
    </div>
    <div class="form-text">
        Identifies the originating platform and/or record for this form. When this property is used, it should 
        ideally contain a fully qualified URL or API path.
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Hook / Integration Code</label>
    <div>
        <insite:TextBox runat="server" ID="Hook" MaxLength="100" />
    </div>
    <div class="form-text">
        Unique code for integration with internal toolkits and external systems.
    </div>
</div>