<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Invoices.Products.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="ProductName" runat="server" EmptyMessage="Product Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="ProductDescription" runat="server" EmptyMessage="Product Description" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ProductTypeComboBox ID="ProductType" runat="server" EmptyMessage="Product Type" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>    
                <div class="col-6">
                    <div class="mb-2">
                        <insite:BooleanComboBox ID="ProductPublishedStatus" runat="server" EmptyMessage="Publication" TrueText="Published" FalseText="Unpublished" AllowBlank="true"/>
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
