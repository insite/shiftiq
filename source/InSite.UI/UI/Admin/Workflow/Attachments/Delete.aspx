<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Issues.Attachments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Attachment" />

    <div class="row settings">
        <div class="col-md-6">
            <div class="settings">
                <h3>Attachment</h3>
                
                <dl class="row">
                    <dt runat="server" id="FileNameDiv" class="col-sm-3">File</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="FileNameOutput" />
                    </dd>

                    <dt runat="server" id="PostedOnDiv" class="col-sm-3">Posted On</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="PostedOnOutput" />
                    </dd>

                    <dt runat="server" id="PostedByDiv" class="col-sm-3">Posted By</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="PostedByOutput" />
                    </dd>

                    <dt runat="server" id="IssueDiv" class="col-sm-3">Case Number</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="IssueNumber" />
                    </dd>

                    <dt runat="server" id="TitleDiv" class="col-sm-3">Case Title</dt>
                    <dd class="col-sm-9">
                        <asp:Literal runat="server" ID="IssueTitle" />
                    </dd>
                </dl>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this attachment from the case?
                </div>

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The attachment will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
        </div>
    </div>
</div>
</asp:Content>
