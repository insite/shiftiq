<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="BankNameField" Src="../Controls/BankNameField.ascx" %>
<%@ Register Src="../Controls/BankInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-balance-scale"></i>
            Rename Bank
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:Details runat="server" ID="BankDetails" />
                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Update Bank Name</h3>
                        <uc:BankNameField runat="server" ID="Name" ValidationGroup="Assessment" />
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
