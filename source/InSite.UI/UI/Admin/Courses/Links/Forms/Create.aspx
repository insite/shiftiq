<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Courses.Links.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Lti Link" />

    <section runat="server" ID="LinkSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-plug me-1"></i>
            Link
        </h2>

        <uc:Detail runat="server" ID="Detail" />
    </section>

    <div class="row mb-3">
        <div class="col-md-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Lti Link" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


</div>
</asp:Content>
