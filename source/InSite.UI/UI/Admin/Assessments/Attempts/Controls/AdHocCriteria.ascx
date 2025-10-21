<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdHocCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.AdHocCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ExamBankUpdatePanel" />

            <div class="row">
                <div class="col-6">
                    <insite:UpdatePanel runat="server" ID="ExamBankUpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:FindBankOccupation runat="server" ID="ExamBankOccupationID" EmptyMessage="Exam Bank Occupation" />
                            </div>
                            <div class="mb-2">
                                <insite:FindBankFramework runat="server" ID="ExamBankFrameworkID" EmptyMessage="Exam Bank Framework" Width="95%" />
                                <insite:RequiredValidator runat="server" ControlToValidate="ExamBankFrameworkID" Display="Dynamic" FieldName="Exam Bank Framework" ValidationGroup="SearchCriteria" />
                            </div>
                            <div class="mb-2">
                                <insite:FindBank runat="server" ID="ExamBankID" EmptyMessage="Exam Bank" />
                            </div>
                            <div class="mb-2">
                                <insite:FindBankForm runat="server" ID="FormIdentifier" EmptyMessage="Exam Form" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                    
                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="ExamCandidateID" EmptyMessage="Exam Candidate" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:FindEvent runat="server" ID="ExamEventID" EmptyMessage="Exam Event" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:EventFormatComboBox runat="server" ID="EventFormat" EmptyMessage="Event Format" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:ExamAttemptTagMultiComboBox runat="server" ID="AttemptTag" Multiple-ActionsBox="true" EnableSearch="true" EmptyMessage="Attempt Tag" />
                    </div>                                   

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" ValidationGroup="SearchCriteria" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IncludeOnlyFirstAttempt">
                            <Items>
                                <insite:ComboBoxOption Value="False" Text="Include All" />
                                <insite:ComboBoxOption Value="True" Text="Include First Only" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptStartedSince" runat="server" EmptyMessage="Started &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptStartedBefore" runat="server" EmptyMessage="Started &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptCompletedSince" runat="server" EmptyMessage="Completed &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AttemptCompletedBefore" runat="server" EmptyMessage="Completed &lt;" />
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
                        <insite:ItemNameMultiComboBox runat="server" ID="CandidateType" Settings-CollectionName="Registrations/Exams/Candidate/Type" EmptyMessage="Candidate Type" />
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

        <div class="mb-2">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Attempt Start Time" Value="AttemptStarted" />
                    <insite:ComboBoxOption Text="Sort by Attempt Score" Value="AttemptScore" />
                    <insite:ComboBoxOption Text="Sort by Exam Form Name" Value="Form.FormName" />
                    <insite:ComboBoxOption Text="Sort by Exam Candidate Name" Value="LearnerPerson.UserFullName" />
                </Items>
            </insite:ComboBox>
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>