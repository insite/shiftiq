<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskPresentationSetup.ascx.cs" Inherits="InSite.Admin.Records.Programs.Tasks.Controls.TaskPresentationSetup" %>

<div class="row mb-3">
    <div class="col-lg-6">

        <div class="form-group mb-3" runat="server" id="TaskImageField">
            <label class="form-label">
                Task Image
            </label>

            <div style="position: relative;">
                <asp:Image runat="server" ID="TaskImage" CssClass="img-responsive" />
                <div style="position: absolute; top: 0px; right: 0px;">
                    <insite:IconButton runat="server" ID="DeleteImage" Name="trash-alt" Type="Solid" ToolTip="Delete this image" />
                </div>
            </div>
        </div>

        <div class="mb-3">
            <insite:FileUploadV2 runat="server" ID="TaskImageUploadV2" LabelText="Upload New Task Image" FileUploadType="Image"/>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                URL overwrite
            </label>
            <insite:TextBox runat="server" ID="TaskImageUrl" />
        </div>

    </div>
</div>