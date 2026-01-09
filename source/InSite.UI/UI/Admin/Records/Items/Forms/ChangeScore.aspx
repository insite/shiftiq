<%@ Page Language="C#" CodeBehind="ChangeScore.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.ChangeScore" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ScoreDetail.ascx" TagName="ScoreDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Item" />

    <section runat="server" ID="ScorePanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Grade Item
        </h2>

        <uc:ScoreDetail runat="server" ID="Detail" />

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Item" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
