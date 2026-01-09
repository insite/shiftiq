<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Sales.Packages.Forms.Create"  MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="~/UI/Admin/Sales/Packages/Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Package" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pallet me-1"></i>
            Package
        </h2>

        <div class="row">
            <div class="col-xxl-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Details</h3>

                        <uc:Details runat="server" ID="PackageDetails" />

                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Package" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>