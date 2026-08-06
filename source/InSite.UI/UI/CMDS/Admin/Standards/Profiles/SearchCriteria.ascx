<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Profiles.ProfileSearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <cmds:OrganizationScopeSelector runat="server" ID="ProfileScope" EmptyMessage="Scope" />
                    </div>

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
                        <cmds:FindProfile runat="server" ID="ParentProfile" EmptyMessage="Parent" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="HasParent" EmptyMessage="Hierarchy">
                            <Items>
                                <insite:ComboBoxOption Value="" />
                                <insite:ComboBoxOption Value="False" Text="Top-level" />
                                <insite:ComboBoxOption Value="True" Text="Child" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsDiverged" EmptyMessage="Divergence">
                            <Items>
                                <insite:ComboBoxOption Value="" />
                                <insite:ComboBoxOption Value="True" Text="Diverged" />
                                <insite:ComboBoxOption Value="False" Text="Aligned" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                </div>
            </div>

            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
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
