<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Sales.Taxes.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Detail" Src="./Controls/Detail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Tax" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-badge-percent me-1"></i>
            Taxes
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:Detail runat="server" ID="Detail" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Tax" />
            <insite:CancelButton runat="server" ID="CancelLink" />
        </div>
    </div>

</asp:Content>