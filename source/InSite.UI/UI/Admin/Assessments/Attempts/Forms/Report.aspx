<%@ Page Language="C#" CodeBehind="Report.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.Report" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AttemptGrid.ascx" TagName="AttemptGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/TimeSeriesChart.ascx" TagName="TimeSeriesChart" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultStatisticExam.ascx" TagName="ResultStatisticExam" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultStatisticCompetency.ascx" TagName="ResultStatisticCompetency" TagPrefix="uc" %>
<%@ Register Src="../Controls/QuestionAnalysisRepeater.ascx" TagName="QuestionAnalysisRepeater" TagPrefix="uc" %>
<%@ Register Src="../Controls/ResultQuestionCommentsRepeater.ascx" TagName="ResultQuestionCommentsRepeater" TagPrefix="uc" %>
<%@ Register Src="../Controls/QuestionCompetencySummaryRepeater.ascx" TagName="QuestionCompetencySummaryRepeater" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Exam Report" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row">

                        <div class="col-lg-3">

                            <div class="form-group mb-3">
                                <label class="form-label">Organization</label>
                                <div><insite:FindOrganization runat="server" ID="SearchOrganizationIdentifier" /></div>
                            </div>

                        </div>
                        <div class="col-lg-3">

                            <div class="form-group mb-3">
                                <label class="form-label">Assessment Bank</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamBank" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Assessment Form</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamForm" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Exam Event</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamEvent" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Exam Event Format</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaEventFormat" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Submission Tag</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptTag" /></div>
                            </div>

                        </div>
                        <div class="col-lg-3">
                    
                            <div class="form-group mb-3">
                                <label class="form-label">Learner (Candidate)</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamCandidate" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Learner Name</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaCandidateName" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Learner Registration Type</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaCandidateType" /></div>
                            </div>

                        </div>
                        <div class="col-lg-3">

                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Started &ge;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaStartedSince" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Started &lt;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaStartedBefore" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Completed &ge;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaCompletedSince" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Completed &lt;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaCompletedBefore" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Pilot Attempts</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaPilotAttempts" /></div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">Pending Attempts</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaPendingAttempts" /></div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AttemptTab" Title="Attempts" Icon="far fa-tasks" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:AttemptGrid runat="server" ID="AttemptGrid" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="TimeSeriesTab" Title="Time Series" Icon="far fa-chart-bar" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:TimeSeriesChart runat="server" ID="TimeSeriesChart" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="BellCurveTab" Title="Statistics" Icon="far fa-tachometer-alt" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:Nav runat="server">
                        <insite:NavItem runat="server" Title="Exam">
                            <uc:ResultStatisticExam runat="server" ID="ResultStatisticExam" />
                        </insite:NavItem>
                        <insite:NavItem runat="server" Title="GAC">
                            <asp:Repeater runat="server" ID="ResultStatisticGacRepeater">
                                <ItemTemplate>
                                    <h2><%# Eval("Name") %></h2>
                                    <uc:ResultStatisticCompetency runat="server" ID="ResultStatisticCompetency" />
                                </ItemTemplate>
                                <SeparatorTemplate>
                                    <div class="py-3"></div>
                                </SeparatorTemplate>
                            </asp:Repeater>
                        </insite:NavItem>
                        <insite:NavItem runat="server" Title="Competency">
                            <asp:Repeater runat="server" ID="ResultStatisticCompetencyRepeater">
                                <ItemTemplate>
                                    <h2><%# Eval("GacName") %></h2>
                                    <h3><%# Eval("CompetencyName") %></h3>
                                    <uc:ResultStatisticCompetency runat="server" ID="ResultStatisticCompetency" />
                                </ItemTemplate>
                                <SeparatorTemplate>
                                    <div class="py-3"></div>
                                </SeparatorTemplate>
                            </asp:Repeater>
                        </insite:NavItem>
                    </insite:Nav>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="QuestionAnalysisTab" Title="Question Analysis" Icon="far fa-question" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:QuestionAnalysisRepeater runat="server" ID="QuestionAnalysisRepeater" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:ResultQuestionCommentsRepeater runat="server" ID="QuestionCommentsRepeater" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="StandardsTab" Title="Standards" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:QuestionCompetencySummaryRepeater runat="server" ID="CompetencySummary" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DownloadTab" Title="Download" Icon="far fa-download" IconPosition="BeforeText">
            <div class="row">
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg mt-3">
                        <div class="card-body">

                            <h3>Output Settings</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Download Options</label>
                                <div class="mb-3">
                                    <asp:CheckBox runat="server" ID="IncludeAdditionalSheets" Text="Include additional metadata sheets" Checked="True" />
                                    <div class="form-text mt-0 ms-4">
                                        Check this box to include additional sheets.
                                    </div>
                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:DownloadButton runat="server" ID="DownloadButton" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

</asp:Content>