<%@ Page Language="C#" CodeBehind="Content.aspx.cs" Inherits="InSite.Admin.Assessments.Sections.Forms.Content" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit"></i>
            Change Question Set Content
        </h2>

        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Assessment" />
                    </div>
                </div>
            </div>
        </div>

    </section>


    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
