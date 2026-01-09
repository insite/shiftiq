<%@ Page Language="C#" CodeBehind="Cart.aspx.cs" Inherits="InSite.UI.Portal.Billing.Cart" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../../Portal/Home/Management/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Billing/Controls/CartDetail.ascx" TagName="CartDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="row">
        <div runat="server" id="PageHeader">
            <h2>Cart</h2>
            <hr class="my-1 text-secondary">
        </div>
        <div class="col-lg-12">
            <div class="pt-0 p-4 mb-4">

                <insite:UpdatePanel runat="server">
                    <ContentTemplate>

                        <uc:CartDetail runat="server" ID="CartDetail" />

                        <div class="d-flex justify-content-between mt-4">
                            <a runat="server" id="ContinueShopping" class="btn btn-outline-primary rounded-pill px-4">Continue Shopping</a>
                            <a runat="server" id="CheckoutLink" href="/ui/portal/billing/checkout" class="btn btn-primary rounded-pill px-4">Checkout</a>
                        </div>

                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </div>
</asp:Content>
