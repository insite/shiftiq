<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Contacts.Reports.Employers.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">

        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerName" EmptyMessage="Employer Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>

            </div>
        </div>

    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-4">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Employer Name" Value="EmployerGroupName" />
                        <insite:ComboBoxOption Text="Sort by Employee Count" Value="EmployeeCount DESC" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>