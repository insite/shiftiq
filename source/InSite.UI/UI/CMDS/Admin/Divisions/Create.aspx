<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Districts.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="District" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-industry me-1"></i>
            District Information
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="settings">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="Name" FieldName="Name" ValidationGroup="District" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="Name" MaxLength="90" />
                                </div>
                                <div class="form-text">The name of the district.</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="District" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
