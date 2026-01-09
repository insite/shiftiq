<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Utilities.Actions.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Action" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-location me-1"></i>
            Action
        </h2>

        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Toolkit and Package
                                <insite:RequiredValidator runat="server" ControlToValidate="PermissionSelector" ValidationGroup="Action" />
                            </label>
                            <div class="row">
                                <div class="col-md-6 mb-2 mb-md-0">
                                    <insite:FindPermission runat="server" ID="PermissionSelector" />
                                </div>
                                <div class="col-md-6">
                                    <insite:TextBox runat="server" ID="PackageName" />
                                </div>
                            </div>
                            <div class="form-text">
                                This action must be assigned to a specific toolkit.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Action URL
                                <insite:RequiredValidator runat="server" ControlToValidate="ActionUrlInput" ValidationGroup="Action" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ActionUrlInput" MaxLength="500" />
                            </div>
                            <div class="form-text">Uniquely identifies an application page URL.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Action Title
                                <insite:RequiredValidator runat="server" ControlToValidate="ActionTitle" ValidationGroup="Action" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ActionTitle" MaxLength="100" />
                            </div>
                            <div class="form-text">User-friendly title for this action.</div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row mb-3">
        <div class="col-md-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Action" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>
