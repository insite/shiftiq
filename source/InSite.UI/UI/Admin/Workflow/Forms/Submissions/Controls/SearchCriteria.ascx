<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Submissions.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                    
                            <div class="mb-2">
                                <insite:FindWorkflowForm runat="server" ID="SurveyID" EmptyMessage="Form" />
                            </div>  
                    
                            <div class="mb-2">
                                <insite:FormQuestionComboBox runat="server" ID="QuestionID" EmptyMessage="Question" MaxTextLength="40" ExcludeSpecialQuestions="true" />
                            </div>

                            <div class="mb-2">
                                <insite:FormSubmissionAnswerComboBox runat="server" ID="AnswerText"
                                    EmptyMessage="Answer"
                                    AllowBlank="true"
                                    MaxTextLength="50"
                                    ShowEllipsisOnTextCut="true"
                                />
                            </div>

                        </ContentTemplate>
                    </insite:UpdatePanel>
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="RespondentName" EmptyMessage="Respondent" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsLocked" EmptyMessage="Lock Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Locked" />
                                <insite:ComboBoxOption Value="False" Text="Unlocked" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="StartedSince" runat="server" EmptyMessage="Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="StartedBefore" runat="server" EmptyMessage="Started Before" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CompletedSince" runat="server" EmptyMessage="Completed Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CompletedBefore" runat="server" EmptyMessage="Completed Before" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="AgencyGroupIdentifier" EmptyMessage="Group" />
                    </div>
                    <div class="mb-2">
                        <insite:FindPeriod runat="server" ID="PeriodIdentifiers" EmptyMessage="Period" MaxSelectionCount="0" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div class="mb-4">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Submission Started" Value="ResponseSessionStarted desc" />
                    <insite:ComboBoxOption Text="Sort by Form Name" Value="SurveyName,ResponseSessionStarted desc" />
                    <insite:ComboBoxOption Text="Sort by Respondent Name" Value="RespondentName,ResponseSessionStarted desc" />
                    <insite:ComboBoxOption Text="Sort by Group" Value="GroupName,ResponseSessionStarted desc" />
                    <insite:ComboBoxOption Text="Sort by Period" Value="PeriodName desc,ResponseSessionStarted desc" />
                </Items>
            </insite:ComboBox>
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>