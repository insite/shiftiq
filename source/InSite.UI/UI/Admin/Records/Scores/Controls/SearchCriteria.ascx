<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Records.Scores.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="GradebookTitle" runat="server" EmptyMessage="Gradebook" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedSince" runat="server" EmptyMessage="Gradebook Created Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedBefore" runat="server" EmptyMessage="Gradebook Created Before" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="ItemTypes" EmptyMessage="Grade Item Type" Multiple-ActionsBox="true">
                            <Items>
                                <insite:ComboBoxOption Value="Category" Text="Category" />
                                <insite:ComboBoxOption Value="Score" Text="Score" />
                                <insite:ComboBoxOption Value="Calculation" Text="Calculation" />
                            </Items>
                        </insite:MultiComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ItemFormat" EmptyMessage="Grade Item Format">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Percent" Text="Percent" />
                                <insite:ComboBoxOption Value="Text" Text="Text" />
                                <insite:ComboBoxOption Value="Boolean" Text="Boolean" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ItemName" EmptyMessage="Grade Item Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AchievementTitle" EmptyMessage="Grade Item Achievement" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FindEmployer runat="server" ID="LearnerEmployerGroupIdentifier" EmptyMessage="Learner Employer" />
                    </div>

                    <div class="mb-2">
                        <insite:CollectionItemComboBox runat="server" ID="LearnerEmployerGroupStatusId" EmptyMessage="Learner Employer Status" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LearnerName" EmptyMessage="Learner Name" MaxLength="256" />
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradedSince" runat="server" EmptyMessage="Learner Graded Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradedBefore" runat="server" EmptyMessage="Learner Graded Before" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="ScorePercentFrom" runat="server" EmptyMessage="Score Percent &ge;" DecimalPlaces="1" MaxValue="100" DigitGrouping="false" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="ScorePercentThru" runat="server" EmptyMessage="Score Percent &le;" DecimalPlaces="1" MaxValue="100" DigitGrouping="false" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ScoreText" EmptyMessage="Score Text" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ProgressStatus" EmptyMessage="Progress Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Started" Text="Started" />
                                <insite:ComboBoxOption Value="Completed" Text="Completed" />
                                <insite:ComboBoxOption Value="Incomplete" Text="Incomplete" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="ScoreComment" runat="server" EmptyMessage="Score Comment" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="IsScoreIgnored" EmptyMessage="Is Score Ignored" TrueText="Ignored" FalseText="Not Ignored" AllowBlank="true" />
                    </div>

                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindPeriod runat="server" ID="UserPeriodSelector" EmptyMessage="Enrollment Period" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Class" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Class Scheduled Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Class Scheduled Before" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventInstructorIdentifier" EmptyMessage="Class Instructor" Height="300" />
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
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
