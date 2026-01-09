<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="NameField" Src="../Controls/FormNameField.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            Rename Form
        </h2>
        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Update Form Name</h3>
                        <uc:NameField runat="server" ID="Name" ValidationGroup="Assessment" />
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:FormDetails ID="FormDetails" runat="server" />
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
