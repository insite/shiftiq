<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDistributionAnalysisText.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDistributionAnalysisText" %>

<div class="text-end" style="margin-bottom:5px;">
    <insite:Button runat="server" ID="DownloadCsv" ButtonStyle="Primary" Size="ExtraSmall" ToolTip="Download CSV" Text="CSV" Icon="fa fa-download" />
</div>

<table class="table table-condensed table-text-analysis">
    <tbody>
        <asp:Repeater runat="server" ID="DataRepeater">
            <ItemTemplate>
                <tr>
		            <td style="width:30px;">
                        <a target="_blank" href="<%# Eval("ResponseSessionID", "/ui/admin/workflow/forms/submissions/outline?session={0}") %>"><i class="icon fa fa-eye"></i></a>
		            </td>
                    <td class="survey-analysis-text">
                        <%# HttpUtility.HtmlEncode((string)Eval("AnswerText")) %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
