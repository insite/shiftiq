<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/SetInfo.ascx" TagName="SetDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list"></i>
            Rename Question Set
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Set</h3>
                        <uc:SetDetails runat="server" ID="SetDetails" />

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Question Set Name
                                <insite:RequiredValidator runat="server" ControlToValidate="SetName" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="SetName" ValidationGroup="Assessment" />
                            </div>
                            <div class="form-text">
                                The name that uniquely identifies this question set within the bank that contains it.
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="col-md-6">
                            <h3>Bank</h3>
                            <uc:BankDetails runat="server" ID="BankDetails" />
                        </div>
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
