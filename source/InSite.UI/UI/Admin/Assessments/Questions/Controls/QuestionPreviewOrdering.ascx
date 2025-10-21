<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewOrdering.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionPreviewOrdering" %>

<div class="form-group ordering-list">
    <asp:Literal runat="server" ID="TopLabel" />
    <asp:Repeater runat="server" ID="OrderingOptionRepeater">
        <HeaderTemplate><div class="mb-3 ordering-list-container"></HeaderTemplate>
        <FooterTemplate></div></FooterTemplate>
        <ItemTemplate>
            <div class="bg-white border rounded py-2 px-3 mb-3">
                <%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Literal runat="server" ID="BottomLabel" />
</div>
