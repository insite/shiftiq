<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Attempts.Reports.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ExamBankUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="ExamBankUpdatePanel" ClientEvents-OnRequestStart="localScript.onRequestStart">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:FindBankFramework runat="server" ID="ExamBankFrameworkID" EmptyMessage="Exam Bank Framework" />
                            </div>

                            <div class="mb-2">
                                <insite:FindBank runat="server" ID="ExamBankID" EmptyMessage="Exam Bank" />
                                <insite:CustomValidator runat="server" ID="CustomValidator" ErrorMessage="Please select 'Exam Bank' or 'Exam Candidate Name'" Display="None" ValidationGroup="Filter" />
                            </div>

                            <insite:Container runat="server" ID="BankRelatedFields">
                                <div class="mb-2">
                                    <insite:FindBankForm runat="server" ID="ExamFormID" EmptyMessage="Exam Form" />
                                </div>

                                <div class="mb-2">
                                    <insite:FindUser runat="server" ID="ExamCandidateID" EmptyMessage="Exam Candidate" />
                                </div>
                            </insite:Container>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mb-2">
                        <insite:BankLevelComboBox runat="server" ID="ExamBankLevel" EmptyMessage="Exam Bank Level" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CandidateName" EmptyMessage="Candidate Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AssessorName" EmptyMessage="Assessor Name" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="PilotAttemptsInclusion" EmptyMessage="Pilot Attempts">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Exclude" Text="Exclude Pilot Attempts" />
                                <insite:ComboBoxOption Value="Only" Text="Pilot Attempts Only" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FindRubric runat="server" ID="Rubric" EmptyMessage="Rubric" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="AttemptCompletionStatus" EmptyMessage="Attempt Completion Status" TrueText="Completed" FalseText="Not Completed" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptStartedSince" runat="server" EmptyMessage="Attempt Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptStartedBefore" runat="server" EmptyMessage="Attempt Started Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptGradedSince" runat="server" EmptyMessage="Attempt Graded Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptGradedBefore" runat="server" EmptyMessage="Attempt Graded Before" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="AttemptScoreMinimum" runat="server" EmptyMessage="Attempt Score Minimum" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="AttemptScoreMaximum" runat="server" EmptyMessage="Attempt Score Maximum" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="AttemptTagStatus" EmptyMessage="Attempt Tag Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Tagged" Text="Tagged" />
                                <insite:ComboBoxOption Value="Not Tagged" Text="Not Tagged" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ExamAttemptTagMultiComboBox runat="server" ID="AttemptTag" Multiple-ActionsBox="true" EmptyMessage="Attempt Tag" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindEvent runat="server" ID="ExamEventID" EmptyMessage="Exam Event" />
                    </div>

                    <div class="mb-2">
                        <insite:EventFormatComboBox runat="server" ID="EventFormat" EmptyMessage="Exam Event Format" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameMultiComboBox runat="server" ID="CandidateType" Settings-CollectionName="Registrations/Exams/Candidate/Type" EmptyMessage="Candidate Type" />
                    </div>

                    <div class="mb-2">
                        <insite:OrganizationRoleMultiComboBox ID="OrganizationRole" runat="server" EmptyMessage="Organization Role" />
                    </div>

                    <div class="mb-2">
                        <insite:CheckBox runat="server" ID="HideLearnerName" Text="Hide Learner Name" />
                    </div>

                    <div class="mb-2">
                        <insite:CheckBox runat="server" ID="IncludePendingAttempts" Text="Include Pending Attempts in Reporting" />
                    </div>

                    <div runat="server" id="GradingAssessorCriteria" visible="false">
                        <div class="mb-2">
                            <insite:FindPerson runat="server" ID="GradingAssessor" EmptyMessage="Grading Assessor" CssClass="me-1" />
                        </div>

                        <div class="mb-2">
                            <insite:CheckBox runat="server" ID="ShowEmptyGradingAssessor" Text="Show Attempts without Grading Assessor" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-4">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Exam Form" Value="Form.FormTitle,AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Exam Candidate" Value="LearnerPerson.UserFullName,AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Submission Start Time" Value="AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Submission Score" Value="AttemptScore,AttemptStarted" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
