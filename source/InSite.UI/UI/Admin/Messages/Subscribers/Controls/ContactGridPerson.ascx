<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactGridPerson.ascx.cs" Inherits="InSite.Admin.Messages.Contacts.Controls.ContactGridPerson" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="False" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" ID="SendItemButton" Name="paper-plane" Type="Regular"
                    NavigateUrl='<%# string.Format("/ui/admin/messages/mailouts/schedule?message={0}&recipient={1}", Eval("MessageIdentifier"), Eval("UserIdentifier")) %>'
                    ToolTip='<%# Eval("UserFullName", "Send message to {0}") %>'
                    Visible='<%# !IsNotification() %>' />
                <insite:IconLink runat="server" ID="DeleteItemButton" Name="trash-alt" Type="Regular" ToolTip="Delete"
                    NavigateUrl='<%# string.Format("/ui/admin/messages/subscribers/delete?message={0}&recipient={1}", Eval("MessageIdentifier"), Eval("UserIdentifier")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User" ItemStyle-Width="300px">
            <ItemTemplate>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                    <%# Eval("UserFullName") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href="mailto:<%# Eval("UserEmail") %>"><%# Eval("UserEmail") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email Alternate" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <a href="mailto:<%# Eval("UserEmailAlternate") %>"><%# Eval("UserEmailAlternate") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subscribed" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Subscribed")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>