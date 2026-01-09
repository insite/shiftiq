<%@ Page Language="C#" CodeBehind="Content.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Content" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section runat="server" id="Section1" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit me-1"></i>
            Change Content
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Assessment" />

                <div class="row mt-5">
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label class="form-label">Diagram Book</label>
                            <div>
                                <asp:CheckBox runat="server" ID="HasDiagrams" Text="This form contains questions that require a diagram book" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Reference Materials for Online Sessions</label>
                            <div>
                                <insite:ComboBox runat="server" ID="HasReferenceMaterials" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
