<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:FindJournalSetup runat="server" ID="JournalSetupIdentifier" EmptyMessage="Logbook" />
                    </div>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="DepartmentUserUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="DepartmentUserUpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:FindDepartment runat="server" ID="DepartmentIdentifier" EmptyMessage="Department" />
                            </div>
    
                            <div class="mb-2">
                                <insite:FindPerson runat="server" ID="UserIdentifier" EmptyMessage="Learner" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CreatedSince" runat="server" EmptyMessage="Created &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CreatedBefore" runat="server" EmptyMessage="Created &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ValidationStatus" EmptyMessage="Validation Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Validated" />
                                <insite:ComboBoxOption Value="False" Text="Not Validated" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TrainingTypeComboBox runat="server" ID="TrainingType" EmptyMessage="Training Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Employer" EmptyMessage="Employer" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Supervisor" EmptyMessage="Supervisor" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="StartDate" runat="server" EmptyMessage="Start Date" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="EndDate" runat="server" EmptyMessage="End Date" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="Hours" EmptyMessage="Hours" NumericMode="Integer" />
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
