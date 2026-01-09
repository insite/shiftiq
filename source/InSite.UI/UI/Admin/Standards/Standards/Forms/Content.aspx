<%@ Page Language="C#" CodeBehind="Content.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Content" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Standard" />

    <section runat="server" id="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit me-1"></i>
            Change Standard Content
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Standard" />
                <asp:CustomValidator runat="server" ID="TitleRequiredValidator" ValidationGroup="Standard" ErrorMessage="Title cannot be empty" />
            </div>
        </div>
    </section>

    <div>
        <div>
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Standard" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
