<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistrictGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.DistrictGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="CreatorLink" Text="Add Division" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<insite:Grid runat="server" ID="Grid" GridLines="None" DataKeyNames="DivisionIdentifier">
    <Columns>
        <asp:TemplateField HeaderText="Division Name" >
            <ItemTemplate>
                <a href='<%# Eval("DivisionIdentifier", "/ui/cmds/admin/divisions/edit?id={0}") %>'>
                    <%# Eval("DivisionName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Departments" DataField="DepartmentCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />                
    </Columns>
</insite:Grid>
