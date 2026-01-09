<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyName" EmptyMessage="Organization Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Code" EmptyMessage="Organization Code" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="DomainName" EmptyMessage="Organization Domain" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsClosed" EmptyMessage="Organization Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="False" Text="Open" />
                                <insite:ComboBoxOption Value="True" Text="Closed" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
                <div class="col-6">
                    
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