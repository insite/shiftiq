<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Sales.Packages.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ProductName" EmptyMessage="Package Title" MaxLength="100" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ProductDescription" EmptyMessage="Package Description" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="ProductQuantity" EmptyMessage="Number of Packages" NumericMode="Integer" MinValue="1" />
                    </div>
    
                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="ProductPrice" EmptyMessage="Package Price" DecimalPlaces="2" MaxValue="999999.99" MinValue="0" />
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
