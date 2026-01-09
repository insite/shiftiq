<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Utilities.Actions.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Action" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="DetailPanel" Title="Details" Icon="fa fa-location" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:Detail ID="Detail" runat="server" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PermissionPanel" Title="Permissions" Icon="far fa-key me-1" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-lg-12">
                            <insite:Button runat="server" ID="AddPermissionButton" ButtonStyle="Default" Text="Add Permission" Icon="fas fa-plus-circle" />
                        </div>
                    </div>

                    <asp:Repeater runat="server" ID="PermissionRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>

                                    <a href='/ui/admin/identity/permissions/create?action=<%# Eval("ActionIdentifier") %>&group=<%# Eval("GroupIdentifier") %>'><i class="icon fas fa-pencil"></i></a>

                                    <insite:IconButton runat="server" ID="DeletePermission"
                                        CommandName="Delete" Name="trash-alt"
                                        ConfirmText="Are you sure you want to delete this permission?"
                                        CommandArgument='<%# Eval("GroupIdentifier") %>'
                                        ToolTip="Delete Permission" />

                                </td>
                                <td>
                                    <span class='badge badge-<%# (string)Eval("GroupType") == "Role" ? "default" : "warning" %>'><%# Eval("GroupType") %></span>
                                </td>
                                <td>
                                    <a href='/ui/admin/contacts/groups/edit?contact=<%# Eval("GroupIdentifier") %>'>
                                        <%# Eval("GroupName") %> 
                                    </a>
                                </td>
                                <td>
                                    <%# (bool)Eval("AllowRead") ? "Read" : string.Empty %>
                                    <%# (bool)Eval("AllowWrite") ? "Write" : string.Empty %>
                                    <%# (bool)Eval("AllowCreate") ? "Create" : string.Empty %>
                                    <%# (bool)Eval("AllowDelete") ? "Delete" : string.Empty %>
                                    <%# (bool)Eval("AllowAdministrate") ? "Administrate" : string.Empty %>
                                    <%# (bool)Eval("AllowFullControl") ? "FullControl" : string.Empty %>
                                </td>

                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SubActionPanel" Title="Subactions" Icon="far fa-location" IconPosition="BeforeText">
            <section>
                
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="ActionRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>URL</th>
                                            <th>Name</th>
                                            <th>Icon</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Url") %>' Text='<%# Eval("ActionUrl") %>' />
                                    </td>
                                    <td>
                                        <%# Eval("ActionName") %>
                                        <div runat="server" visible='<%# Eval("Note") != null %>' class="text-muted fs-sm"><%# Eval("Note") %></div>
                                    </td>
                                    <td><%# ValueConverter.IsNull(Eval("ActionIcon")) ? null : string.Format("<i class='{0}'></i> <span>{0}</span>", Eval("ActionIcon")) %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
    <div class="row mb-3">
        <div class="col-md-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Action" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
        <div class="col-md-6 text-end">
            <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this Action?" />
        </div>
    </div>
</asp:Content>
