<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.AchievementSearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <cmds:OrganizationScopeSelector runat="server" ID="AchievementScope" EmptyMessage="Achievement Scope" />
                    </div>

                    <div class="mb-2">
                        <cmds:AchievementTypeSelector ID="AchievementType" runat="server" EmptyMessage="Achievement Type" NullText="" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Achievement Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Description" runat="server" EmptyMessage="Achievement Description" MaxLength="256" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsTimeSensitive" EmptyMessage="Time-Sensitivity">
                            <Items>
                                <insite:ComboBoxOption Value="" />
                                <insite:ComboBoxOption Value="True" Text="Time-Sensitive" />
                                <insite:ComboBoxOption Value="False" Text="Not Time-Sensitive" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="AllowSelfDeclaration" EmptyMessage="Self-Declaration">
                            <Items>
                                <insite:ComboBoxOption Value="" />
                                <insite:ComboBoxOption Value="True" Text="Allow self-declaration" />
                                <insite:ComboBoxOption Value="False" Text="Disallow self-declaration" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <cmds:TrainingCategorySelector runat="server" ID="Category" EmptyMessage="Category" />

                </div>
            </div>            

	        <insite:FilterButton runat="server" ID="SearchButton" />
	        <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-6">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
