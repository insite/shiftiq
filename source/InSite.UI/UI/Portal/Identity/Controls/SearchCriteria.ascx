<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Desktops.Design.Users.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Criteria" /></h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="MatchNamesWith" EnableTranslation="true">
                            <Items>
                                <insite:ComboBoxOption Text="Exact Spelling" Value="Exact" />
                                <insite:ComboBoxOption Text="Similar Pronunciation" Value="Similar" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    
                    <div class="mb-2">
                        <insite:TextBox ID="Name" runat="server" MaxLength="128" EmptyMessage="Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Email" runat="server" MaxLength="128" EmptyMessage="Email" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="SessionCount" runat="server" NumericMode="Integer" EmptyMessage="Session Count" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:DateSelector ID="LastAuthenticatedSince" runat="server" EmptyMessage="Last Authenticated After" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="LastAuthenticatedBefore" runat="server" EmptyMessage="Last Authenticated Before" />
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="col-3">
        <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
        <uc:FilterManager runat="server" ID="FilterManager" />
    </div>

</div>