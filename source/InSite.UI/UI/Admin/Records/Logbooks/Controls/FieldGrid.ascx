<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldGrid.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.FieldGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="AddFields" Text="Add Fields" Icon="fas fa-plus-circle" ButtonStyle="Default" />
    <insite:Button runat="server" ID="ReorderFields" Text="Reorder" Icon="fas fa-sort" ButtonStyle="Default" />
</div>

<asp:Repeater runat="server" ID="FieldRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>Field Name</th>
                    <th style="text-align:center;">Required Field</th>
                    <th>Tag Text</th>
                    <th>Help Text</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:80px;">
                <insite:IconLink runat="server" ToolTip="Change" Name="pencil" NavigateUrl='<%# Eval("Identifier", "/ui/admin/records/logbooks/change-field?field={0}") %>' />
                <insite:IconLink runat="server" ToolTip="Delete" Name="trash-alt" NavigateUrl='<%# Eval("Identifier", "/admin/records/logbooks/delete-field?field={0}") %>' />
            </td>
            <td>
                <%# Eval("FieldType") %>
            </td>
            <td style="text-align:center;">
                <%# Eval("IsRequired") %>
            </td>
            <td>
                <%# Eval("LabelText") %>
            </td>
            <td>
                <%# Eval("HelpText") %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
