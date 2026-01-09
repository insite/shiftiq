<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-4">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SpecName" EmptyMessage="Specification Name" MaxLength="200" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SpecType" EmptyMessage="Specification Type" MaxLength="8" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SpecAsset" EmptyMessage="Asset Number" MaxLength="10" />
                    </div>
                </div>                
            </div>
            
            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-4">
        <div>
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-2">                
                <insite:ComboBox ID="OrderBy" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Bank and Specification Name" Value="Bank.BankName, SpecName" />
                        <insite:ComboBoxOption Text="Sort by Specification Name" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>       
    </div>
    <div class="col-4">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
