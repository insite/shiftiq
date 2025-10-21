<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Candidates.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Filter" /></h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="FullName" runat="server" MaxLength="100" EmptyMessage="Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="City" runat="server" MaxLength="100" EmptyMessage="Current City" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="WillingToRelocate" EmptyMessage="Willing to Relocate">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Yes" Value="True" />
                                <insite:ComboBoxOption Text="No" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:SearchButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="Qualification" runat="server" MaxLength="100" EmptyMessage="Credentials" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="CurrentJobTitle" runat="server" MaxLength="100" EmptyMessage="Job Title" />
                    </div>

                    <div class="mb-2">
                        <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" EmptyMessage="Area of Interest" />
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>

</div>
