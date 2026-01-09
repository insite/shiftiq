<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Utilities.Labels.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LabelName" EmptyMessage="Placeholder Name" MaxLength="128" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LabelTranslation" EmptyMessage="Placeholder Value" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:FindOrganization runat="server" ID="LabelOrganization" EmptyMessage="Organization" />
                    </div>                   
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
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