<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ContainerTypeComboBox runat="server" ID="ContainerType" EmptyMessage="Container Type" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ContainerIdentifier" EmptyMessage="Container Identifier" MaxLength="256" />
                    </div>                   

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ContentLabel" EmptyMessage="Label" MaxLength="256" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:LanguageComboBox runat="server" ID="ContentLanguage" EmptyMessage="Language" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TextHTML" EmptyMessage="Text/Html" MaxLength="256" />
                    </div>                    
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-2">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Content" Value="ContentSnip" />
                        <insite:ComboBoxOption Text="Sort by Label" Value="ContentLabel" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>       
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
