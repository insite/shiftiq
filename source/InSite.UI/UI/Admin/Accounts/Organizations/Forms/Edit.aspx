<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Accounts.Organizations.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Organization" />
    
    <div class="row mb-3">
        <div class="col-12">
            <uc:Detail runat="server" ID="Details" />
        </div>
    </div>
        
    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Organization" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>