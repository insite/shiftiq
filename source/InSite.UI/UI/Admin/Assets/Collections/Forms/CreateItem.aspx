<%@ Page Language="C#" CodeBehind="CreateItem.aspx.cs" Inherits="InSite.Admin.Utilities.Collections.Forms.CreateItem" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ItemDetail.ascx" TagName="ItemDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CollectionItem" />

    <div class="row mb-3">
        <div class="col-12">

            <section runat="server" ID="GeneralSection">
                <h2 class="h4 mb-3">
                    <i class="far fa-album me-1"></i>
                    Collection Item
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ItemDetail runat="server" ID="ItemDetail" />
                    </div>
                </div>
            </section>

        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CollectionItem" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
