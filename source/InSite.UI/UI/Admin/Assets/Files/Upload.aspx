<%@ Page CodeBehind="Upload.aspx.cs" Inherits="InSite.UI.Admin.Assets.Files.Upload" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="UploadDetail" Src="./Controls/UploadDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="FileAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="File" />

    <uc:UploadDetail runat="server" ID="Detail" />

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="File" DisableAfterClick="true" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>