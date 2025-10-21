<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Departments.DepartmentGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="CreatorLink" Text="Add Department" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<insite:Grid runat="server" ID="Grid" GridLines="None"  DataKeyNames="DepartmentIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Department Name">
            <ItemTemplate>
                <a href='<%# Eval("DepartmentIdentifier", "/ui/cmds/admin/departments/edit?id={0}") %>'>
                    <%# Eval("DepartmentName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Division">
            <ItemTemplate>
                <a href='<%# "/ui/cmds/admin/divisions/edit?id=" + Eval("DivisionIdentifier") %>'>
                    <%# Eval("DivisionName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Description" DataField="DepartmentDescription" />
        <asp:BoundField HeaderText="Users" DataField="UserCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Profiles" DataField="ProfileCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Competencies" DataField="CompetencyCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                
    </Columns>
</insite:Grid>
