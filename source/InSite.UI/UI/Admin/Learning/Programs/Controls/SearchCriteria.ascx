<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="ProgramName" runat="server" EmptyMessage="Program Name" MaxLength="200" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:TextBox ID="ProgramCode" runat="server" EmptyMessage="Program Code" MaxLength="20" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="ProgramDescription" runat="server" EmptyMessage="Program Description" MaxLength="500" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" EmptyMessage="Department" />
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

