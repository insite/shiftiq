<%@ Page Language="C#" CodeBehind="ChangeCategory.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.ChangeCategory" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CategoryDetail.ascx" TagName="CategoryDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Item" />

    <section runat="server" ID="CategoryPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Grade Category
        </h2>

        <uc:CategoryDetail runat="server" ID="Detail" />
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Item" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
