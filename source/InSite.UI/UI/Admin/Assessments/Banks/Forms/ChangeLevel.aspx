<%@ Page Language="C#" CodeBehind="ChangeLevel.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.ChangeLevel" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/BankInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">

        <h2 class="h4 mb-3"><i class="far fa-balance-scale me-2"></i>Change Bank Settings</h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Bank Details</h3>
                        <uc:Details runat="server" ID="BankDetails" />
                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Bank Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Level
                            </label>
                            <div class="row">
                                <div class="col-lg-8">
                                    <insite:ComboBox runat="server" ID="LevelType">
                                        <Items>
                                            <insite:ComboBoxOption />
                                            <insite:ComboBoxOption Text="Certificate of Qualification" Value="CofQ" />
                                            <insite:ComboBoxOption Text="Endorsement Exam" Value="EE" />
                                            <insite:ComboBoxOption Text="Foundation Exam" Value="FE" />
                                            <insite:ComboBoxOption Text="Interprovincial Standard Exam" Value="IPSE" />
                                            <insite:ComboBoxOption Text="Standard Level Exam" Value="SLE" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                                <div class="col-lg-4">
                                    <insite:ComboBox runat="server" ID="LevelNumber">
                                        <Items>
                                            <insite:ComboBoxOption />
                                            <insite:ComboBoxOption Text="Level 1" Value="1" />
                                            <insite:ComboBoxOption Text="Level 2" Value="2" />
                                            <insite:ComboBoxOption Text="Level 3" Value="3" />
                                            <insite:ComboBoxOption Text="Level 4" Value="4" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                            </div>
                            <div class="form-text">
                                The type and number for a discrete skill level.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Status
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="BankEnabled">
                                    <Items>
                                        <insite:ComboBoxOption Text="Active" Value="True" />
                                        <insite:ComboBoxOption Text="Inactive" Value="False" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Edition
                                <insite:RequiredValidator runat="server" ControlToValidate="EditionMajor" FieldName="Major Version" ValidationGroup="Assessment" />
                                <insite:RequiredValidator runat="server" ControlToValidate="EditionMinor" FieldName="Minor Version" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <div class="row">
                                    <div class="col-lg-8">
                                        <insite:TextBox runat="server" ID="EditionMajor" ValidationGroup="Assessment" Text="1" />
                                    </div>
                                    <div class="col-md-4">
                                        <insite:TextBox runat="server" ID="EditionMinor" ValidationGroup="Assessment" Text="0" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-text">
                                The edition of this bank (e.g. Year and Month).
                            </div>
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
