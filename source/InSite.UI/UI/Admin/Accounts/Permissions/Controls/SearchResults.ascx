<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Permissions.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid DataKeyNames="PermissionIdentifier" runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetPermissionEditHtml(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Object" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# GetObjectHtml(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Object Type" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("ObjectType") %>
                <span class="text-body-secondary fs-sm"><%# Eval("ObjectSubtype") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group">
            <ItemTemplate>
                <a title="Edit Group" href="/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>"><%# Eval("GroupName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Type" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("GroupType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Access">
            <ItemTemplate>
                
                <%# (bool)Eval("AllowExecute") ? "Execute" : "" %>

                <%# (bool)Eval("AllowRead") ? "Read" : "" %>
                <%# (bool)Eval("AllowWrite") ? "Write" : "" %>
                <%# (bool)Eval("AllowCreate") ? "Create" : "" %>
                <%# (bool)Eval("AllowDelete") ? "Delete" : "" %>
                <%# (bool)Eval("AllowAdministrate") ? "Administrate" : "" %>
                <%# (bool)Eval("AllowConfigure") ? "Configure" : "" %>
                <%# (bool)Eval("AllowTrialAccess") ? "Trial" : "" %>
                
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>