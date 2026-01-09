<%@ Control AutoEventWireup="true" CodeBehind="CommentPanel.ascx.cs" Inherits="InSite.Admin.Events.Comments.Controls.CommentPanel" Language="C#" %>

<div class="text-end pb-3">
    <insite:AddButton runat="server" ID="AddCommentButton" Text="New Comment" />
</div>

<section>

    <asp:Repeater runat="server" ID="Repeater">
        <ItemTemplate>

            <h4 class="mt-0 mb-0">
                <%# GetAuthorName() %>
            </h4>
            <div class="form-text mt-1 mb-2">
                <%# GetTimestamp() %>
            </div>

            <div class="float-end" runat="server" id="CommandsBlock">
                <insite:IconLink ID="EditCommentButton" runat="server" Name="pencil" ToolTip="Revise Comment" />
                <insite:IconLink ID="DeleteCommentButton" runat="server" Name="trash-alt" ToolTip="Delete Comment" />
            </div>

            <div class="pe-5" style="white-space:pre-wrap;">
                <%# Eval("CommentText") %>
            </div>
                
            <hr/>

        </ItemTemplate>
    </asp:Repeater>

</section>
