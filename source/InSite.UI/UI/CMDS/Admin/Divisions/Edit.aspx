<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Districts.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Division" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-industry me-1"></i>
            Division
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="settings">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Division Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="Name" FieldName="Name" ValidationGroup="Division" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="Name" MaxLength="90" />
                                </div>
                                <div class="form-text">The name of the division, district, or business unit.</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row mb-3">
        <div class="col-md-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Division" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
        <div class="col-md-6 text-end">
            <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete division?" />
        </div>
    </div>
</asp:Content>
