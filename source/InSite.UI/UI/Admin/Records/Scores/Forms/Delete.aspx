<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Scores.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

        
    <div class="row settings">
        <div class="col-md-6">
            <h3>Score</h3>

            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ItemName" /></dd>

                <dt class="col-sm-3">Type</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ScoreType" /></dd>
            </dl>

            <dl class="row" runat="server" id="OptionField">
                <dt class="col-sm-3"><asp:Literal runat="server" ID="OptionName" /></dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="OptionValue" /></dd>
            </dl>

            <dl class="row" runat="server" id="GradeField">
                <dt class="col-sm-3">Grade</dt>
                <dd class="col-sm-9" runat="server" id="ScorePercentField"><asp:Literal runat="server" ID="ScorePercent" /> %</dd>

                <dd class="col-sm-12" runat="server" id="ScorePointField">
                    <asp:Literal runat="server" ID="ScorePoint" />
                    <asp:Literal runat="server" ID="OutOfLiteral" />
                </dd>

                <dt class="col-sm-3" runat="server" id="ScoreTextField"><asp:Literal runat="server" ID="ScoreText" /></dt>
                <dd class="col-sm-9" runat="server" id="ScoreNumberField"><asp:Literal runat="server" ID="ScoreNumber" /></dd>
            </dl>

            <dl class="row" runat="server" id="DateGradedField">
                <dt class="col-sm-3">Date Graded</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Graded" /></dd>
            </dl>

            <dl class="row" runat="server" id="ProgressStatusField">
                <dt class="col-sm-3">Status</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgressStatus" /></dd>
            </dl>
                        
            <asp:Panel runat="server" ID="ProgressCompletedPanel">
                <dl class="row">
                    <dt class="col-sm-3">Completed</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgressCompleted" /></dd>
                
                    <dt class="col-sm-3">Elapsed Seconds</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="ElapsedSeconds" /></dd>
                
                    <dt class="col-sm-3">Fail or Pass?</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgressFailOrPass" /></dd>
                
                    <dt class="col-sm-3">Percent</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="CompletedProgressPercent" /></dd>
                </dl>
            </asp:Panel>

            <dl class="row" runat="server" id="ProgressStartedField">
                <dt class="col-sm-3">Started</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgressStarted" /></dd>
            </dl>

            <dl class="row">
                <dt class="col-sm-3">Comment</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Comment" /></dd>

                <dt class="col-sm-3">User Name</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="UserName" /></dd>

                <dt class="col-sm-3">Gradebook Title</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="GradebookTitle" /></dd>
            </dl>

            <div class="alert alert-danger" role="alert" runat="server" ID="ConfirmText">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this score?
            </div>

            <p>
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" Identifier="Cancel" />
            </p>
        </div>

        <div class="col-md-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The score will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
        </div>
    </div>

</div>
</asp:Content>
