<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDistributionSearchResults.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDistributionSearchResults" %>

<asp:Repeater runat="server" ID="QuestionRepeater">
    <ItemTemplate>
        <div class="row row-distr-result mb-4">
            <div class="col-xs-12">
                <div runat="server" class="fw-bold" visible='<%# !string.IsNullOrEmpty((string)Eval("QuestionText")) %>'>
                    <%# Shift.Common.Markdown.ToHtml((string)Eval("QuestionText")) %>
                </div>

                <div runat="server" class="field-heading" visible='<%# !string.IsNullOrEmpty((string)Eval("QuestionSubText")) %>'>
                    <span>
                        <%# Shift.Common.Markdown.ToHtml((string)Eval("QuestionSubText")) %>
                    </span>
                </div>

                <asp:Repeater runat="server" ID="AnalysisRepeater">
                    <ItemTemplate>
                        <insite:DynamicControl runat="server" ID="AnalysisContainer" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
