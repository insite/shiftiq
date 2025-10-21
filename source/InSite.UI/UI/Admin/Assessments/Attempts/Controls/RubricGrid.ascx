<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricGrid.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.RubricGrid" %>

<asp:Repeater runat="server" ID="Rubrics">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th colspan="2">Question</th>
                    <th>Rubric</th>
                    <th>Score</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:50px;">
                <%# Eval("QuestionSequence") %>
            </td>
            <td>
                <%# Eval("QuestionText") %>
            </td>
            <td>
                <%# Eval("RubricTitle") %>
            </td>
            <td>
                <%# FormatScore() %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>