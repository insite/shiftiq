<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-4">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <cmds:FindCompany ID="CompanySelector" runat="server" EmptyMessage="Organization" />
                    </div>
                    <div class="mb-2">
                        <cmds:FindDepartment ID="DepartmentSelector" runat="server" EmptyMessage="Department" />
                    </div>
                    <div class="mb-2">
                        <cmds:FindProfile ID="ProfileSelector" runat="server" EmptyMessage="Profile" />
                    </div>
                    <div class="mb-2">
                        <cmds:FindPerson runat="server" ID="Person" EmptyMessage="Person" />
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