<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentUserStatusCriteria.ascx.cs" Inherits="InSite.Custom.CMDS.Reports.Controls.DepartmentUserStatusCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="SnapshotCriteriaType" OnClientSelectedIndexChanged="departmentUserStatusCriteria.onSnapshotCriteriaTypeChanged">
                            <Items>
                                <insite:ComboBoxOption Text="Search by Snapshot" Value="Select" Selected="true" />
                                <insite:ComboBoxOption Text="Search by Date Range" Value="Range" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div runat="server" id="SnapshotDateField" class="mb-2">
                        <insite:ComboBox runat="server" ID="SnapshotDate" EmptyMessage="Snapshot Date" />
                    </div>

                    <div runat="server" id="SnapshotDateSinceField" class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="SnapshotDateSince" EmptyMessage="Snapshot Date &ge;" />
                    </div>

                    <div runat="server" id="SnapshotDateBeforeField" class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="SnapshotDateBefore" EmptyMessage="Snapshot Date &lt;" />
                    </div>

                    <div runat="server" id="DepartmentLabelField" class="mb-2">
                        <insite:DepartmentLabelComboBox runat="server" ID="DepartmentLabel" EmptyMessage="Department Tag" />
                    </div>

                    <div runat="server" id="DepartmentField" class="mb-2">
                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" MaxSelectionCount="0" />
                    </div>

                    <div class="mb-2">
                        <insite:FindEntity runat="server" ID="UserIdentifier" MaxSelectionCount="0"
                            EntityName="User" PageSize='20' />
                    </div>

                </div>
                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="AchievementType" EmptyMessage="No Achievement Types"
                            AllowBlank="false" Multiple-ActionsBox="true" Multiple-CountAllFormat="All Achievement Types" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="StandardType" EmptyMessage="No Standard Types"
                            AllowBlank="false" Multiple-ActionsBox="true" Multiple-CountAllFormat="All Standard Types" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="DepartmentRole" EmptyMessage="Employment Type">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="None" Value="None" />
                                <insite:ComboBoxOption Text="Department Employment" Value="Department" Selected="true" />
                                <insite:ComboBoxOption Text="Organization Employment" Value="Organization" />
                                <insite:ComboBoxOption Text="Data Access" Value="Administration" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div runat="server" id="ScoreFromField" visible="false" class="mb-2">
                        <insite:NumericBox ID="ScoreFrom" runat="server" EmptyMessage="Score &ge;" />
                    </div>

                    <div  runat="server" id="ScoreThruField" visible="false" class="mb-2">
                        <insite:NumericBox ID="ScoreThru" runat="server" EmptyMessage="Score &le;" />
                    </div>

                </div>
            </div>

            <div class="mt-3">
	            <insite:FilterButton ID="SearchButton" runat="server" />
	            <insite:ClearButton ID="ClearButton" runat="server" />
            </div>
        </div>
    </div>
    <div class="col-3">
        <h4>Settings</h4>
        <insite:MultiComboBox ID="ShowColumns" runat="server" />
    </div>
    <div class="col-3">
        <h4>Saved Filters</h4>
        <uc:FilterManager runat="server" ID="FilterManager" />
    </div>
</div>
