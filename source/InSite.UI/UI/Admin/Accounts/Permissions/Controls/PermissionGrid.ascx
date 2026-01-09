<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermissionGrid.ascx.cs" Inherits="InSite.Admin.Accounts.Permissions.Controls.PermissionGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="AddButton" 
        ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Grant Permission" />
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="PermissionIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="False">
            <ItemTemplate>

                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# GetRedirectUrl("/ui/admin/accounts/permissions/edit?id={0}", Eval("PermissionIdentifier")) %>'
                />

                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Deny permission"
                    OnClientClick='<%# Eval("ObjectIdentifier", JsManagerName + ".deletePermission(\"{0}\"); return false;") %>'
                    Visible="<%# CanDelete %>" />

            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Object Type">
            <ItemTemplate>
                <%# Eval("ObjectType") %>
                <span class="text-body-secondary fs-xs"><%# Eval("ObjectSubtype") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Object Name">
            <ItemTemplate>
                <a href='<%# GetRedirectUrl("/ui/admin/platform/routes/edit?id={0}", Eval("ObjectIdentifier"))  %>'><%# Eval("ObjectName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Access">
            <ItemTemplate>
                <%# (bool)Eval("AllowExecute") ? "Execute" : string.Empty %>
                <%# (bool)Eval("AllowRead") ? "Read" : string.Empty %>
                <%# (bool)Eval("AllowWrite") ? "Write" : string.Empty %>
                <%# (bool)Eval("AllowCreate") ? "Create" : string.Empty %>
                <%# (bool)Eval("AllowDelete") ? "Delete" : string.Empty %>
                <%# (bool)Eval("AllowAdministrate") ? "Administrate" : string.Empty %>
                <%# (bool)Eval("AllowConfigure") ? "Configure" : string.Empty %>
                <%# (bool)Eval("AllowTrialAccess") ? "Trial" : string.Empty %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div class="d-none">
    <asp:Button ID="RefreshButton" runat="server" />
    <asp:Button ID="DeletePermissionButton" runat="server" />

    <asp:HiddenField runat="server" ID="DeleteIdentifier" />
</div>

<script type="text/javascript">

    (function () {
        var manager = window.<%= JsManagerName %> = window.<%= JsManagerName %> || {};

        manager.deletePermission = function (id) {

            if (!confirm("Are you sure you want to deny this permission?"))
                return;

            $("#<%= DeleteIdentifier.ClientID %>").val(id);

            __doPostBack('<%= DeletePermissionButton.UniqueID %>', '');
        };
    })();

</script>

