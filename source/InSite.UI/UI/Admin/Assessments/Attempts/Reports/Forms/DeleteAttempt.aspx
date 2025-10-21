<%@ Page Language="C#" CodeBehind="DeleteAttempt.aspx.cs" Inherits="InSite.UI.Admin.Attempts.Reports.Forms.DeleteAttempt" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
        <div class="alert alert-danger" role="alert">
             <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />	
        </div>	
    </asp:Panel>

    <div class="row">
        <div class="col-lg-6">
            <div class="card mb-3">
                <div class="card-body">
                    <h3>Attempt</h3>

                    <dl class="row">
                        <dt class="col-sm-4">Started</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptStarted" /></dd>
                    
                        <dt class="col-sm-4">Completed</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptGraded" /></dd>
                    
                        <dt class="col-sm-4">Time Taken</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptTimeTaken" /></dd>
                    
                        <dt class="col-sm-4">Grade</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptIsPassing" /></dd>
                    
                        <dt class="col-sm-4">Score</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptScore" /></dd>
                    
                        <dt class="col-sm-4">Points</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="AttemptPoints" /></dd>

                        <dt class="col-sm-4">Candidate Name</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="FullName" /></dd>

                        <dt class="col-sm-4">Bank Name</dt>
                        <dd class="col-sm-8"><asp:Literal runat="server" ID="BankName" /></dd>
                    </dl>
                </div>
            </div>

            <div runat="server" id="ConfirmMessage" class="alert alert-danger mb-3" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this attempt?	
            </div>	

            <p class="mb-3">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />
            </p>	
        </div>

        <div class="col-lg-6">
            <div class="card">
                <div class="card-body">
                    <h3>Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The attempt will be deleted from all forms, queries, and reports.
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
                                Attempts
                            </td>
                            <td>
                                1
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Answers
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="AnswerCount" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div> 
</asp:Content>
