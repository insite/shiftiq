<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Developers.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="TokenIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Developer">
            <ItemTemplate>
                <a title="Edit User" href='/ui/admin/accounts/developers/edit?token=<%# Eval("TokenIdentifier") %>'><%# Eval("DeveloperName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiry">
            <ItemTemplate>
                <%# DateToHtml(Eval("TokenExpiry")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Organizations" DataField="OrganizationCode" />

    </Columns>
</insite:Grid>

