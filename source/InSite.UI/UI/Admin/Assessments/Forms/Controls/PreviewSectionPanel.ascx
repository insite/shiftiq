<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PreviewSectionPanel.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Forms.Controls.PreviewSectionPanel" %>

<%@ Register TagPrefix="uc" TagName="QuestionPreviewPanel" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionPreviewPanel.ascx" %>

<div runat="server" id="Summary" visible='<%# (bool)Eval("HasSummary") %>' class="my-3"><%# Eval("Summary") %></div>

<asp:Repeater runat="server" ID="QuestionRepeater">
    <ItemTemplate>
        <uc:QuestionPreviewPanel runat="server" ID="PreviewPanel" />
    </ItemTemplate>
</asp:Repeater>
