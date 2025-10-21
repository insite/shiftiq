<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Invoices.Products.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Product" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pallet me-1"></i>
            Product
        </h2>

        <div class="row">

            <div class="col-xxl-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:Details runat="server" ID="ProductDetails" />
                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Product" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


</asp:Content>
