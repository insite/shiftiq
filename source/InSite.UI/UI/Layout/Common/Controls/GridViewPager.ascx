<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridViewPager.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.GridViewPager" %>

<nav aria-label="Page Navigation">
    <ul class="pagination" style="margin:0; float:left;">
        <li runat="server" id="PrevPageContainer" visible="false" class="page-item">
            <asp:LinkButton runat="server" ID="PrevPage" CssClass="page-link" CommandName="Page" ToolTip="Previous Pages" Text="..." />
        </li>

        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>
                <li runat="server" class='<%# "page-item" + ((bool)Eval("IsActive") ? " active" : string.Empty) %>'>
                    <asp:LinkButton runat="server" Enabled='<%# !(bool)Eval("IsActive") %>' CssClass="page-link"
                        CommandName="Page" CommandArgument='<%# Eval("Page") %>'
                        Text='<%# Eval("Page", "{0:n0}") %>' ToolTip='<%# Eval("Page", "Go To Page #{0:n0}") %>' />
                </li>
            </ItemTemplate>
        </asp:Repeater>

        <li runat="server" id="NextPageContainer" visible="false" class="page-item">
            <asp:LinkButton runat="server" ID="NextPage" CssClass="page-link" CommandName="Page" ToolTip="Next Pages" Text="..." />
        </li>
    </ul>
    <div style="float:right; line-height:36px;">
        <asp:Literal runat="server" ID="Info" />
    </div>
    <div class="clearfix"></div>
</nav>
