<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardInfo.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.StandardInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Standard Title
    </label>
    <div>
        <a id="StandardLink" runat="server">
            <asp:Literal runat="server" ID="StandardTitle" />
        </a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Standard Code
    </label>
    <div>
        <asp:Literal runat="server" ID="StandardCode" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Standard Tag
    </label>
    <div>
        <asp:Literal runat="server" ID="StandardLabel" />
    </div>
</div>
