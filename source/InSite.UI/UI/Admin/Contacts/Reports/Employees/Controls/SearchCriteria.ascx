<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Contacts.Reports.Employees.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">

        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployeeName" EmptyMessage="Employee" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployeeEmail" EmptyMessage="Employee Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployeeJobTitle" EmptyMessage="Employee Job Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameMultiComboBox runat="server" ID="MembershipStatus" EmptyMessage="Membership Status" />
                    </div>

                    <div class="mb-2">
                        <insite:GenderComboBox runat="server" ID="EmployeeGender" AllowBlank="true" EmptyMessage="Employee Gender" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>

                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EmployeeJoinedSince" runat="server" EmptyMessage="Membership Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EmployeeJoinedBefore" runat="server" EmptyMessage="Membership Started Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EmployeeEndedSince" runat="server" EmptyMessage="Membership Ended Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EmployeeEndedBefore" runat="server" EmptyMessage="Membership Ended Before" />
                    </div>
                </div>

                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindGroup ID="EmployerGroupIdentifier" runat="server" EmptyMessage="Employer" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerNumber" EmptyMessage="Employer Number" MaxLength="8" />
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="EmployerDistrictIdentifier" EmptyMessage="District" />
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup ID="EmployeeGroupIdentifier" runat="server" EmptyMessage="Group" />
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
                        <insite:ComboBoxOption Text="Sort by First Name" Value="EmployeeFirstName, EmployeeLastName" />
                        <insite:ComboBoxOption Text="Sort by Last Name" Value="EmployeeLastName, EmployeeFirstName" />
                        <insite:ComboBoxOption Text="Sort by Employee Type" Value="EmployeeContactType" />
                        <insite:ComboBoxOption Text="Sort by Join Date" Value="EmployeeMemberStartDate desc" />
                        <insite:ComboBoxOption Text="Sort by Employer Number" Value="EmployerGroupNumber" />
                        <insite:ComboBoxOption Text="Sort by District, Roles, FullName" Value="EmployerDistrictName, RolesParticipationGroupName, EmployeeFullName" />
                        <insite:ComboBoxOption Text="Sort by District, FullName" Value="EmployerDistrictName, EmployeeFullName" />
                        <insite:ComboBoxOption Text="Sort by District, Employer, Join Date" Value="EmployerDistrictName, EmployerGroupName, EmployeeMemberStartDate" />
                        <insite:ComboBoxOption Text="Sort by District, Membership Status, FullName" Value="EmployerDistrictName, EmployeeProcessStep, EmployeeFullName" />
                        <insite:ComboBoxOption Text="Sort by Membership Status, FullName" Value="EmployeeProcessStep, EmployeeFullName" />
                        <insite:ComboBoxOption Text="Sort by District, Employer, Name" Value="EmployerDistrictName, EmployerGroupName, EmployeeFullName" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>