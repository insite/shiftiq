<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Identities.Departments.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Department" />

    <div class="row mb-3">
        <div class="col-lg-12">
            <uc:Details runat="server" ID="DepartmentDetails" />
        </div>
    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Department" />
            <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" ConfirmText="Are you sure you want to delete this department?" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>
