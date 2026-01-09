<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeCounterRepeater.ascx.cs" Inherits="InSite.UI.Admin.Foundations.Controls.HomeCounterRepeater" %>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>
        <div class="col">
            <a class="card card-hover card-tile border-0 shadow" href='<%# Eval("Url") %>'>
                <span runat="server" visible='<%# Eval("Count") != null %>' class="badge badge-floating badge-pill bg-primary"><%# Eval("Count", "{0:n0}") %></span>
                <div class="card-body text-center">
                    <span runat="server" visible='<%# IsTrial() %>' class="badge border border-warning text-warning fs-xl bg-white badge-trial">Trial</span>
                    <i runat="server" visible='<%# Eval("Icon") != null %>' class='<%# Eval("Icon", "fa-3x mb-3 {0}") %>'></i>
                    <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Title") %></h3>
                </div>
            </a>
        </div>
    </ItemTemplate>
</asp:Repeater>