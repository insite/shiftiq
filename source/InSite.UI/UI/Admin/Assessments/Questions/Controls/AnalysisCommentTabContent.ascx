<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisCommentTabContent.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.AnalysisCommentTabContent" %>

<%@ Register TagPrefix="uc" TagName="CommentRepeater" Src="CommentRepeater.ascx" %>

<asp:Literal runat="server" ID="MessageLiteral" />

<div class="row">
    <div runat="server" id="AdministratorColumn" class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">
                <h3>Administrator Comments</h3>
                <uc:CommentRepeater runat="server" ID="AdministratorRepeater" AllowEdit="false" />
            </div>
        </div>
    </div>
    <div runat="server" id="CandidateColumn" class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">
                <h3>By Candidate</h3>
                <uc:CommentRepeater runat="server" ID="CandidateRepeater" AllowEdit="false" ShowAuthor="false" AllowHide="false" />
            </div>
        </div>
    </div> 
</div>