<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricSummary.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.RubricSummary" %>

<asp:Repeater runat="server" ID="RubricRepeater">
    <ItemTemplate>

        <section>
            <h2 class="h4 mb-3">
                <%# Eval("RubricTitle") %>
            </h2>

            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="card h-100">
                                <div class="card-body">
                                    <h3>Question</h3>

                                    <div>
                                        <%# Eval("QuestionText") %>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6">

                            <div class="card h-100">
                                <div class="card-body">

                                    <table class="table table-striped rubric-criteria mb-3">
                                        <thead>
                                            <tr>
                                                <th class="w-50">Criterion</th>
                                                <th class="w-25">Rating</th>
                                                <th class="w-25 text-end">Points</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater runat="server" ID="CriteriaRepeater">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <%# Eval("CriterionTitle") %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("RatingTitle") %>
                                                        </td>
                                                        <td class="text-end">
                                                            <%# Eval("Points", "{0:n2}") %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>

                                    <div>
                                        <%= Translate("Total points:") %> <%# Eval("SelectedPoints", "{0:n2}") %>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </section>

    </ItemTemplate>
</asp:Repeater>