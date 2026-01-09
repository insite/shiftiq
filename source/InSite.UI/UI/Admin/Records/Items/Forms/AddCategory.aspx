<%@ Page Language="C#" CodeBehind="AddCategory.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.AddCategory" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CategoryDetail.ascx" TagName="CategoryDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Item" />

    <section runat="server" ID="CategoryPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Category
        </h2>

        <uc:CategoryDetail runat="server" ID="Detail" />
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Item" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
