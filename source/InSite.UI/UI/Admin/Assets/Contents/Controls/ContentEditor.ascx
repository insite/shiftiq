<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor_" %>

<div class="row" runat="server" id="ErrorPanel" visible="false">
    <div class="col-md-12">
        <div class="alert alert-danger">
            <asp:Label runat="server" ID="ErrorAlert" />
        </div>
    </div>
</div>

<div class="row content-editor">
    <div class="col-md-12">
        <insite:Nav runat="server" ID="PillsNav"></insite:Nav>
    </div>
</div>
