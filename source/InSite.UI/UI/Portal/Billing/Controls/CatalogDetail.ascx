<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CatalogDetail" %>

<%@ Register Src="~/UI/Portal/Billing/Controls/PriceSelector.ascx" TagName="PriceSelector" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CatalogGrid.ascx" TagName="CatalogGrid" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-8">
        <uc:PriceSelector runat="server" ID="PriceSelector" />
    </div>
    <div runat="server" id="FilterPanel" class="col-lg-4 mt-3 mt-lg-0 catalog-filters">
        <div class="d-grid gap-2">
            <insite:ItemIdComboBox runat="server" ID="IndustryFilter" EmptyMessage="Industry" ButtonSize="Small">
                <Settings CollectionName="Sales/Industry/Name" UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" />
            </insite:ItemIdComboBox>
            <insite:ItemIdComboBox runat="server" ID="OccupationFilter" EmptyMessage="Role" ButtonSize="Small">
                <Settings CollectionName="Sales/Occupation/Name" UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" />
            </insite:ItemIdComboBox>
            <insite:ItemIdComboBox runat="server" ID="LevelFilter" EmptyMessage="Level" ButtonSize="Small">
                <Settings CollectionName="Sales/Level/Name" UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" />
            </insite:ItemIdComboBox>
            <insite:Button runat="server" ID="ClearFiltersButton" CssClass="mt-1" Icon="fas fa-arrows-rotate" Text="Clear filters" 
                ButtonSize="Small" ButtonStyle="OutlineSecondary" />
        </div>
    </div>
</div>

<div class="row pb-2 pt-3 pb-sm-4">
    <div class="col-lg-12">

        <insite:Alert runat="server" ID="CartWarning" />

        <asp:Panel runat="server" ID="SubscribeBanner" CssClass="subscribe-banner rounded-1 p-4 my-4" Visible="false">
            <div class="d-flex align-items-start gap-3">
            <i class="fa-solid fa-angles-right fa-2x text-white d-none d-sm-inline"></i>

            <div class="text-white">
                <div class="h5 text-white fw-bold mb-1">You have selected: <asp:Literal runat="server" ID="SubscribeBannerTitle" /></div>
                <div class="mb-0 text-white">
                    <asp:Literal runat="server" ID="SubscribeBannerText" />
                </div>
            </div>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="SelectBanner" CssClass="subscribe-banner rounded-1 p-4 mb-4" Visible="false">
            <div class="d-flex align-items-start gap-3">
                <i class="fa-solid fa-angles-right fa-2x text-white d-none d-sm-inline"></i>

                <div class="text-white">
                    <div class="h5 text-white fw-bold mb-1">
                        You have <asp:Label runat="server" ID="CreditCountSpan" CssClass="CatalogDetail_CreditCount" /> <asp:Literal runat="server" ID="CreditText" />!
                    </div>
                    <div class="mb-0 text-white">
                        Select your SkillsChecks from the Catalogue below and click Save Changes.
                        They’ll be automatically added to your account and activated on your Management Dashboard.
                    </div>
                </div>
            </div>
        </asp:Panel>

        <insite:SaveButton runat="server"
            ID="SaveButton"
            Text="Save Changes"
            CssClass="CatalogDetail_SaveButton disabled mb-3"
            Visible="false"
        />

        <div class="catalog-filter-empty text-muted py-4 text-center d-none">
            No SkillsCheck match the selected filters.
        </div>

        <uc:CatalogGrid runat="server" ID="CatalogGrid" />
        
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .catalog-filters .bootstrap-select {
            max-width: 100%;
            min-width: 0;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="FilterScript">
    <script type="text/javascript">
        (function () {
            if (!document.getElementById('<%= FilterPanel.ClientID %>'))
                return;

            const filters = [
                { id: '<%= IndustryFilter.ClientID %>', attr: 'industry' },
                { id: '<%= OccupationFilter.ClientID %>', attr: 'occupation' },
                { id: '<%= LevelFilter.ClientID %>', attr: 'level' }
            ];

            document.addEventListener('DOMContentLoaded', applyFilters);

            document.getElementById('<%= ClearFiltersButton.ClientID %>').addEventListener('click', onClearClick);

            for (let i = 0; i < filters.length; i++) {
                const f = filters[i];
                const el = document.getElementById(f.id);

                el.addEventListener('change', applyFilters);
            }

            function onClearClick(e) {
                e.preventDefault();

                for (let i = 0; i < filters.length; i++) {
                    const $select = $('#' + filters[i].id);
                    $select.selectpicker('val', '');
                }

                applyFilters();
            }

            function applyFilters() {
                const values = filters.map(f => ({ attr: f.attr, value: getValue(f.id) }));
                const cards = document.querySelectorAll('.catalog-grid .product-card');

                let visible = 0;

                cards.forEach(card => {
                    const match = values.every(v => !v.value || (card.dataset[v.attr] || '').toLowerCase() === v.value);

                    card.classList.toggle('d-none', !match);

                    if (match)
                        visible++;
                });

                const emptyMsg = document.querySelector('.catalog-filter-empty');
                emptyMsg.classList.toggle('d-none', visible > 0 || cards.length === 0);
            }

            function getValue(id) {
                const el = document.getElementById(id);
                return (el.value || '').toLowerCase();
            }
        })();
    </script>
</insite:PageFooterContent>
