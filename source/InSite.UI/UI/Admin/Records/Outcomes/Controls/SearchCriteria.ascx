<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Records.Outcomes.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="GradebookTitle" runat="server" EmptyMessage="Gradebook" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedSince" runat="server" EmptyMessage="Created &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedBefore" runat="server" EmptyMessage="Created &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="StudentName" EmptyMessage="Student" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FindEmployer runat="server" ID="StudentEmployerGroupIdentifier" EmptyMessage="Student Employer" />
                    </div>

                    <div class="mb-2">
                        <insite:FindStandard runat="server" ID="CompetencySelector" EmptyMessage="Competency" />
                    </div>

                    <div class="mb-2">
                        Points:<br />
                        <div class="row">
                            <div class="col-6">
                                <insite:NumericBox runat="server" ID="PointsFrom" DecimalPlaces="1" MaxValue="100" DigitGrouping="false" EmptyMessage="From" />                                
                            </div>
                            <div class="col-6">
                                <insite:NumericBox runat="server" ID="PointsThru" DecimalPlaces="1" MaxValue="100" DigitGrouping="false" EmptyMessage="Thru" />
                            </div>
                        </div>
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-6">
                    <div class="mb-2"> 
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Class" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AchievementTitle" EmptyMessage="Achievement" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventInstructorIdentifier" EmptyMessage="Instructor" Height="300" />
                    </div>

                    <div class="mb-2">
                        <insite:FindPeriod runat="server" ID="GradebookPeriodIdentifier" EmptyMessage="Gradebook Period" />
                    </div>

                    <div class="mb-2">
                        <insite:FindPeriod runat="server" ID="UserPeriodIdentifier" EmptyMessage="Student Period" />
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
    </div>
    <div class="col-3">              
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
