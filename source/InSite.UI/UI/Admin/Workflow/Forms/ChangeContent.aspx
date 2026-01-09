<%@ Page Language="C#" CodeBehind="ChangeContent.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.ChangeContent" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit me-1"></i>
            Change Form Content
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Survey" />
            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>
</asp:Content>
