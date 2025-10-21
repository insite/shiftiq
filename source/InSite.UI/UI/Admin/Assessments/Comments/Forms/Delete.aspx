<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Comments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="SubjectOutput" Src="../Controls/SubjectOutputDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row settings">
        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <h3>Comment</h3>

                    <dl class="row">
                        <dt class="col-sm-3">Text</dt>
                        <dd class="col-sm-9" style="white-space:pre-wrap;"><asp:Literal runat="server" ID="CommentText" /></dd>
            
                        <dt class="col-sm-3">Flag</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="CommentFlag" /></dd>
            
                        <dt class="col-sm-3">Category</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="FeedbackCategory" /></dd>
                    </dl>

                    <uc:SubjectOutput runat="server" ID="SubjectOutput" />
                </div>
            </div>

            <div class="alert alert-danger">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this comment?
            </div>

            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <h3>Impact</h3>

                    <div class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The comment will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>

                    <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                        <tr>
                            <td>
                                Type
                            </td>
                            <td>
                                Rows
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Comments
                            </td>
                            <td>
                                1
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
