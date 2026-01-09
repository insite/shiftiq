<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileSearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileSearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="Number" runat="server" EmptyMessage="Number" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Description" runat="server" EmptyMessage="Description" MaxLength="256" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <cmds:ProfileVisibilitySelector ID="AccountScope" runat="server" EmptyMessage="Visibility" />
                    </div>

                    <div runat="server" id="CompanyPanel" class="mb-2">
                        <cmds:FindCompany ID="Company" runat="server" EmptyMessage="Organization" />
                    </div>

                    <div id="ParentProfilePanel" runat="server" class="mb-2">
                        <cmds:FindProfile ID="ParentProfile" runat="server" EmptyMessage="Parent Profile" />
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