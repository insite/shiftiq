<%@ Page Language="C#" CodeBehind="CreateCategory.ascx.cs" Inherits="InSite.Cmds.Actions.Contact.Category.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CategoryDetails.ascx" TagName="CategoryDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CategoryInfo" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-tag me-1"></i>
            Category
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
            <uc:CategoryDetails ID="Details" runat="server" />
            </div>
        </div>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CategoryInfo" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
