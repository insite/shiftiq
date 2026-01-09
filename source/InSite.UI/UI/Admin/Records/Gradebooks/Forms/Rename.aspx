<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Gradebook" />

    <section runat="server" ID="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Rename Gradebook
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Title
                                <insite:RequiredValidator runat="server" ControlToValidate="GradebookTitle" FieldName="Title" ValidationGroup="Gradebook" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="GradebookTitle" MaxLength="400" />
                            </div>
                            <div class="form-text">The descriptive title for this gradebook.</div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Gradebook" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
