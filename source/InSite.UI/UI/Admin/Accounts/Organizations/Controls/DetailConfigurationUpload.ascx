<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationUpload.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationUpload" %>

<div class="row">
    <div class="col-md-3">

        <h3>Images</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Maximum File Size
                <insite:RequiredValidator runat="server" ControlToValidate="UploadImageMaxFileSize" FieldName="Image Maximum File Size" ValidationGroup="Organization" />
            </label>
            <div>
                <insite:NumericBox runat="server" ID="UploadImageMaxFileSize" NumericMode="Integer" MinValue="0" Width="150px" style="display:inline;" /> bytes
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Maximum Height
                <insite:RequiredValidator runat="server" ControlToValidate="UploadImageMaxHeight" FieldName="Image Maximum Height" ValidationGroup="Organization" />
            </label>
            <div>
                <insite:NumericBox runat="server" ID="UploadImageMaxHeight" NumericMode="Integer" MinValue="0" Width="150px" style="display:inline;" /> pixels
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Maximum Width
                <insite:RequiredValidator runat="server" ControlToValidate="UploadImageMaxWidth" FieldName="Image Maximum Width" ValidationGroup="Organization" />
            </label>
            <div>
                <insite:NumericBox runat="server" ID="UploadImageMaxWidth" NumericMode="Integer" MinValue="0" Width="150px" style="display:inline;" /> pixels
            </div>
        </div>

    </div>
    <div class="col-md-3">

        <h3>Documents</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Maximum File Size
                <insite:RequiredValidator runat="server" ControlToValidate="UploadDocumentMaxFileSize" FieldName="Document Maximum File Size" ValidationGroup="Organization" />
            </label>
            <insite:NumericBox runat="server" ID="UploadDocumentMaxFileSize" NumericMode="Integer" MinValue="0" Width="150px" style="display:inline;" /> bytes
        </div>

    </div>
</div>
