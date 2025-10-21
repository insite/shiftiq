<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResultStatisticCompetency.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.ResultStatisticCompetency" %>

<div class="row my-3" style="height:40vh;">
    <chart:BarChart runat="server" ID="Chart" LegendVisible="false" MaintainApectRatio="false" ToolTipIntersect="false" ToolTipInteractionMode="X" />
</div>

<div class="row bell-curve-stats mb-3">
    <div class="col-md-6">

        <table class="table table-striped mb-0">
            <tr>
                <td class="text-end">Attempts Completed:</td>
                <td class="text-start"><asp:Literal runat="server" ID="AttemptCompletedCount" /></td>
            </tr>
            <tr>
                <td class="text-end">Attempts Passed:</td>
                <td class="text-start"><asp:Literal runat="server" ID="AttemptPassedCount" /></td>
            </tr>
            <tr>
                <td class="text-end">Pass Rate:</td>
                <td class="text-start"><asp:Literal runat="server" ID="AttemptPassRate" /></td>
            </tr>
            <tr>
                <td class="text-end">Questions:</td>
                <td class="text-start"><asp:Literal runat="server" ID="QuestionCount" /></td>
            </tr>
            <tr>
                <td class="text-end">Highest Score:</td>
                <td class="text-start"><asp:Literal runat="server" ID="HighestScore" /></td>
            </tr>
            <tr>
                <td class="text-end">Lowest Score:</td>
                <td class="text-start"><asp:Literal runat="server" ID="LowestScore" /></td>
            </tr>
        </table>

    </div>
    <div class="col-md-6">

        <table class="table table-striped mb-0">
            <tr>
                <td class="text-end">Median:</td>
                <td class="text-start"><asp:Literal runat="server" ID="ResultsMedian" /></td>
            </tr>
            <tr>
                <td class="text-end">Mean:</td>
                <td class="text-start"><asp:Literal runat="server" ID="ResultsMean" /></td>
            </tr>
            <tr>
                <td class="text-end">Standard Deviation:</td>
                <td class="text-start"><asp:Literal runat="server" ID="ResultsStandardDeviation" /></td>
            </tr>
            <tr>
                <td class="text-end">Standard Error of Measurement:</td>
                <td class="text-start"><asp:Literal runat="server" ID="StandardErrorOfMeasurement" /></td>
            </tr>
        </table>

    </div>
</div>