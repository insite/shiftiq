<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Assessments.Specifications.Forms.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/SpecificationInfo.ascx" TagName="SpecificationDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clipboard-list"></i>
            Rename Specification
        </h2>

        <div class="row">

            <div class="col-lg-8 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Specification</h3>

                        <div class="row">
                            <div class="col-md-8">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Specification Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="SpecificationName" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="SpecificationName" />
                                    </div>
                                    <div class="form-text">
                                        A short, descriptive name that identifies this specification within the bank.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Specification Type</label>
                                    <div>
                                        <asp:RadioButtonList runat="server" ID="SpecificationType">
                                            <asp:ListItem Value="Dynamic" Text="Dynamic (generated randomly per attempt)" />
                                            <asp:ListItem Value="Static" Text="Static (fixed identically for all attempts)" />
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="form-text" runat="server" id="SpecificationTypeHelp"></div>
                                </div>

                            </div>
                            <div class="col-md-4">
                                <uc:SpecificationDetails runat="server" ID="SpecificationDetails" />
                            </div>
                        </div>

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
