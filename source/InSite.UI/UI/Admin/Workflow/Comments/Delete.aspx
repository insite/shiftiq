<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Issues.Comments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <div runat="server" id="Desktop">

        <insite:Alert runat="server" ID="EditorStatus" />

        <div class="row settings">
            <div class="col-md-6">
                
                <h3>Comment</h3>

                <dl class="row">

                    <dd class="col-sm-12">
                        <asp:Literal runat="server" ID="CommentText" />
                    </dd>

                    <dt runat="server" id="AuthorDiv" class="col-sm-3">Author</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="CommentAuthor" />
                    </dd>

                    <dt runat="server" id="PostedDiv" class="col-sm-3">Posted</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="CommentPosted" />
                    </dd>

                </dl>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this comment?
                </div>

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>
            </div>

             <div class="col-md-6">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The comment will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>
            </div>
        </div>
    </div>
</asp:Content>
