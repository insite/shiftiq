<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Talents.EmployeeCompetencies.EmployeeCompetencySearchCriteria" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div runat="server" id="CompanyPanel" class="mb-2">
                        <cmds:FindCompany ID="OrganizationIdentifier" runat="server" EmptyMessage="Organization" AllowClear="false" />
                        <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" FieldName="Organization" ValidationGroup="SearchCriteria" Display="None" RenderMode="Exclamation" />
                    </div>
                    <div runat="server" id="EmployeePanel" class="mb-2">
                        <cmds:FindPerson ID="Employee" runat="server" EmptyMessage="Employee" />
                    </div>
                    <div id="ProfilePanel" runat="server" class="mb-2">
                        <cmds:FindProfile ID="CurrentProfile" runat="server" EmptyMessage="Profile" />
                    </div>
                    <div id="DepartmentPanel" runat="server" class="mb-2">
                        <cmds:FindDepartment ID="Department" runat="server" EmptyMessage="Department" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="Number" runat="server" EmptyMessage="Competency #" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="NumberOld" runat="server" EmptyMessage="Old Competency #" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="Keyword" runat="server" EmptyMessage="Keyword" MaxLength="256" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <cmds:CompetencyCategorySelector ID="Category" runat="server" EmptyMessage="Category" />
                    </div>
                    <div class="mb-2">
                        <cmds:CompetencyPrioritySelector ID="Priority" runat="server" EmptyMessage="Priority" />
                    </div>
                    <div class="mb-2">
                        <cmds:SelfAssessmentStatusSelector ID="SelfAssessmentStatus" runat="server" EmptyMessage="Self-Assessment" />
                    </div>
                    <div class="mb-2">
                        <cmds:CompetencyStatusSelector ID="Status" runat="server" EmptyMessage="Validation" />
                    </div>
                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="IsValidated" TrueText="Validated" FalseText="Not Validated" />
                    </div>
                    <div id="SearchModePanel" runat="server" class="mb-2">
                        <insite:ComboBox ID="SearchMode" runat="server">
                            <Items>
                                <insite:ComboBoxOption Value="Person" Text="Person Competencies" Selected="True" />
                                <insite:ComboBoxOption Value="Group" Text="Group Competencies" />                    
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div id="GroupManagerPanel" runat="server" class="mb-2">
                        <cmds:FindPerson ID="GroupManagerID" runat="server" EmptyMessage="Supervisor / Manager" />
                    </div>
                </div>
            </div>            

            <insite:FilterButton ID="SearchButton" runat="server" CausesValidation="true" ValidationGroup="SearchCriteria" />
            <insite:ClearButton ID="ClearButton" runat="server" />
        </div>
    </div>
    <div class="col-6">       
        <div>
            <h4>Settings</h4>
            <insite:BooleanComboBox runat="server" ID="ShowValidationHistory" TrueText="Show Validation History" FalseText="Hide Validation History" AllowBlank="false" />
        </div>
    </div>
</div>