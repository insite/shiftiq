<%@ Page Language="C#" CodeBehind="EditCategory.ascx.cs" Inherits="InSite.Cmds.Actions.Contact.Category.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
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

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:CategoryDetails ID="Details" runat="server" />
            </div>
        </div>
    </section>

    <div class="row mb-3">
        <div class="col-md-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CategoryInfo" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
        <div class="col-md-6 text-end">
            <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this Category?" />
        </div>
    </div>

</asp:Content>
