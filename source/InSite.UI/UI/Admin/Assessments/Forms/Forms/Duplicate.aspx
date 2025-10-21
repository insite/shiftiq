<%@ Page Language="C#" CodeBehind="Duplicate.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Duplicate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>
<%@ Register TagPrefix="uc" TagName="NameField" Src="../Controls/FormNameField.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CommandStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-balance-scale"></i>
            Duplicate
        </h2>

        <div class="row">

            <div class="col-lg-4">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Settings</h3>
                        <uc:NameField runat="server" ID="Name" ValidationGroup="Assessment" />
                    </div>
                </div>
            </div>

            <div class="col-lg-8">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:FormDetails ID="FormDetails" runat="server" />
                    </div>
                </div>

            </div>
        </div>

    </section>

    <div runat="server" id="DraftedMessage" class="row">
        <div class="col-lg-12">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <div class="alert alert-warning" role="alert">
                        <i class="fas fa-exclamation-triangle"></i><strong>Warning:</strong>
                        This form is currently in development. Are you sure you want to create a duplicate copy of it?
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
