<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Persons.PersonSearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-lg-3">
        <div id="toolbox" class="toolbox-section">
        <h3>Criteria</h3>

        <div class="mb-2">
            <insite:TextBox ID="Name" runat="server" EmptyMessage="Name" MaxLength="256" />
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="NameFilterType" runat="server">
                <Items>
                    <insite:ComboBoxOption Value="Similar" Text="Match similar pronunciation" Selected="True" />
                    <insite:ComboBoxOption Value="Exact" Text="Match exact spelling" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:TextBox ID="Email" runat="server" EmptyMessage="Email" MaxLength="256" />
        </div>

        <div class="mb-2">
            <insite:TextBox ID="PersonCode" runat="server" EmptyMessage="Person Code" MaxLength="20" />
        </div>

        <div class="mb-2">
            <insite:TextBox ID="PersonType" runat="server" EmptyMessage="Person Type" MaxLength="20" />
        </div>

        <div class="mt-3">
            <insite:FilterButton ID="SearchButton" runat="server" />
            <insite:ClearButton ID="ClearButton" runat="server" />
        </div>
        </div>
    </div>

    <div class="col-lg-3">

        <h3>&nbsp;</h3>

        <div class="mb-2">
            <cmds:RoleSelector ID="Role" runat="server" EmptyMessage="Role" DropDown-Width="250" />
        </div>

        <div class="mb-2">
            <cmds:FindCompany ID="Company" runat="server" EmptyMessage="Organization" PageSize="10" />
        </div>

        <div class="mb-2">
            <cmds:FindDepartment ID="Department" runat="server" EmptyMessage="Department" PageSize="10" />
        </div>

        <div id="PersonAssignmentPanel" runat="server" class="mb-2">
            <insite:ComboBox ID="PersonAssignment" runat="server" EmptyMessage="Membership Function">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="Organization" Text="Organization Employment" />
                    <insite:ComboBoxOption Value="Department" Text="Department Employment" />
                    <insite:ComboBoxOption Value="Administration" Text="Data Access" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:TextBox ID="EmployeeType" runat="server" EmptyMessage="Employee Type" MaxLength="16" />
        </div>

    </div>

    <div class="col-lg-3">

        <h3>&nbsp;</h3>

        <div class="mb-2">
            <cmds:PersonStatusSelector ID="Status" runat="server" EmptyMessage="Login Status" />
        </div>

        <div id="ArchivedUsersPanel" runat="server" class="mb-2">
            <insite:BooleanComboBox ID="IsArchived" runat="server" EmptyMessage="Archive Status"
                FalseText="Hide archived users"
                TrueText="Show archived users" />
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="EmailStatus" runat="server" EmptyMessage="Email Status">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="No Email" Text="No Email" />
                    <insite:ComboBoxOption Value="Valid Email" Text="Valid Email" />
                    <insite:ComboBoxOption Value="Invalid Email" Text="Invalid Email" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:BooleanComboBox ID="IsCmdsAccessGranted" runat="server"
                TrueText="CMDS"
                FalseText="Skills Passport" />
        </div>

    </div>

    <div class="col-lg-3">

        <h3>Settings</h3>
        <insite:MultiComboBox ID="ShowColumns" runat="server" />

        <h3 class="mt-4">Saved Filters</h3>
        <uc:FilterManager runat="server" ID="FilterManager" />

    </div>

</div>