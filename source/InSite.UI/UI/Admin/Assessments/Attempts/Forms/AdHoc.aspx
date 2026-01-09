<%@ Page Language="C#" CodeBehind="AdHoc.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.AdHoc" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AdHocCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="../Controls/AdHocResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="../Controls/TimeSeriesChart.ascx" TagName="TimeSeriesChart" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultStatisticExam.ascx" TagName="ResultStatisticExam" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultStatisticCompetency.ascx" TagName="ResultStatisticCompetency" TagPrefix="uc" %>
<%@ Register Src="../Controls/QuestionAnalysisRepeater.ascx" TagName="QuestionAnalysisRepeater" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultQuestionCommentsRepeater.ascx" TagName="ResultQuestionCommentsRepeater" TagPrefix="uc" %>
<%@ Register Src="../Controls/QuestionCompetencySummaryRepeater.ascx" TagName="QuestionCompetencySummaryRepeater" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="SearchCriteria" />

    <div class="float-end">
        <insite:Button runat="server" ID="RatioButton" ButtonStyle="Warning" Icon="fas fa-chart-bar" Text="Ratio" IconPosition="BeforeText" NavigateUrl="/ui/admin/assessments/attempts/ratio" />
    </div>

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Attempts">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="TimeSeriesTab" Icon="fas fa-chart-bar" Title="Time Series">
            <uc:TimeSeriesChart runat="server" ID="TimeSeriesChart" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="BellCurveTab" Icon="fas fa-tachometer-alt" Title="Statistics">
            <insite:Nav runat="server">
                <insite:NavItem runat="server" Title="Exam">
                    <uc:ResultStatisticExam runat="server" ID="ResultStatisticExam" />
                </insite:NavItem>
                <insite:NavItem runat="server" Title="GAC">
                    <asp:Repeater runat="server" ID="ResultStatisticGacRepeater">
                        <ItemTemplate>
                            <h3><%# Eval("Name") %></h3>
                            <uc:ResultStatisticCompetency runat="server" ID="ResultStatisticCompetency" />
                        </ItemTemplate>
                    </asp:Repeater>
                </insite:NavItem>
                <insite:NavItem runat="server" Title="Competency">
                    <asp:Repeater runat="server" ID="ResultStatisticCompetencyRepeater">
                        <ItemTemplate>
                            <h3><%# Eval("GacName") %></h3>
                            <h4><%# Eval("CompetencyName") %></h4>
                            <uc:ResultStatisticCompetency runat="server" ID="ResultStatisticCompetency" />
                        </ItemTemplate>
                    </asp:Repeater>
                </insite:NavItem>
            </insite:Nav>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="QuestionAnalysisTab" Icon="fas fa-question" Title="Question Analysis">
            <uc:QuestionAnalysisRepeater runat="server" ID="QuestionAnalysisRepeater" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CommentTab" Icon="fas fa-comment" Title="Comments">
            <uc:ResultQuestionCommentsRepeater runat="server" ID="QuestionCommentsRepeater" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="StandardsTab" Icon="fas fa-ruler-triangle" Title="Standards">
            <uc:QuestionCompetencySummaryRepeater runat="server" ID="CompetencySummary" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>
