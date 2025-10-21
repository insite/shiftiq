<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Specifications.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CalculationDetails" Src="../Controls/ScoreCalculationDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="ConfigurationDetails" Src="../Controls/ConfigurationDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clipboard-list"></i>
            Specification
        </h2>

        <div class="row mb-3">
            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Specification Type
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="SpecificationType">
                                    <asp:ListItem Value="Dynamic" Text="Dynamic (generated randomly per attempt)" />
                                    <asp:ListItem Value="Static" Text="Static (fixed identically for all attempts)" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text" runat="server" id="SpecificationTypeHelp"></div>
                        </div>

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

                    </div>
                </div>
            </div>
            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:CalculationDetails runat="server" ID="CalculationDetails" />

                    </div>
                </div>
            </div>
        </div>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ConfigurationUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ConfigurationUpdatePanel">
                            <ContentTemplate>
                                <uc:ConfigurationDetails runat="server" ID="ConfigurationDetails" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </div>

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Criteria</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Question Sets
                            </label>
                            <div>
                                <asp:Repeater runat="server" ID="QuestionSetRepeater">
                                    <ItemTemplate>
                                        <insite:CheckBox runat="server" ID="QuestionSetCheckBox" Value='<%# Eval("Identifier") %>' Text='<%# Eval("Name") %>' />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="form-text">
                                Which question sets would you like to include?
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
