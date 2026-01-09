<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Filter" /></h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox ID="FullName" runat="server" MaxLength="100" EmptyMessage="Full Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Email" runat="server" MaxLength="100" EmptyMessage="Email" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="PersonCode" runat="server" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:SearchButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-4">
                        <insite:CheckBox runat="server" ID="CompletedCasesOnly" Text="Completed Cases Only" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector runat="server" ID="IssueStatusEffectiveSince" EmptyMessage="Updated Since" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="IssueType" EmptyMessage="Case Type" />
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
