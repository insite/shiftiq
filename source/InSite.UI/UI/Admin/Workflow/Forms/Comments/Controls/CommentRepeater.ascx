<%@ Control Language="C#" CodeBehind="CommentRepeater.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Comments.Controls.CommentRepeater" %>

<div class="float-end text-end mb-3">
    <insite:AddButton ID="AddCommentButton" runat="server" Text="New Comment" />
</div>

<div class="clearfix" ></div>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>

        <strong class="mb-1">
            <%# Eval("AuthorName") %><span class="fs fs-sm text-body-secondary ms-2 fw-normal"><%# Eval("TimestampHtml") %></span>
            <span class="ms-2">
                <%# Eval("CommentFlagHtml") %>
            </span>
        </strong>

        <div>
            <%# Eval("CommentHtml") %>
        </div>

        <div>
            <a runat="server" href='<%# Eval("ReviseUrl") %>' title="Revise Comment"><i class="icon fas fa-pencil"></i></a>
            <a runat="server" href='<%# Eval("DeleteUrl") %>' title="Remove Comment"><i class="icon fas fa-trash-alt"></i></a>
        </div>

    </ItemTemplate>
    <SeparatorTemplate>
        <hr class="my-3" />
    </SeparatorTemplate>
</asp:Repeater>