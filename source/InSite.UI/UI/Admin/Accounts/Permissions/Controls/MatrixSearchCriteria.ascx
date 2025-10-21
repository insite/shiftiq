<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatrixSearchCriteria.ascx.cs" Inherits="InSite.Admin.Accounts.Permissions.Controls.MatrixSearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">

        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-6">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:GroupTypeComboBox runat="server" ID="GroupType" EmptyMessage="Group Type" />
                            </div>

                            <div class="mb-2">
                                <insite:FindGroup runat="server" ID="GroupIdentifier" EmptyMessage="Group" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mb-2">
                        <insite:FindPermission runat="server" ID="PermissionIdentifier" EmptyMessage="Toolkit" />
                    </div>

                    <div class="mb-2">
	                    <insite:FilterButton ID="SearchButton" runat="server" />
	                    <insite:ClearButton ID="ClearButton" runat="server" />
                    </div>
                </div>

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