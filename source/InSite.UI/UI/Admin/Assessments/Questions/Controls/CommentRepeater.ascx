<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.CommentRepeater" %>

<div class="row mb-3">
    <div class="col-lg-7">
        <insite:AddButton runat="server" ID="AddCommentLink" Text="New Comment" Visible="false" />
    </div>
</div>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>
        <div class="question-comment border-bottom pb-3 mb-3 mx-2<%# (bool)Eval("IsHidden") ? " comment-hidden" : string.Empty %>">
            <div class="row">
                <div class="col-md-11">
                    <div runat="server" class="fs-5" visible='<%# ShowAuthor %>'>
                        <%# Eval("Author") %>
                    </div>
                    <div class="form-text">
                        <%# Eval("Posted") %>
                    </div>

                    <div class="comment-text"><%# Eval("Text") %></div>
                </div>
                <div class="col-md-1 text-end">
                    <div class="fs-6 mb-1">
                        <%# Eval("IconHtml") %>
                    </div>
                    <insite:Container runat="server" visible='<%# AllowEdit %>'>
                        <a href='<%# GetRedirectUrl("/ui/admin/assessments/comments/revise?bank={0}&comment={1}", Eval("BankID"), Eval("CommentID")) %>' title="Revise Comment" class="scroll-send d-block mb-1"><i class="icon fas fa-pencil"></i></a>
                        <a runat="server" data-action="hide" data-id='<%# Eval("CommentID") %>' visible='<%# AllowHide %>' class="d-block mb-1"><i class="icon fas"></i></a>
                        <a href='<%# GetRedirectUrl("/admin/assessments/comments/delete?bank={0}&comment={1}", Eval("BankID"), Eval("CommentID")) %>' title="Delete Comment" class="scroll-send d-block mb-1"><i class="icon fas fa-trash-alt"></i></a>
                    </insite:Container>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageHeadContent runat="server" ID="CommonStyle" RenderRequired="true">
    <style type="text/css">
        .question-comment { position: relative; }

        .question-comment .comment-text { white-space: pre-wrap; }
    </style>
</insite:PageHeadContent>