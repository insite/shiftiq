<%@ Page Language="C#" CodeBehind="AddCalculation.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.AddCalculation" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CalculationDetail.ascx" TagName="CalculationDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Item" />

    <section runat="server" ID="CalculationPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Calculation
        </h2>

        <uc:CalculationDetail runat="server" ID="Detail" />
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Item" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
