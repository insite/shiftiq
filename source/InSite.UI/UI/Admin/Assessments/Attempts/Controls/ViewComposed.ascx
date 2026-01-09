<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewComposed.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.ViewComposed" %>


<asp:Repeater runat="server" ID="QuestionRepeater">
    <ItemTemplate>
        <div class="vstack">
            <div class="mb-2">
                <div class="float-start">
                    <%# FormatScore() %>
                </div>
                <div class="float-end">
                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                    </span>
                </div>
            </div>

            <div runat="server" class="row" visible='<%# !(bool)Eval("IsGraded") %>'>
                <div class="col-lg-6">

                    <div class="card h-100">
                        <div class="card-body">
                            <h3>
                                Question <%# Eval("QuestionSequence") %>
                            </h3>

                            <div>
                                <%# GetQuestionText() %>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="col-lg-6">

                    <div class="card h-100">
                        <div class="card-body">
                            <h3>
                                Answer
                            </h3>

                            <div>
                                <asp:PlaceHolder runat="server" ID="AnswerPlaceholder1" />
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div runat="server" class="row" visible='<%# Eval("IsGraded")  %>'>
                <div class="col-lg-6 mb-3 mb-lg-0">

                    <div class="card">
                        <div class="card-body">
                            <h3>
                                Question <%# Eval("QuestionSequence") %>
                            </h3>

                            <div>
                                <%# GetQuestionText() %>
                            </div>
                        </div>
                    </div>

                    <div class="card mt-3">
                        <div class="card-body">
                            <h3>
                                Answer
                            </h3>

                            <div>
                                <asp:PlaceHolder runat="server" ID="AnswerPlaceholder2" />
                            </div>
                        </div>
                    </div>

                </div>
                <div class="col-lg-6">

                    <div runat="server" class="card" visible='<%# Eval("Ratings") != null %>'>
                        <div class="card-body">
                            <h3>
                                Rubrics
                            </h3>

                            <asp:Repeater runat="server" ID="RatingRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th>Criterion</th>
                                                <th>Rating</th>
                                                <th class="text-end">Points</th>
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
                                        <td>
                                            <%# Eval("CriterionTitle") %>
                                        </td>
                                        <td>
                                            <%# Eval("RatingTitle") %>
                                        </td>
                                        <td class="text-end">
                                            <%# Eval("Points") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <div class="mt-3">
                                Total Points: <%# GetRatingTotalPoints() %>
                            </div>

                        </div>
                    </div>

                    <div runat="server" class="card mt-3" visible='<%# Eval("ExemplarHtml") != null %>'>
                        <div class="card-body">
                            <h3>
                                Exemplar
                            </h3>

                            <div>
                                <%# Eval("ExemplarHtml") %>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </ItemTemplate>
</asp:Repeater>
