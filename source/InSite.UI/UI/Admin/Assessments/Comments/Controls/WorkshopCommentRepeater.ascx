<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkshopCommentRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Comments.Controls.WorkshopCommentRepeater" %>

<%@ Import Namespace="Humanizer" %>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>

        <h5 class="mb-0">
            <%# Eval("Author.FullName") %>
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

                <a href='<%# GetRedirectUrl("/admin/assessments/comments/delete?bank={0}&comment={1}", Eval("BankID"), Eval("CommentID")) %>' title="Delete Comment" class="scroll-send"><i class="icon fas fa-trash-alt"></i></a>
                <a href='<%# GetRedirectUrl("/ui/admin/assessments/comments/revise?bank={0}&comment={1}", Eval("BankID"), Eval("CommentID")) %>' title="Revise Comment" class="scroll-send"><i class="icon fas fa-pencil"></i></a>
            </div>
        </h5>

        <div class="form-text">
            posted <%# TimeZoneInfo.ConvertTime((DateTimeOffset)Eval("PostedOn"), User.TimeZone).Humanize() %>
        </div>

        <div class="mt-1 fw-light">
            Subject: <%# HttpUtility.HtmlEncode(Eval("Subject")) %>
        </div>

        <div class="mt-2">
            <%# Eval("Text") %>
        </div>
                
    </ItemTemplate>
    <SeparatorTemplate>
        <hr/>
    </SeparatorTemplate>
</asp:Repeater>
