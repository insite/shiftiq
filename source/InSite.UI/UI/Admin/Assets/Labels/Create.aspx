<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Utilities.Labels.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Label" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-tag me-1"></i>
            User Interface (UI) Label
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <uc:Detail runat="server" ID="Detail" />

            </div>
        </div>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Label" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</div>
</asp:Content>
