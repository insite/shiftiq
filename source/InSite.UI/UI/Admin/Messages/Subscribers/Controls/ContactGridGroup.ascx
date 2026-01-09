<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactGridGroup.ascx.cs" Inherits="InSite.Admin.Messages.Contacts.Controls.ContactGridGroup" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="GroupIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="False" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to remove this group from this message?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group" ItemStyle-Width="300px">
            <ItemTemplate>
                <a href="/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>">
                    <%# Eval("GroupName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Size" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" ItemStyle-Width="120px">
            <ItemTemplate>
                <%# Eval("GroupSize") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subscribed" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Subscribed")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>