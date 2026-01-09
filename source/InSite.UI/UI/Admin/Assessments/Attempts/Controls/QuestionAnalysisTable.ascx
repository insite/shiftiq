<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionAnalysisTable.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.QuestionAnalysisTable" %>

<table class="table table-answers table-transparent">
    <thead>
        <tr>
            <th>
                <asp:Literal runat="server" ID="SuccessRate" />
                Success on
                <asp:HyperLink runat="server" ID="AttemptCountLink">
                    <asp:Literal runat="server" ID="AttemptCount" />
                    Attempts
                </asp:HyperLink>
            </th>
            <asp:Repeater runat="server" ID="AnswerHeaderRepeater">
                <ItemTemplate>
                    <th style="text-align:center; <%# (bool?)Eval("HasPoints") == true ? "color:#27ae60 !important; font-weight:bold;" : "color:#c0392b !important; font-weight:normal;" %>">
                        <%# (bool?)Eval("HasPoints") == true ? "<i class='fa fa-check'></i>" : "<i class='fa fa-times'></i>" %>
                        <%# Eval("Letter") %>
                    </th>
                </ItemTemplate>
            </asp:Repeater>
            <th style="text-align:center;">
                NA
            </th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Answer Rate</td>
            <asp:Repeater runat="server" ID="AnswerRateRepeater">
                <ItemTemplate>
                    <td style="text-align:center;">
                        <%# Eval("AnswerRate", "{0:p0}") %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td style="text-align:center;">
                <asp:Literal runat="server" ID="NoAnswerRate" />
            </td>
        </tr>
        <tr>
            <td>Answer Count</td>
            <asp:Repeater runat="server" ID="AnswerCountRepeater">
                <ItemTemplate>
                    <td style="text-align:center;">
                        <%# Eval("AnswerCount", "{0:n0}") %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td style="text-align:center;">
                <asp:Literal runat="server" ID="NoAnswerCount" />
            </td>
        </tr>
        <tr>
            <td>Average Exam Score</td>
            <asp:Repeater runat="server" ID="AverageAttemptScoreRepeater">
                <ItemTemplate>
                    <td style="text-align:center;">
                        <%# Eval("AverageAttemptScorePercent", "{0:n0}") %> %
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td style="text-align:center;">
                <asp:Literal runat="server" ID="NoAnswerAverageAttemptScorePercent" />
            </td>
        </tr>
        <tr>
            <td>Item Total Correlation</td>
            <asp:Repeater runat="server" ID="ItemTotalCorrelationRepeater">
                <ItemTemplate>
                    <td style="text-align:center;">
                        <%# GetItemTotalCorrelationText((double)Eval("ItemTotalCorrelation")) %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td style="text-align:center;">
                <asp:Literal runat="server" ID="NoAnswerItemTotalCorrelationText" />
            </td>
        </tr>
        <tr>
            <td>Item Rest Coefficient</td>
            <asp:Repeater runat="server" ID="ItemRestCoefficientRepeater">
                <ItemTemplate>
                    <td style="text-align:center;">
                        <%# GetItemRestCoefficientText((double)Eval("ItemRestCoefficient")) %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td style="text-align:center;">
                <asp:Literal runat="server" ID="NoAnswerItemRestCoefficientText" />
            </td>
        </tr>
    </tbody>
</table>