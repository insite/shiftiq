<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttemptCommentsResults.ascx.cs" Inherits="InSite.Admin.Assessments.Reports.Controls.SubmissionCommentaryResults" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .attempt-question-text img {
            max-width: 100%;
        }
    </style>
</insite:PageHeadContent>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Question">
            <itemtemplate>
                <span style="white-space: pre-wrap;"><%# Eval("QuestionSequence") %></span>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Question Content">
            <itemtemplate>
                <span class="attempt-question-text" style="white-space: pre-wrap;"><%# Shift.Common.Markdown.ToHtml((string)Eval("QuestionText")) %></span>
                <div class="form-text"><%# Eval("FormTitle") %></div>
                <div class="form-text">Form Asset #<%# Eval("FormAssetNumber") %></div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Learner Comment">
            <itemtemplate>
                <span style="white-space: pre-wrap;"><%# Eval("CommentText") %></span>
                <div class="form-text">Posted by <%# Eval("AuthorName") %> on <%# TimeZones.Format((DateTimeOffset)Eval("CommentPosted"), CurrentSessionState.Identity.User.TimeZone) %> </div>
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
