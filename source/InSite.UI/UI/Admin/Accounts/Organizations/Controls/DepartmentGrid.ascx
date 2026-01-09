<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentGrid.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.DepartmentGrid" %>

<p runat="server" ID="EmptyGrid" class="help">This organization has no departments.</p>

<insite:Grid runat="server" ID="Grid">

    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href='<%# Eval("DepartmentIdentifier", "/ui/admin/accounts/departments/edit?id={0}") %>'><%# Eval("DepartmentName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Code">
            <ItemTemplate>
                <%# Eval("DepartmentCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# LocalDate(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Division">
            <ItemTemplate>
                <a href='<%# Eval("DivisionIdentifier", "/ui/admin/accounts/divisions/edit?id={0}") %>'><%# Eval("DivisionName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</insite:Grid>
