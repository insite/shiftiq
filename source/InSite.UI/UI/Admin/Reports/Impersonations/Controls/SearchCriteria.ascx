<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Reports.Impersonations.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="SinceDate" runat="server" EmptyMessage="Date Since" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="BeforeDate" runat="server" EmptyMessage="Date Before" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GroupLabel" EmptyMessage="Group Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-6">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>