<%@ Control Language="C#" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Integrations.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox">

            <h4>Criteria</h4>

            <div class="row">

                <div class="col-5">

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="StartedSince" runat="server" EmptyMessage="Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="StartedBefore" runat="server" EmptyMessage="Started Before" />
                    </div>

                </div>

                <div class="col-7">
                    
                    <div class="mb-2">
                        <insite:TextBox ID="RequestUri" runat="server" EmptyMessage="Request URL" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="RequestData" runat="server" EmptyMessage="Request Data" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="RequestDirection" EmptyMessage="Request Direction" TrueText="In" FalseText="Out" />
                    </div>

                </div>
            </div>

            <div class="mb-2">
                <insite:FilterButton runat="server" ID="SearchButton" />
                <insite:ClearButton runat="server" ID="ClearButton" />
            </div>

        </div>
    </div>
    <div class="col-3">       
        <div class="mb-4">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>