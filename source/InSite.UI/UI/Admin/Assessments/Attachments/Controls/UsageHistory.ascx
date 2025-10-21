<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsageHistory.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.UsageHistory" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class='table table-striped table-usage-history'><thead>
            <tr>
                <th>Attachment</th>
                <th>User</th>
                <th>Time</th>
                <th>Action</th>
                <th>Info</th>
            </tr>
        </thead><tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="text-nowrap"><%# Eval("Asset") %></td>
            <td class="text-nowrap"><%# Eval("User") %></td>
            <td class="text-nowrap"><%# Eval("Time") %></td>
            <td class="text-nowrap"><%# Eval("Action") %></td>
            <td class="td-html"><%# Eval("InfoHtml") %></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        table.table-usage-history td.td-html img {
            max-width: 100%;
        }
    </style>
</insite:PageHeadContent>