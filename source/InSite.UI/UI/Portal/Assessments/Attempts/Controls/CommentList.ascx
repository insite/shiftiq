<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentList.ascx.cs" Inherits="InSite.Portal.Assessments.Attempts.Controls.CommentList" %>

<asp:Repeater runat="server" ID="CommentRepeater">
    <ItemTemplate>
        <hr runat="server" visible='<%# Container.ItemIndex != 0 %>' />

        <h3>Question <%# Eval("QuestionSequence") %></h3>
        <div class="q-text"><%# Eval("QuestionTitle") %></div>
        <small class="text-body-secondary" style="margin-top: -10px;"><%# Eval("CommentPosted") %></small>
        <div class="commands">
            <a href="#edit-feedback" title="Edit Feedback"
               data-action="edit-feedback" data-question='<%# Eval("QuestionSequence") %>'
               onclick="feedbackList.onEditFeedback(event, this); return false;">
                <i class="fas fa-pencil"></i>
            </a>
        
            <a href="#delete-feedback" title="Delete Feedback"
               data-action="delete-feedback" data-question='<%# Eval("QuestionSequence") %>'
               onclick="feedbackList.onDeleteFeedback(event, this); return false;">
                <i class="fas fa-trash-alt"></i>
            </a>
        </div>
        <div class="pt-4"><%# Eval("CommentText") %></div>
    </ItemTemplate>
</asp:Repeater>

<div runat="server" id="NoComments">You have not posted feedback on any of the questions in this exam.</div>