<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CatalogDetail" %>

<%@ Register Src="~/UI/Portal/Billing/Controls/PriceSelector.ascx" TagName="PriceSelector" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CatalogGrid.ascx" TagName="CatalogGrid" TagPrefix="uc" %>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .subscribe-banner {
          background: var(--bs-primary, #2f6e8e);
          border-left: .5rem solid rgba(255,255,255,.35);
        }
        .subscribe-banner .fa-solid{ opacity: .9; }
    </style>
</insite:PageHeadContent>

<uc:PriceSelector runat="server" ID="PriceSelector" />

<div class="row pb-2 pt-5 pb-sm-4">
    <div class="col-lg-12">

        <asp:Panel runat="server" ID="SubscribeBanner" CssClass="subscribe-banner rounded-1 p-4 my-4" Visible="false">
            <div class="d-flex align-items-start gap-3">
            <i class="fa-solid fa-angles-right fa-2x text-white d-none d-sm-inline"></i>

            <div class="text-white">
                <div class="h5 text-white fw-bold mb-1">You have selected: Subscribe &amp; Choose Later</div>
                <div class="mb-0 text-white">
                Select a Package below to be added to your SkillsCheck account upon checkout.
                There’s no expiry date to make your selection.
                </div>
            </div>
            </div>
        </asp:Panel>

        <uc:CatalogGrid runat="server" ID="CatalogGrid" />
        
    </div>
</div>
