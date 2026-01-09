<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Publications.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="PageTitle" runat="server" EmptyMessage="Page Title" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="PageIsHidden" 
                            TrueText="Hidden" FalseText="Not Hidden" 
                            EmptyMessage="Portal Visibility" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="FormName" runat="server" EmptyMessage="Form Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:NumericBox ID="FormAsset" runat="server" EmptyMessage="Asset #" NumericMode="Integer" />
                    </div>
                </div>
            </div>
            
            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-3">
        <div>
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
