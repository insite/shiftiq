<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTreeView.ascx.cs" Inherits="InSite.UI.Admin.Sites.Pages.Controls.PageTreeView" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate><ul class="tree-view" data-init="code"></HeaderTemplate>
    <FooterTemplate></ul></FooterTemplate>
    <ItemTemplate>
        <li data-key='<%# Eval("Id") %>' class="<%# (bool)Eval("IsActive") ? "selected" : null %>">
            <div>
                <div>
                    <div class="node-title">
                        <i class='<%# GetItemIcon() %> me-1'></i>
                        <a href="<%# GetItemEditUrl() %>" class="<%# (bool)Eval("IsActive") ? "fst-italic" : null %>"><%# Eval("Title") %></a>
                        <span class='form-text ms-2'>
                            <%# GetItemDescription() %>
                        </span>
                    </div>
                </div>
            </div>

            <insite:DynamicControl runat="server" ID="Container" />
        </li>
    </ItemTemplate>
</asp:Repeater>
