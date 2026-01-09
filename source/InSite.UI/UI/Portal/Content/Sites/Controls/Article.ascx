<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Article.ascx.cs" Inherits="InSite.UI.Portal.Sites.PageArticle" %>

<div class="row">
    <div class="col-lg-12">

        <div runat="server" id="DangerPanel" class="alert alert-danger" visible="false"></div>
        <div runat="server" ID="OutlineBody" class="mb-5"></div>
        <asp:PlaceHolder runat="server" ID="BodyContentBlocks"></asp:PlaceHolder>

    </div>
</div>