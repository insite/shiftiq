<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.CommentsSection" %>

<div class="row mb-3">
    <div class="col-lg-7">
        <insite:AddButton runat="server" ID="AddCommentLink" Text="New Comment" />
        <insite:DownloadButton runat="server" ID="DownloadCommentsXlsx" />
    </div>
    <asp:Panel runat="server" DefaultButton="SearchInput" CssClass="col-lg-5">
        <insite:InputSearch runat="server" ID="SearchInput" />
    </asp:Panel>
</div>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>

        <h5 class="mb-0">
            <%# Eval("AuthorName") %>
            <div class="d-inline-block ms-3 fs-6">
                <span runat="server" class="me-3" visible='<%# (FlagType)Eval("Flag") != FlagType.None %>'>
                    <%# Eval("FlagIcon") %>
                </span>
                <span runat="server" class="badge bg-custom-default me-3" visible='<%# !string.IsNullOrEmpty((string)Eval("Category")) %>'>
                    <%# Eval("Category") %>
                </span>
                <span runat="server" class="badge bg-custom-default me-3" visible='<%# !string.IsNullOrEmpty((string)Eval("EventFormat")) %>'>
                    <%# Eval("EventFormat") %>
                </span>

                <a runat="server" visible='<%# CanWrite %>' href='<%# GetRedirectUrl("/admin/assessments/comments/delete?bank={0}&comment={1}&return=bank", Eval("BankID"), Eval("CommentID")) %>' title="Delete Comment" class="scroll-send"><i class="icon fas fa-trash-alt"></i></a>
                <a runat="server" visible='<%# CanWrite %>' href='<%# GetRedirectUrl("/ui/admin/assessments/comments/revise?bank={0}&comment={1}&return=bank", Eval("BankID"), Eval("CommentID")) %>' title="Revise Comment" class="scroll-send"><i class="icon fas fa-pencil"></i></a>
                <asp:LinkButton Visible='<%# Eval("QuestionVisible") %>' runat="server" ToolTip="Jump To Question" CommandName="JumpToQuestion" OnClientClick="SetTarget();" CommandArgument='<%# Eval("QuestionID") %>' Text="<i class='far fa-reply fa-rotate-270'></i>" />
            </div>
        </h5>

        <div class="form-text">
            <%# Eval("PostedOn") %>
        </div>

        <div class="mt-1 fw-light">
            Subject: <%# HttpUtility.HtmlEncode((string)Eval("Subject")) %>
        </div>

        <div class="mt-2">
            <%# Eval("Text") %>
        </div>

    </ItemTemplate>
    <SeparatorTemplate>
        <hr/>
    </SeparatorTemplate>
</asp:Repeater>

<script type = "text/javascript">
 function SetTarget() {
     document.forms[0].target = "_blank";
 }
</script>