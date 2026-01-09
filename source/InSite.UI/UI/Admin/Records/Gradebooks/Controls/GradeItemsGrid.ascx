<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeItemsGrid.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.GradeItemsGrid" %>

<div runat="server" id="NoItems" class="alert alert-warning" role="alert">
    There are no grade items
</div>

<asp:Repeater runat="server" ID="ItemRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Grade Items</th>
                    <th>Achievement</th>
                    <th style="text-align:center;">Reporting</th>
                    <th>Type</th>
                    <th runat="server" id="GradeItemControls"></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <div style='<%# "padding-left:" + (15 * (int)Eval("Level")) + "px" %>'>

                    <%# Eval("Name") %> 
                    
                    <span class="form-text">
                        <%# GetCodeAndShortName(Eval("Code"),Eval("ShortName")) %>
                    </span>
                    
                    <div class="form-text text-warning">
                        <%# Eval("Hook") %>
                    </div>

                    <div runat="server" id="NoIncludedItems" class="form-text" visible="false">
                        Included Items: <strong>None</strong>
                    </div>

                    <asp:Repeater runat="server" ID="IncludedItems">
                        <HeaderTemplate>
                            <div class="form-text">
                                Included Items:
                                <ul>
                        </HeaderTemplate>
                        <FooterTemplate></ul></div></FooterTemplate>
                        <ItemTemplate>
                            <li>
                                <%# Eval("Name") %>
                                <%# Eval("Score") != null ? Eval("Score", ": {0:p1}") : "" %>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Repeater runat="server" ID="Standards">
                        <HeaderTemplate>
                            <div class="form-text">
                                Standards:
                                <ul>
                        </HeaderTemplate>
                        <FooterTemplate></ul></div></FooterTemplate>
                        <ItemTemplate>
                            <li>
                                <%# Eval("Title") %>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </td>
            <td>
                <a runat="server" visible='<%# !HideControls && Eval("AchievementIdentifier") != null %>' href='<%# Eval("AchievementIdentifier", "/ui/admin/records/achievements/outline?id={0}") %>'>
                    <%# Eval("AchievementTitle") %>
                </a>
                <asp:Literal runat="server" Visible='<%# HideControls %>' Text='<%# Eval("AchievementTitle") %>' />
            </td>
            <td style="text-align:center;">
                <%# (bool)Eval("IncludeToReport") ? "Yes" : "No" %>
            </td>
            <td>
                <%# (string)Eval("Type") != "Score" ? (string)Eval("Type") : "" %>
                <asp:Repeater runat="server" ID="OptionRepeater">
                    <ItemTemplate>
                        <%# Container.DataItem %>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
            <td runat="server" id="GradeItemControls" style="width:90px;text-align:right;" class="text-nowrap">
                <insite:IconLink runat="server" ID="ReorderLink" ToolTip="Reorder this item" Name="sort" />
                <insite:IconLink runat="server" ID="EditLink" ToolTip="Edit" Name="pencil" />
                <insite:IconLink runat="server" ID="DeleteLink" ToolTip="Void" Name="trash-alt" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
