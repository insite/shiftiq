<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Courses.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">

                    <div class="mb-2">
                        <insite:CatalogComboBox runat="server" ID="CatalogIdentifier" EmptyMessage="Catalogue" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CourseName" EmptyMessage="Course Name" MaxLength="200" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CourseLabel" EmptyMessage="Course Tag" MaxLength="20" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="CourseAsset" EmptyMessage="Asset Number" NumericMode="Integer" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="HasWebPage" EmptyMessage="Publication Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Published" />
                                <insite:ComboBoxOption Value="False" Text="Unpublished" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="WebPageAuthoredSince" runat="server" EmptyMessage="Published Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="WebPageAuthoredBefore" runat="server" EmptyMessage="Published Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GradebookTitle" EmptyMessage="Gradebook Name" MaxLength="256" />
                    </div>

                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CatalogVisibility" EmptyMessage="Catalogue Visibility">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Visible" />
                                <insite:ComboBoxOption Value="False" Text="Hidden" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CatalogAccess" EmptyMessage="Access">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Restricted" />
                                <insite:ComboBoxOption Value="False" Text="Unrestricted" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="PermissionGroupIdentifiers" EmptyMessage="Group Permissions" MaxSelectionCount="0" />
                    </div>

                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mb-3">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
