<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/RubricDetail.ascx" TagName="RubricDetail" TagPrefix="uc" %>
<%@ Register Src="./Controls/RubricCriteriaList.ascx" TagName="RubricCriteriaList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Rubric" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="DetailsTab" Title="Details" Icon="far fa-table" IconPosition="BeforeText">
            <section class="mb-3">

                <h2 class="h4 mb-3">Rubric</h2>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Rubric" />
                    </div>
                </div>

                <asp:MultiView runat="server" ID="MultiViewRubricDetail">

                    <asp:View runat="server" ID="OneViewRubricDetail">

                        <div class="row">
                            <div class="col-md-6">

                                <div class="card border-0 shadow-lg">
                                    <div class="card-body">

                                        <uc:RubricDetail runat="server" ID="Detail" ShowPoints="false" />

                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                    <asp:View runat="server" ID="CopyViewRubricDetail">

                        <div class="row">
                            <div class="col-md-6">

                                <div class="card border-0 shadow-lg">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Rubric
                                                <insite:RequiredValidator runat="server" ID="CopyRubricValidator" ControlToValidate="CopyRubricComboBox" FieldName="Rubric" ValidationGroup="Rubric" />
                                            </label>
                                            <insite:RubricComboBox runat="server" ID="CopyRubricComboBox" AllowBlank="false" />
                                            <div class="form-text">
                                                Choose the existing rubric you wish to copy.
                                            </div>
                                        </div>

                                        <uc:RubricDetail runat="server" ID="CopyDetail" ShowPoints="false" />

                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                </asp:MultiView>

            </section>

            <div>
                <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Rubric" />
                <insite:CancelButton runat="server" ID="CancelButton1" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-question-circle" IconPosition="BeforeText" Visible="false">

            <section class="mb-3">

                <h2 class="h4 mb-3">Criteria</h2>

                <asp:MultiView runat="server" ID="MultiViewCriteriaList">

                    <asp:View runat="server" ID="OneViewCriteriaList">

                        <uc:RubricCriteriaList runat="server" ID="CriteriaList" />

                    </asp:View>

                    <asp:View runat="server" ID="CopyViewCriteriaList">

                        <uc:RubricCriteriaList runat="server" ID="CopyCriteriaList" />

                    </asp:View>

                </asp:MultiView>
            </section>

            <div>
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Rubric" />
                <insite:CancelButton runat="server" ID="CancelButton2" />
            </div>

        </insite:NavItem>

    </insite:Nav>

</asp:Content>
