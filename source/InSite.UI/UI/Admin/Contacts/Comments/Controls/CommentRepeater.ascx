<%@ Control AutoEventWireup="true" CodeBehind="CommentRepeater.ascx.cs" Inherits="InSite.Admin.Contacts.Comments.Controls.CommentRepeater" Language="C#" %>

<div class="mb-3">
    <insite:AddButton ID="AddCommentButton" runat="server" Text="New Comment" />
    <insite:DownloadButton ID="DownloadXlsxButton" runat="server" />
</div>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>

        <strong class="mb-1">
            <%# GetAuthorName(Container.DataItem) %><span class="fs fs-sm text-body-secondary ms-2 fw-normal"><%# GetTimestamp(Container.DataItem) %></span>
            <span class="ms-2">
                <%# Eval("CommentCategoryHtml") %>
                <%# Eval("CommentFlagHtml") %>
            </span>
        </strong>

        <div>
            <%# GetCommentHtml(Container.DataItem) %>
        </div>
                
        <div>
            <a runat="server" href='<%# GetRedirectUrl("/ui/admin/contacts/comments/revise?contact={0}&comment={1}", UserIdentifier, Eval("CommentIdentifier")) %>' title="Revise Comment"><i class="icon fas fa-pencil"></i></a>
            <a runat="server" href='<%# GetRedirectUrl("/ui/admin/contacts/comments/delete?contact={0}&comment={1}", UserIdentifier, Eval("CommentIdentifier")) %>' title="Remove Comment"><i class="icon fas fa-trash-alt"></i></a>
        </div>

    </ItemTemplate>
    <SeparatorTemplate><hr class="my-3" /></SeparatorTemplate>
</asp:Repeater>