<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Assessor.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
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
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
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
