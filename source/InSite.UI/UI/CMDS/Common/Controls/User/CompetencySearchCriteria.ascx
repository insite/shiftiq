<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencySearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Talents.Competencies.CompetencySearchCriteria" %>

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
                        <insite:TextBox ID="Summary" runat="server" EmptyMessage="Summary" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="NumberOld" runat="server" EmptyMessage="Old Number" MaxLength="256" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <cmds:CompetencyCategorySelector ID="Category" runat="server" EmptyMessage="Category" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Description" runat="server" EmptyMessage="Description" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="DeletedSelector">
                            <Items>
                                <insite:ComboBoxOption Value="True" Text="Deleted" />
                                <insite:ComboBoxOption Value="False" Text="Not Deleted" Selected="true" />
                            </Items>
                        </insite:ComboBox>
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