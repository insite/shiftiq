<%@ Page Language="C#" CodeBehind="Reject.aspx.cs" Inherits="InSite.UI.Admin.Assets.Glossaries.Terms.Forms.Reject" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
        <insite:Button runat="server" ID="RejectButton" Text="Reject" Icon="fas fa-thumbs-down" ButtonStyle="Danger" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
