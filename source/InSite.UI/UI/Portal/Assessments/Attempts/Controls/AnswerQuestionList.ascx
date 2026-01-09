<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnswerQuestionList.ascx.cs" Inherits="InSite.UI.Portal.Assessments.Attempts.Controls.AnswerQuestionList" %>

<%@ Register TagPrefix="uc" TagName="QuestionOutput" Src="AnswerQuestionOutput.ascx" %>

<asp:Repeater runat="server" ID="SectionRepeater">
    <ItemTemplate>
        <h3 runat="server" visible='<%# HasContent("Title") %>'><%# GetContentText("Title") %></h3>
        <div runat="server" visible='<%# HasContent("Summary") %>' class="mt-2 mb-4"><%# GetContentHtml("Summary") %></div>
        <asp:Repeater runat="server" ID="QuestionRepeater">
            <ItemTemplate>
                <uc:QuestionOutput runat="server" ID="Output" />
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
