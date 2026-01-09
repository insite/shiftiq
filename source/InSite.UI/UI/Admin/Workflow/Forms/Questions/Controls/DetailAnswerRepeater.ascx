<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailAnswerRepeater.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.DetailAnswerRepeater" %>

<section>
    <h2 class="h4 mt-4 mb-3">Answers</h2>
    <div class="card border-0 shadow-lg h-100">
        <div class="card-body">

            <table class="table table-striped">
                <tr>
                    <th>Submission</th>
                    <th>Respondent</th>
                    <th>Answer</th>
                </tr>
                <asp:Repeater runat="server" ID="AnswerRepeater">
                    <ItemTemplate>
                        <tr>
                            <td class="text-nowrap"><%# Eval("ResponseDate") %></td>
                            <td class="text-nowrap"><%# Eval("RespondentName") %>
                                <div class="fs-sm text-body-secondary"><%# Eval("RespondentEmail") %></div>
                            </td>
                            <td><%# Eval("OptionText") %>
                                <div class="fs-sm text-body-secondary"><%# Eval("AnswerText") %></div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </table>

        </div>
    </div>
</section>
