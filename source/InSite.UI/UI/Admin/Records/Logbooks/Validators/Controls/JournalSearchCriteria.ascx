<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JournalSearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Records.Validators.Controls.JournalSearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:FindJournalSetup runat="server" ID="JournalSetupIdentifier" EmptyMessage="Logbook" />
                    </div>
    
                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="UserIdentifier" EmptyMessage="Learner" />
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
