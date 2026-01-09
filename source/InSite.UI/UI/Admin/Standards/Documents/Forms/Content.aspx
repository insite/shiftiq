<%@ Page Language="C#" CodeBehind="Content.aspx.cs" Inherits="InSite.Admin.Standards.Documents.Forms.Content" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Document" />

    <section runat="server" id="Section1" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit me-1"></i>
            Change National Occupation Standard Content
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Document" />
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Document" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>


</asp:Content>
