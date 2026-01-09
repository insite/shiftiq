<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanySearchCriteria.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.CompanySearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-4">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:TextBox ID="Name" runat="server" EmptyMessage="Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Archived" EmptyMessage="Account Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Exclude" Text="Open/Active" />
                                <insite:ComboBoxOption Value="Only" Text="Closed/Archived" />
                            </Items>
                        </insite:ComboBox>
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
        <h4>Settings</h4>
        <insite:MultiComboBox ID="ShowColumns" runat="server" />
    </div>
    <div class="col-3">
        <h4>Saved Filters</h4>
        <uc:FilterManager runat="server" ID="FilterManager" />
    </div>
</div>