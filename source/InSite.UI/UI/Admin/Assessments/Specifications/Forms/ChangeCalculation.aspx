<%@ Page Language="C#" CodeBehind="ChangeCalculation.aspx.cs" Inherits="InSite.Admin.Assessments.Specifications.Forms.ChangeCalculation" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/SpecificationInfo.ascx" TagName="SpecificationDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="CalculationDetails" Src="../Controls/ScoreCalculationDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clipboard-list"></i>
            Scoring Calculation
        </h2>

        <div class="row">

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Specification</h3>
                        <uc:SpecificationDetails runat="server" ID="SpecificationDetails" />

                    </div>
                </div>
            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CalculationDetails runat="server" ID="CalculationDetails" />

                    </div>
                </div>

            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Bank</h3>
                        <uc:BankDetails runat="server" ID="BankDetails" />

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
