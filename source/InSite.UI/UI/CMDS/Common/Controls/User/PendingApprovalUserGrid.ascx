<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PendingApprovalUserGrid.ascx.cs" Inherits="InSite.Cmds.Controls.User.PendingApprovalUserGrid" %>

<p runat="server" id="NoDataMessage" class="alert alert-info">There are no users pending approval.</p>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Roles</th>
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
            <td>
                <a href="/ui/cmds/admin/users/edit?userID=<%# Eval("UserIdentifier") %>"><%# Eval("FullName") %></a>
            </td>
            <td>
                <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
            </td>
            <td>
                <%# GetGroupList(Container.DataItem) %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

