<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupGrid.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.GroupGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="GroupIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="80px" ItemStyle-Wrap="False" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to remove this group from this message?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group">
            <ItemTemplate>
                <a href="/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>">
                    <%# Eval("GroupName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Size" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" ItemStyle-Width="120px">
            <ItemTemplate>
                <%# Eval("MembershipCount") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Added" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>