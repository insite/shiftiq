<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwoColumns.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.TwoColumns" %>

<div class="container">
    <div class="row">
        <div class="col-md-6">
            <asp:Literal runat="server" ID="Column1" />
        </div>
        <div class="col-md-6">
            <asp:Literal runat="server" ID="Column2" />
        </div>
    </div>
</div>