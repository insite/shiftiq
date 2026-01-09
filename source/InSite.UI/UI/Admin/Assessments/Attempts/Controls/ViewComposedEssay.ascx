<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewComposedEssay.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.ViewComposedEssay" %>

<%@ Register TagPrefix="uc" TagName="ComposedRepeater" Src="~/UI/Admin/Assessments/Attempts/Controls/ViewComposed.ascx" %>

<uc:ComposedRepeater runat="server" ID="ComposedRepeater">
    <AnswerTemplate>
        <%# Markdown.ToHtml(Container.DataItem.AnswerText) %>
    </AnswerTemplate>
</uc:ComposedRepeater>