<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageInfo.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.PageInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Page Title
    </label>
    <div>
        <a id="PageLink" runat="server">
            <asp:Literal runat="server" ID="PageTitle" />
        </a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Page Slug
    </label>
    <div>
        <asp:Literal runat="server" ID="PageSlug" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Page Type
    </label>
    <div>
        <asp:Literal runat="server" ID="PageType" />
    </div>
</div>


