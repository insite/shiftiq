<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Comments.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row">

        <div class="col-md-6">
            <h3>Comment</h3>

            <dl class="row">
                <dd class="col-sm-12"><asp:Literal runat="server" ID="CommentText" /></dd>
                
                <dt class="col-sm-3">Author</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="CommentAuthor" /></dd>
                
                <dt class="col-sm-3">Posted</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="CommentPosted" /></dd>

                <dt class="col-sm-3">Form Name</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="SurveyName" /></dd>
            </dl>

            <div class="alert alert-danger">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this comment?
            </div>

            <insite:Button runat="server" ID="RemoveButton" Text="Delete" Icon="fas fa-trash-alt" ButtonStyle="Danger" />
            <insite:Button runat="server" ID="CancelButton" ButtonStyle="Default" Text="Cancel" Icon="fas fa-ban" />
        </div>

        <div class="col-lg-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The comment will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
            
            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>
                        Comment
                    </td>
                    <td>
                        1
                    </td>
                </tr>
            </table>
        </div>

    </div>
</asp:Content>
