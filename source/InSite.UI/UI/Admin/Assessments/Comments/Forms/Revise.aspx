<%@ Page Language="C#" CodeBehind="Revise.aspx.cs" Inherits="InSite.Admin.Assessments.Comments.Forms.Revise" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="SubjectOutput" Src="~/UI/Admin/Assessments/Comments/Controls/SubjectOutputDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="SubjectInput" Src="~/UI/Admin/Assessments/Comments/Controls/SubjectInputDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Comment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-comment"></i>
            Revise Comment
        </h2>

        <div class="row">

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Comment</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Text
                                <insite:RequiredValidator runat="server" FieldName="Text" ControlToValidate="CommentText" ValidationGroup="Comment" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CommentText" TextMode="MultiLine" Rows="4" />
                            </div>
                            <div class="form-text">
                                The body/text for the comment.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Category
                            </label>
                            <div>
                                <insite:FeedbackCategoryComboBox runat="server" ID="FeedbackCategory" />
                            </div>
                            <div class="form-text">
                                Select a category for the comment.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Flag</label>
                            <div>
                                <insite:FlagComboBox runat="server" ID="CommentFlag" AllowBlank="false" />
                            </div>
                            <div class="form-text">
                                Assign a coloured flag to the comment.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Author Type
                                <insite:RequiredValidator runat="server" ControlToValidate="CommentAuthorType" ValidationGroup="Comment" />
                            </label>
                            <div>
                                <insite:CommentAuthorTypeComboBox runat="server" ID="CommentAuthorType" AllowBlank="false" />
                            </div>
                        </div>

                    </div>

                </div>

            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Subject</h3>
                        <uc:SubjectOutput runat="server" ID="SubjectOutput" />
                        <uc:SubjectInput runat="server" ID="SubjectInput" />

                    </div>

                </div>

            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Optional Metadata</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Instructor / Training Provider</label>
                            <div>
                                <insite:FindGroup runat="server" ID="Instructor" />
                            </div>
                            <div class="form-text">
                                The instructor or training provider to whom the comment pertains.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Exam Event Date</label>
                            <div>
                                <insite:TextBox runat="server" ID="EventDate" />
                            </div>
                            <div class="form-text">
                                The date of the exam event to which the comment pertains.
                            Please use the format <strong>Use the format MMM d, yyyy</strong>.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Exam Event Format</label>
                            <div>
                                <insite:EventFormatComboBox runat="server" ID="EventFormat" />
                            </div>
                            <div class="form-text">
                                The format of the exam event to which the comment pertains.
                            </div>
                        </div>


                    </div>

                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Comment" />
            <insite:DeleteButton runat="server" ID="RemoveButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
