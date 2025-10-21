<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:WebSiteComboBox runat="server" ID="WebSiteId" EmptyMessage="Site" AllowBlank="true" />
                    </div>
    
                    <div class="mb-2">
                        <insite:WebPageTypeComboBox runat="server" ID="PageType" EmptyMessage="Type" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="PublicationStatus" EmptyMessage="Publication Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Published" />
                                <insite:ComboBoxOption Value="False" Text="Unpublished" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PageSlug" EmptyMessage="Slug" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Title" EmptyMessage="Title" MaxLength="256" />
                    </div>  
                    
                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="GroupIdentifier" EmptyMessage="Group Permissions" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcModifiedSince" runat="server" EmptyMessage="Last Modified Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcModifiedBefore" runat="server" EmptyMessage="Last Modified Before" />
                    </div>

                    <div class="mb-2">
                        <insite:WebPageTemplateComboBox runat="server" ID="ContentControl" AllowBlank="true" EmptyMessage="Content Control" />
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
        <div class="mb-3">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Page Slug" Value="PageSlug" />
                    <insite:ComboBoxOption Text="Sort by Page Title" Value="PageTitle,PageSlug" />
                    <insite:ComboBoxOption Text="Sort by Last Modified" Value="LastChangeTime desc,PageSlug" />
                </Items>
            </insite:ComboBox>
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
