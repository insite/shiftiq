<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Records.Reports.LearnerActivity.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">

        <div id="toolbox" class="toolbox-section">
            
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-4">

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LearnerNameLast" EmptyMessage="Learner Last Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LearnerNameFirst" EmptyMessage="Learner First Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LearnerEmail" EmptyMessage="Learner Email " MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="LearnerGenders" EmptyMessage="Learner Gender" DropDown-Size="10" Multiple-ActionsBox="true" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="LearnerCitizenships" EmptyMessage="Learner Citizenship" DropDown-Size="10" Multiple-ActionsBox="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemIdMultiComboBox runat="server" ID="MembershipStatusItemIdentifiers" EmptyMessage="Membership Status" DropDown-Size="10" Multiple-ActionsBox="true" />
                    </div>

                    <div runat="server" id="CountStrategyContainer" class="mb-2">
                        <asp:RadioButtonList runat="server" ID="CountStrategy" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio-count-strategy">
                            <asp:ListItem Text="Summary by Learner" Value="Summary" Selected="True" />
                            <asp:ListItem Text="Detail by Learner and Gradebook (Course)" Value="Detail" />
                        </asp:RadioButtonList>
                    </div>

                    <div class="mt-3">
                        <insite:FilterButton ID="SearchButton" runat="server" />
                        <insite:ClearButton ID="ClearButton" runat="server" />
                    </div>

                </div>

                <div class="col-4">

                    <div class="mb-2">
                        <insite:FindEmployer runat="server" ID="EmployerGroupIdentifiers" EmptyMessage="Belongs To / Employed By" MaxSelectionCount="0" />
                    </div>

                    <div class="mb-2">
                        <insite:FindProgram runat="server" ID="ProgramIdentifiers" EmptyMessage="Program Names" MaxSelectionCount="0" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="GradebookNames" EmptyMessage="Gradebook Names" DropDown-Size="10" Multiple-ActionsBox="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EnrollmentStatus" EmptyMessage="Enrollment Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="1. Registered" Text="1. Registered" />
                                <insite:ComboBoxOption Value="2. Started" Text="2. Started" />
                                <insite:ComboBoxOption Value="3. Completed" Text="3. Completed" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EnrollmentStartedSince" runat="server" EmptyMessage="Enrollment Started &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EnrollmentStartedBefore" runat="server" EmptyMessage="Enrollment Started &lt;" />
                    </div>

                </div>

                <div class="col-4">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="AchievementStatus" EmptyMessage="Achievement Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Not Granted" Text="Achievement Not Granted" />
                                <insite:ComboBoxOption Value="Granted" Text="Achievement Granted" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AchievementGrantedSince" runat="server" EmptyMessage="Achievement Granted &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="AchievementGrantedBefore" runat="server" EmptyMessage="Achievement Granted &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EngagementStatus" EmptyMessage="Engagement Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="NoActivity" Text="Never Signed In" />
                                <insite:ComboBoxOption Value="LastActivityOverOneWeekAgo" Text="Last Authenticated Over 1 Week Ago" />
                                <insite:ComboBoxOption Value="LastActivityOverOneMonthAgo" Text="Last Authenticated Over 1 Month Ago" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="LearnerRole" EmptyMessage="Learner Role">
                            <Items>
                                <insite:ComboBoxOption Value="" Text="" />
                                <insite:ComboBoxOption Value="Learner" Text="Learners Only" />
                                <insite:ComboBoxOption Value="Administrator" Text="Administrators Only" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PersonCode" EmptyMessage="Person Code" MaxLength="20" />
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
        </div>
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>

</div>