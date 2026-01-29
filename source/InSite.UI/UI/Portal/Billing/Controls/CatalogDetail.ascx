<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CatalogDetail" %>

<%@ Register Src="~/UI/Portal/Billing/Controls/PriceSelector.ascx" TagName="PriceSelector" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CatalogGrid.ascx" TagName="CatalogGrid" TagPrefix="uc" %>

<uc:PriceSelector runat="server" ID="PriceSelector" />

<div class="row pb-2 pt-3 pb-sm-4">
    <div class="col-lg-12">

        <insite:Alert runat="server" ID="CartWarning" />

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

        <asp:Panel runat="server" ID="SelectBanner" CssClass="subscribe-banner rounded-1 p-4 mb-4" Visible="false">
            <div class="d-flex align-items-start gap-3">
                <i class="fa-solid fa-angles-right fa-2x text-white d-none d-sm-inline"></i>

                <div class="text-white">
                    <div class="h5 text-white fw-bold mb-1">
                        You have <asp:Label runat="server" ID="CreditCountSpan" CssClass="CatalogDetail_CreditCount" /> <asp:Literal runat="server" ID="CreditText" />!
                    </div>
                    <div class="mb-0 text-white">
                        Select your SkillsChecks from the Catalog below and click Save Changes.
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

        <uc:CatalogGrid runat="server" ID="CatalogGrid" />
        
    </div>
</div>
