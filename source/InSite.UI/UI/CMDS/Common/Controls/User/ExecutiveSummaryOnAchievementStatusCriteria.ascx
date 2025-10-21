<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExecutiveSummaryOnAchievementStatusCriteria.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.Controls.ExecutiveSummaryOnAchievementStatusCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="SnapshotCriteriaType" ClientEvents-OnChange="executiveSummaryOnAchievementStatusCriteria.onSnapshotCriteriaTypeChanged">
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
                        <insite:DateTimeOffsetSelector runat="server" ID="SnapshotDateSince" EmptyMessage="Snapshot Date &ge;" Width="100%" />
                    </div>

                    <div runat="server" id="SnapshotDateBeforeField" class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="SnapshotDateBefore" EmptyMessage="Snapshot Date &lt;" Width="100%" />
                    </div>

                </div>
                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:FindDepartment runat="server" ID="DepartmentIdentifier" EmptyMessage="Department" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AchievementType" EmptyMessage="Achievement Type" />
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

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.executiveSummaryOnAchievementStatusCriteria = window.executiveSummaryOnAchievementStatusCriteria || {};

            $(document).ready(function () {
                var el = document.getElementById('<%= SnapshotCriteriaType.ClientID %>');
                instance.onSnapshotCriteriaTypeChanged.call(el);
            });

            instance.onSnapshotCriteriaTypeChanged = function () {
                var value = $(this).selectpicker('val');

                var $snapshotDateField = $('#<%= SnapshotDateField.ClientID %>');
                var $snapshotDateSinceField = $('#<%= SnapshotDateSinceField.ClientID %>');
                var $snapshotDateBeforeField = $('#<%= SnapshotDateBeforeField.ClientID %>');

                if (value === 'Range') {
                    $snapshotDateField.hide();
                    $snapshotDateSinceField.show();
                    $snapshotDateBeforeField.show();
                } else {
                    $snapshotDateField.show();
                    $snapshotDateSinceField.hide();
                    $snapshotDateBeforeField.hide();
                }
            };
        })();
    </script>
</insite:PageFooterContent>
