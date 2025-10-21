<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Payments.Discounts.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Discount" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-badge-percent me-1"></i>
            Discount
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <uc:Details runat="server" ID="DiscountDetails" />
                    </div>
                </div>
            </div>
        </div>
    </section>
    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Discount" />
            <insite:CancelButton runat="server" ID="CancelLink" />
        </div>
    </div>

</asp:Content>
