<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReviewDetails.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.ReviewDetails" %>

<%@ Import Namespace="InSite.Domain.Surveys.Forms" %>

<div runat="server" id="CompletedInstructionsPanel" class="mt-3">
    <asp:Literal runat="server" ID="CompletedInstructionsLiteral" />
</div>

<div class="mb-4">
    <div runat="server" id="BarChartPanel" class="mt-5 mb-5">
        <div class="card card-hover shadow bg-secondary">

            <div class="card-header border-bottom-0">
                <h2 class="mb-0"><insite:Literal runat="server" Text="Summary Chart" /></h2>
            </div>

            <div runat="server" id="BarChartCardBody" class="card-body bg-white">
                <div class="d-flex flex-wrap justify-content-center fs-sm">
                    <asp:Repeater runat="server" ID="ChartLegend">
                        <ItemTemplate>
                            <div class="border rounded-1 py-1 px-2 me-2 mb-2">
                                <div class="d-inline-block align-middle me-1" style="width:.75rem; height:.75rem; background-color:<%# Eval("Item2") %>;"></div>
                                <span class="d-inline-block align-middle"><%# Eval("Item1") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Literal runat="server" ID="Chart" />
            </div>

        </div>
    </div>

    <asp:Repeater runat="server" ID="CardRepeater">
        <ItemTemplate>
            <div class="mt-5 mb-5" style="page-break-inside:avoid !important;">
                <div class="card card-hover shadow bg-secondary">

                    <div class="card-header border-bottom-0<%# string.IsNullOrEmpty((string)Eval("Title")) ? " d-none" : string.Empty %>">
                        <h2 class="mb-0"><%# Eval("Title") %></h2>        
                    </div>

                    <div class="card-body bg-white">

                        <asp:Repeater runat="server" ID="QuestionRepeater">
                            <ItemTemplate>
                                <div class="mb-4" data-question-item='<%# Eval("Identifier") %>'>

                                    <div class="mb-4">
                                        <%# Eval("Html") %>
                                    </div>

                                    <div class="row" runat="server" id="AnswerPanel" visible='<%# !(bool)Eval("IsHidden") %>'>
                                        <div class='col-md-12'>

                                            <h4 class="text-primary"><%# Translate("Your Answer") %>:</h4>

                                            <ul runat="server" visible='<%# HasResponse() %>'>
                                                <asp:Repeater runat="server" ID="OptionRepeater">
                                                    <ItemTemplate>
                                                        <li class='mb-3'>
                                                            <div runat="server" class="mb-2" visible='<%# Eval("Header") != null %>'><em><%# Eval("Header") %></em></div>
                                                            <strong><%# Eval("Title") %></strong>
                                                            <insite:Container runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("Description")) %>'>
                                                                <h5 class='mt-2 mb-1 text-primary'><%# Translate("Description") %>:</h5>
                                                                <%# Eval("Description") %>
                                                            </insite:Container>
                                                            <insite:Container runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("Feedback")) %>'>
                                                                <h5 class='mt-2 mb-1 text-primary'><%# Translate("Feedback") %>:</h5>
                                                                <%# Eval("Feedback") %>
                                                            </insite:Container>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <%# GetUploadLinks() %>

                                                <li runat="server" class='mb-3' visible='<%# HasComment() %>'>
                                                    <div style="white-space:pre-wrap;"><%# Eval("Answer.Text") %></div>
                                                </li>
                                            </ul>

                                            <p runat="server" style="font-weight:bold;" visible='<%# !HasResponse() %>'>No Submission</p>

                                            <insite:Container runat="server" Visible='<%# Eval("Answer.LikertScale") != null %>'>
                                                <asp:Repeater runat="server" ID="ScaleRepeater" Visible="false">
                                                    <HeaderTemplate><div></HeaderTemplate>
                                                    <FooterTemplate></div></FooterTemplate>
                                                    <ItemTemplate>
                                                        <div class='card bg-secondary mt-3'>
                                                            <div class='card-header card-header-category'>
                                                                <%# Eval("Category") %>
                                                            </div>
                                                            <div class='card-body'>
                                                                <h5 class='card-title'>
                                                                    <%# Eval("Grade") %>
                                                                    <asp:Literal runat="server" ID="GradeValue" />
                                                                </h5>
                                                                <%# Eval("FeedbackHtml") %>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <div runat="server" class='fs-ms text-body-secondary float-end' visible='<%# Eval("Answer.LikertScale.HighestPoints") != null %>'>
                                                    <%# Eval("Answer.LikertScale.HighestPoints", "{0:n2}") %> Points (Highest)
                                                </div>
                                            </insite:Container>

                                        </div>
                                    </div>

                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<insite:PageFooterContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .card-header-category {
            font-size: 1.125rem;
            font-weight: bold;
            color: #4a4b65;
        }
    </style>
</insite:PageFooterContent>

<insite:PageFooterContent runat="server">
    <link rel="stylesheet" href="/ui/portal/workflow/forms/controls/charts.css" />
    <script src="/ui/portal/workflow/forms/controls/charts.js"></script>
</insite:PageFooterContent>
