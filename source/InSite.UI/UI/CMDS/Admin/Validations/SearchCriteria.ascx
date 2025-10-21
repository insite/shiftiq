<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Validations.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:StandardTypeComboBox runat="server" ID="StandardType" EmptyMessage="Standard Type" />
                    </div>
                    <div class="mb-2">
                        <insite:FindStandard runat="server" ID="StandardIdentifier" EmptyMessage="Standard" TextType="CodeTitle" />
                    </div>
                    <div class="mb-2">
                        <insite:FindDepartment runat="server" ID="DepartmentIdentifier" EmptyMessage="Department" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="UserIdentifier" EmptyMessage="User" />
                    </div>
                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="ValidatorUserIdentifier" EmptyMessage="Validator" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <cmds:SelfAssessmentStatusSelector ID="SelfAssessmentStatus" runat="server" EmptyMessage="Self-Assessment Status" />
                    </div>
                    <div class="mb-2">
                        <cmds:CompetencyStatusSelector ID="ValidationStatus" runat="server" EmptyMessage="Validation Status" />
                    </div>
                </div>
            </div>            

	        <insite:FilterButton ID="SearchButton" runat="server" />
	        <insite:ClearButton ID="ClearButton" runat="server" />
        </div>
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>