<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentBlocksManager.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocksManager" %>

<div runat="server" id="InfoPanel" class="alert alert-info" visible="false"></div>
<div runat="server" id="DangerPanel" class="alert alert-danger" visible="false"></div>

<div class="row content-editor">
    <div class="form-group mb-3">
        <div class="row">
            <div class="col-md-2">
                <insite:Nav runat="server" ID="Navigation" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="NavigationRenderer">
                </insite:Nav>
            </div>
            <div class="col-md-10">
                <insite:NavContent runat="server" ID="NavigationRenderer" />
            </div>
        </div>
    </div>
</div>
