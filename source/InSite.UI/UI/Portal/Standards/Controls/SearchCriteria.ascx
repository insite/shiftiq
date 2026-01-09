<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Criteria" /></h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="StandardTitle" runat="server" MaxLength="100" EmptyMessage="Title" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="StandardLabel" runat="server" MaxLength="100" EmptyMessage="Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="StandardCode" runat="server" MaxLength="100" EmptyMessage="Code" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="AssetNumber" runat="server" EmptyMessage="Asset #" NumericMode="Integer" />
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>

</div>