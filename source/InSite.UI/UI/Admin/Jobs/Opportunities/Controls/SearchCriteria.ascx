<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Jobs.Opportunities.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:OccupationListComboBox runat="server" ID="OccupationStandardIdentifier" EmptyMessage="Occupation Area" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobTitle" EmptyMessage="Job Title" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerName" EmptyMessage="Employer Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PositionType" EmptyMessage="Position Type" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobLocation" EmptyMessage="Job Location" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobType" EmptyMessage="Job Type" MaxLength="256" />
                    </div>

 
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>

                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox ID="IsPublished" runat="server" EmptyMessage="Published">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Published" />
                                <insite:ComboBoxOption Value="False" Text="Not Published" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DatePublishedSince" runat="server" EmptyMessage="Published Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DatePublishedBefore" runat="server" EmptyMessage="Published Before" />
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
