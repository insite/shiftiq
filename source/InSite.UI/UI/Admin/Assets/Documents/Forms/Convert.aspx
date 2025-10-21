<%@ Page Language="C#" CodeBehind="Convert.aspx.cs" Inherits="InSite.UI.Admin.Assets.Documents.Convert" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<section class="container mb-2 mb-sm-0 pb-sm-5">

    <h1 runat="server" id="PageTitle" class="mb-0"></h1>
    <div runat="server" id="PageSubtitle" class="mb-4"></div>

    <uc:Detail runat="server" ID="Detail" />

</section>
</asp:Content>
