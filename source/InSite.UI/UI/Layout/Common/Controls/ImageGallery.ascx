<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageGallery.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.ImageGallery" %>

<div class="container">
    <section class="pt-4 mt-lg-2 pb-lg-0">
        <div class="gallery row">
            <asp:Repeater runat="server" ID="ImageRepeater">
                <ItemTemplate>
                    <div class="col-md-4 col-sm-6 mb-4"><a class="gallery-item rounded-3" href='<%# Eval("Url") %>'><img src="<%# Eval("Url") %>" alt="<%# Eval("Alt") %>"></a></div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</div>