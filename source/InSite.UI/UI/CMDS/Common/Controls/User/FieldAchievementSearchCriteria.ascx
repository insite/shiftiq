<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldAchievementSearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.FieldAchievementSearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <cmds:AchievementTypeSelector ID="SubType" runat="server" EmptyMessage="Type" NullText="" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="IsTimeSensitive" runat="server">
                            <Items>
                                <insite:ComboBoxOption Text="All" Selected="True" />
                                <insite:ComboBoxOption Value="true" Text="Time-Sensitive" />
                                <insite:ComboBoxOption Value="false" Text="Not Time-Sensitive" />                    
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
                <div class="col-6">
                    <div id="CategoryPanel" runat="server" class="mb-2">
                        <cmds:TrainingCategorySelector ID="Category" runat="server" EmptyMessage="Category" />
                    </div>
                </div>
            </div>            

	        <insite:FilterButton ID="SearchButton" runat="server" />
	        <insite:ClearButton ID="ClearButton" runat="server" />
        </div>
    </div>
    <div class="col-6">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>