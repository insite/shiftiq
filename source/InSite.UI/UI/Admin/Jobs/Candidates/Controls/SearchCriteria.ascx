<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Jobs.Candidates.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:GroupMultiComboBox runat="server" ID="DepartmentGroupIdentifier" EmptyMessage="Department" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Name" EmptyMessage="Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="NameFilterType" runat="server">
                            <Items>
                                <insite:ComboBoxOption Value="Exact" Text="Match Exact Spelling" Selected="true" />
                                <insite:ComboBoxOption Value="Similar" Text="Similar Pronunciation" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Email" EmptyMessage="Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CityName" EmptyMessage="City" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="ActivelySeeking" runat="server" EmptyMessage="Actively Seeking Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Actively Seeking" />
                                <insite:ComboBoxOption Value="False" Text="Not Actively Seeking" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateLastActive" runat="server" EmptyMessage="Date Last Active" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox ID="Approved" runat="server" EmptyMessage="Approved Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Approved" />
                                <insite:ComboBoxOption Value="False" Text="Not Approved" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CreatedSince" runat="server" EmptyMessage="Date Registered Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CreatedBefore" runat="server" EmptyMessage="Date Registered Before" />
                    </div>

                    <div class="mb-2">
                        <insite:FindStandard runat="server" ID="OccupationalProfile" TextType="Title" EmptyMessage="Occupational Profile" />
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
