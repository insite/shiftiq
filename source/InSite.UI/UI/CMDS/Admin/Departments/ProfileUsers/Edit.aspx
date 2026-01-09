<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="Detail.ascx" TagName="Detail" TagPrefix="uc" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <uc:Detail runat="server" ID="Detail" />

    <div>
        <div class="float-start">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Profile" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
        <div class="float-end">
            <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this profile?" />
        </div>
    </div>
</asp:Content>
