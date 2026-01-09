<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.ContentEditor" %>

<insite:PageHeadContent runat="server">
    <style>
        .btn-xs, .btn-group-xs > .btn {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }
    </style>
</insite:PageHeadContent>

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

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .content-editor .content-field {
            position: relative;
        }

        .content-editor .content-field > .commands {
            position: absolute;
            right: 51px;
            top: -2px;
        }

        .content-editor .content-field > .commands > span.lang-out {
            background-color: #aaa;
        }

    </style>
</insite:PageHeadContent>
