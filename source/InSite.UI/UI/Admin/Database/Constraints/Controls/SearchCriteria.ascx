<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Utilities.Constraints.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">

            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SchemaName" EmptyMessage="Schema Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TableName" EmptyMessage="Table Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ColumnName" EmptyMessage="Column Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ForeignTableName" EmptyMessage="Foreign Table Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:ComboBox ID="EnforcedInclusion" runat="server" EmptyMessage="Enforced and Not Enforced">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Only" Text="Enforced" />
                                <insite:ComboBoxOption Value="Exclude" Text="Not Enforced" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div>
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