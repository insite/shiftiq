<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Messages.Clicks.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:TemplateField HeaderText="Clicked" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("Clicked","{0:MMM d, yyyy}") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Message">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("MessageTitle") %>' ToolTip="Open Message Outline page"
                    NavigateUrl='<%# Eval("MessageIdentifier", "/ui/admin/messages/outline?message={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Link" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("LinkText") %>
                <div class="text-body-secondary fs-sm"><%# Eval("LinkUrl") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="User" ItemStyle-Wrap="false">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>'><%# Eval("UserFullName") %></a>
                <div class="text-body-secondary fs-sm"><%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Browser" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("UserBrowser") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="IP Address" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("UserHostAddress") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>