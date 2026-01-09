<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Categories.CategoryGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="CreatorLink" Text="Add Category" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="CategoryIdentifier">
    <Columns>
        <asp:BoundField HeaderText="Achievement Type" DataField="AchievementLabel"></asp:BoundField>
        <asp:TemplateField HeaderText="Category Name">
            <ItemTemplate>
                <a href='<%# "/ui/cmds/admin/organizations/categories/edit?id=" + Eval("CategoryIdentifier") %>'>
                    <%# Eval("CategoryName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-Width="30px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Delete" 
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to delete this category?" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
