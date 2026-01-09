<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Identities.Departments.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a title="Edit Department" href='/ui/admin/accounts/departments/edit?id=<%# Eval("DepartmentIdentifier") %>'><%# Eval("DepartmentName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Code">
            <ItemTemplate>
                <%# Eval("DepartmentCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# DateToHtml(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Division">
            <ItemTemplate>
                <a title="Edit Division" href='/ui/admin/accounts/divisions/edit?id=<%# Eval("DivisionIdentifier") %>'><%# Eval("DivisionName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organization">
            <ItemTemplate>
                <a title="Edit Organization" href='/ui/admin/accounts/organizations/edit?organization=<%# Eval("OrganizationIdentifier") %>'><%# Eval("OrganizationName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# Eval("DepartmentIdentifier", "/ui/admin/accounts/departments/edit?id={0}") %>' ToolTip="Edit" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>