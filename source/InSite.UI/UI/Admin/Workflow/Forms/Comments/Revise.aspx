<%@ Page Language="C#" CodeBehind="Revise.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Comments.Revise" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Comment" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">

                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Text
                                <insite:RequiredValidator runat="server" ControlToValidate="CommentText" FieldName="Text" ValidationGroup="Comment" />
                            </label>
                            <div>
                                <insite:MarkdownEditor runat="server" ID="CommentText" UploadControl="CommentUpload" />
                                <insite:EditorUpload runat="server" ID="CommentUpload" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Flag
                            </label>
                            <div>
                                <insite:ColorComboBox runat="server" ID="CommentFlag" CssClass="w-25" />
                            </div>
                        </div>

                    </div>

                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Resolved
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector runat="server" ID="CommentResolved" />
                            </div>
                        </div>

                    </div>

                </div>

            </div>
        </div>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Comment" />
        <insite:DeleteButton runat="server" ID="RemoveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
