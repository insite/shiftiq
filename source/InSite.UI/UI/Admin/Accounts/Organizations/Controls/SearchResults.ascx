<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("OrganizationIdentifier", "/ui/admin/accounts/organizations/edit?organization={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# GetAccountStatusHtml(Eval("AccountStatus")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a title="Edit Organization" href='<%# Eval("OrganizationIdentifier", "/ui/admin/accounts/organizations/edit?organization={0}") %>'><%# Eval("CompanyName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Code" HeaderStyle-Wrap="False" DataField="OrganizationCode" />
        <asp:BoundField HeaderText="Domain" DataField="CompanyDomain" />
        <asp:BoundField HeaderText="Opened" DataField="AccountOpened" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Wrap="false" />
        <asp:BoundField HeaderText="Closed" DataField="AccountClosed" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Wrap="false" />

    </Columns>
</insite:Grid>