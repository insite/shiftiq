<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Contacts.Reports.Employers.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Employer Name">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("EmployerGroupName") %>'
                    NavigateUrl='<%# Eval("EmployerGroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Employer Category">
            <ItemTemplate>
                <%# Eval("EmployerGroupCategory") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="District">
            <ItemTemplate>
                <%# Eval("EmployerDistrictName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employees" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
            <ItemTemplate>
                <%# Eval("EmployeeCount") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>