<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Departments.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/DepartmentDetails.ascx" TagName="DepartmentDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="DepartmentInfo" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-building me-1"></i>
            Department Information
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:DepartmentDetails ID="Details" runat="server" />
            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="DepartmentInfo" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
