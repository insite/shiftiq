<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteInfo.ascx.cs" Inherits="InSite.Admin.Sites.Sites.Controls.SiteInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Site Title
    </label>
    <div>
        <a id="Sitelink" runat="server">
            <asp:Literal runat="server" ID="SiteTitle" />
        </a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Domain
    </label>
    <div>
        <asp:Literal runat="server" ID="Domain" />
    </div>
</div>