<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineTreeNodeRepeater.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.OutlineTreeNodeRepeater" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate><ul class="tree-view" data-init="code"></HeaderTemplate>
    <FooterTemplate></ul></FooterTemplate>
    <ItemTemplate>
        <li class="outline-item" data-key="<%# Eval("Number") %>">
            <div>
                <div>
                    <div class="node-title">
                        <span class='text'>
                            <%# Eval("TypeIcon") != null ? Eval("TypeIcon", "<i class='align-middle {0} me-1'></i>") : null %>
                            <small class='badge bg-custom-default asset-code-<%# ((string)Eval("Type")).ToLowerInvariant().Replace(" ", "-") %>'><%# Eval("Code") %></small>
                            <a target="_blank" href="/ui/admin/standards/edit?id=<%# Eval("Identifier") %>"><%# Eval("Title") %></a>
                        </span>
                        <small class='text-body-secondary text-nowrap'><%# Eval("TypeName") %> Asset #<%# Eval("Number") %></small>
                        <%# (bool)Eval("IsPractical") ? "<small title='Practical' class='text-body-secondary'><i class='far fa-hand-paper'></i></small>" : null %>
                        <%# (bool)Eval("IsTheory") ? "<small title='Theory' class='text-body-secondary'><i class='far fa-book'></i></small>" : null %>
                        <%# InSite.Admin.Standards.Standards.Utilities.OutlineHelper.GetStatusBadgeHtml((string)Eval("Status"), null) %>

                        <asp:Repeater runat="server" ID="TagRepeater">
                            <ItemTemplate>
                                <small class='<%# Eval("ColorClass") %>'><%# Eval("Tag") %></small>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="node-summary" style="display: none;">
                        <small class="text-body-secondary text-nowrap">
                            <%# Eval("Summary") != null ? Shift.Common.Markdown.ToHtml(Eval("Summary").ToString()) : null %>
                        </small>
                    </div>

                    <div class="commands">
                        <a class="btn btn-sm btn-icon btn-default cmd-info" data-action="show-info" title="Show Details" href="javascript:void(0);"><i class="far fa-bars"></i></a>
                        <insite:RadioButton runat="server" CssClass="cmd-select" GroupName="node-select" data-action="select" autocomplete="off" />
                    </div>
                </div>
            </div>

            <insite:DynamicControl runat="server" ID="Container" />
        </li>
    </ItemTemplate>
</asp:Repeater>
