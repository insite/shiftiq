<%@ Page Language="C#" CodeBehind="Propose.aspx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Forms.Propose" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HelpContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="GlossaryTerm" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-books me-1"></i>
            New Term
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <uc:Detail runat="server" ID="Detail" />

            </div>
        </div>
    </section>
        
    <div class="mb-3">
        <insite:SaveButton runat="server" ID="AddButton" ValidationGroup="GlossaryTerm" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>
