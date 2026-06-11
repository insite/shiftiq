<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoicingContacts.ascx.cs" Inherits="InSite.Cmds.Controls.User.InvoicingContacts" %>

<asp:Repeater runat="server" ID="GroupMembershipRepeater">
    <ItemTemplate>
        <div runat="server" class="form-group mb-3" visible='<%# Convert.ToInt32(Eval("Members.Length")) > 0 %>'>
            <label class="form-label">
                <%# Eval("GroupName") %>
            </label>
            <div class="ms-3">
                <asp:Repeater runat="server" ID="MembersRepeater" DataSource='<%# Eval("Members") %>' OnItemCommand="MembersRepeater_ItemCommand">
                    <HeaderTemplate>
                        <ul class="list-unstyled mb-0">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="mb-2">
                            <div>
                                <insite:IconButton runat="server" Name="minus-circle" CommandName="Remove"
                                    Visible='<%# AllowMembershipDeletion %>'
                                    CommandArgument='<%# Eval("GroupId") + "|" + Eval("UserId") %>'
                                    ToolTip="Remove from team"
                                    OnClientClick="if (!confirm('Remove this user from the team?')) return false;" />
                                <%# Eval("UserName") %>
                                <span class="form-text">
                                    <%# Eval("UserEmail") %> &middot; <%# Eval("UserPhone") %>
                                </span>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
