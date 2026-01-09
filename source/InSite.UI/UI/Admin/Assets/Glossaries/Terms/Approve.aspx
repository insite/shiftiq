<%@ Page Language="C#" CodeBehind="Approve.aspx.cs" Inherits="InSite.UI.Admin.Assets.Glossaries.Terms.Forms.Approve" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/ApproveDetail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HelpContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-books"></i>
            Term
        </h2>

        <uc:Detail runat="server" ID="Detail" />
    </section>

    <div class="mb-3">
        <insite:Button runat="server" ID="ApproveButton" Text="Approve" Icon="fas fa-thumbs-up" ButtonStyle="Success" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
