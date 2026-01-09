<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Competencies.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <section class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Competency
        </h2>

        <div class="row mb-3">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Category
                                <insite:RequiredValidator runat="server" ControlToValidate="CategoryIdentifier" FieldName="Category" ValidationGroup="Competency" />
                            </label>
                            <div>
                                <cmds:CompetencyCategorySelector ID="CategoryIdentifier" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Number
                                <insite:CustomValidator ID="UniqueNumber" runat="server" ControlToValidate="Number" ErrorMessage="Competency with this number already exist." ValidationGroup="Competency" Display="None" />
                                <insite:RequiredValidator runat="server" ControlToValidate="Number" FieldName="Number" ValidationGroup="Competency" />
                            </label>
                            <insite:TextBox ID="Number" runat="server" MaxLength="9" />
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Summary
                                <insite:RequiredValidator runat="server" ControlToValidate="Summary" FieldName="Summary" ValidationGroup="Competency" />
                            </label>
                            <insite:TextBox ID="Summary" runat="server" TextMode="MultiLine" Rows="3" MaxLength="256" />
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Old Number
                            </label>
                            <insite:TextBox ID="NumberOld" runat="server" MaxLength="3400" />
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Training Program Url
                            </label>
                            <insite:TextBox ID="ProgramUrl" runat="server" MaxLength="256" />
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Training Program Hours
                            </label>
                            <insite:NumericBox ID="ProgramHours" runat="server" DecimalPlaces="2" />
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Competency" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>
