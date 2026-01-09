<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileHistoryList.ascx.cs" Inherits="InSite.UI.Admin.Assets.Files.Controls.FileHistoryList" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <h3>History</h3>

        <insite:Alert runat="server" ID="EmptyMessage" Indicator="Warning">
            No changes
        </insite:Alert>

        <asp:Repeater runat="server" ID="HistoryList">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Changed On</th>
                            <th>Changed By</th>
                            <th>Changes</th>
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
                        <%# Eval("Date") %>
                    </td>
                    <td>
                        <%# Eval("User") %>
                    </td>
                    <td>
                        <div runat="server" id="CreatedPanel">
                            Document Created
                        </div>
                        <asp:Repeater runat="server" ID="ChangeRepeater">
                            <HeaderTemplate><ul style="margin-bottom:0;"></HeaderTemplate>
                            <FooterTemplate></ul></FooterTemplate>
                            <ItemTemplate>
                                <li>
                                    <b><i><%# Eval("FieldName") %></i></b> changed from <%# Eval("OldValueHtml") %> to <%# Eval("NewValueHtml") %>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>