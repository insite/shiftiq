<%@ Page Language="C#" CodeBehind="Revise.aspx.cs" Inherits="InSite.Admin.Contacts.Comments.Forms.Revise" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CommentTextEditor" Src="Controls/CommentTextEditor.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Comment" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">

                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Text
                                <span class="text-danger">*</span>
                            </label>
                            <div>
                                <uc:CommentTextEditor runat="server" ID="CommentText" />
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

                        <div class="form-group mb-3 d-none">
                            <label class="form-label">
                                Options
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="CommentIsPrivate" Text="Private" ToolTip="Private comments are not released to learners in the portal" />
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
