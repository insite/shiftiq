<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesSelectorRepeater.ascx.cs" Inherits="InSite.Admin.Courses.Activities.Controls.CompetenciesSelectorRepeater" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate><ul class="tree-view" data-init="code"></HeaderTemplate>
    <FooterTemplate></ul></FooterTemplate>
    <ItemTemplate>
        <li class="outline-item competency-check" data-key="<%# Eval("Asset") %>" data-competency="<%# (bool)Eval("IsCompetency") ? "1" : "0" %>">
            <div>
                <div>
                    <div class="node-title">
                        <label>
                        <asp:CheckBox runat="server" ID="IsSelected" Checked='<%# Eval("IsSelected") %>' />
                        <span class='text'>
                            <%# Eval("TypeIcon") != null ? Eval("TypeIcon", "<i class='align-middle {0}'></i>") : null %>
                            <%# (string.IsNullOrEmpty((string)Eval("Code")) ? string.Empty : (string)Eval("Code") + ". ") + (string)Eval("Title") %>
                        </span>
                        <small class='text-body-secondary text-nowrap'><%# Eval("TypeName") %> <%# Eval("Asset") %></small>
                        </label>
                    </div>
                </div>
            </div>

            <insite:DynamicControl runat="server" ID="Container" />
        </li>
    </ItemTemplate>
</asp:Repeater>
