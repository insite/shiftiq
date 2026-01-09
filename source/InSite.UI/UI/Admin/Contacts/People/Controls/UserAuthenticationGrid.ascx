<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAuthenticationGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.UserAuthenticationGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Date/Time">
            <ItemTemplate>
                <%# GetDateString((DateTimeOffset)Eval("SessionStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Authentication">
            <ItemTemplate>
                <%# (bool)Eval("SessionIsAuthenticated") ? "Succeeded" : "Failed" %> <%# Eval("AuthenticationErrorType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Browser">
            <ItemTemplate>
                <%# Eval("UserBrowser") %> <%# Eval("UserBrowserVersion") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="IP Address">
            <ItemTemplate>
                <%# Eval("UserHostAddress") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Language">
            <ItemTemplate>
                <%# Eval("UserLanguage") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organization">
            <ItemTemplate>
                <%# GetOrganizationCode((Guid)Eval("OrganizationIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
