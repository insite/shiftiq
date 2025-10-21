<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Identity.Permissions.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CreatePermission" />

    <section runat="server" id="PermissionPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-key me-1"></i>
            Permission
        </h2>

        <div class="row">
            <div class="col-md-7">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="form-group mb-3" runat="server" id="GroupField">
                            <label class="form-label">
                                Group
                                <insite:RequiredValidator runat="server" ControlToValidate="PermissionGroupIdentifier" FieldName="Group" ValidationGroup="CreatePermission" />
                            </label>
                            <div>
                                <insite:FindGroup ID="PermissionGroupIdentifier" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Access</label>
                            <div>
                                <asp:CheckBox runat="server" ID="AllowRead" Text="Read" Checked="true" /><br />
                                <asp:CheckBox runat="server" ID="AllowWrite" Text="Write" /><br />

                                <asp:CheckBox runat="server" ID="AllowCreate" Text="Create" /><br />
                                <asp:CheckBox runat="server" ID="AllowDelete" Text="Delete" /><br />

                                <asp:CheckBox runat="server" ID="AllowAdministrate" Text="Administrate" /><br />
                                <asp:CheckBox runat="server" ID="AllowConfigure" Text="Configure (Full Control)" />
                            </div>
                        </div>

                    </div>
        </div>

        </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CreatePermission" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>