<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Sections.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="QuestionList" Src="../Controls/QuestionList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />


    <section class="mb-3" runat="server" id="MainSection">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list"></i>
            Add Section
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Section</h3>
                        <div runat="server" id="SectionNameField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Section Name
                                <insite:RequiredValidator runat="server" ControlToValidate="SectionName" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="SectionName" MaxLength="128" />
                            </div>
                            <div class="form-text">
                                The internal name used to uniquely identify this section for filing purposes.
                            </div>
                        </div>

                        <div runat="server" id="CriterionField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Specification Criteria
                                <insite:RequiredValidator runat="server" ControlToValidate="CriterionSelector" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="CriterionSelector" />
                            </div>
                            <div style="margin-top: 10px;">
                                <asp:CheckBox runat="server" ID="ShowUsedSets" Checked="false" Text="Show criteria for question sets already used by this form" />
                            </div>
                            <div class="form-text">
                                Each section on a form uses criteria to determine the questions it contains and displays.
                            </div>
                        </div>

                        <div runat="server" id="BasicSetField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Question Set
                                <asp:CustomValidator runat="server" ID="SetRequiredValidator" ValidationGroup="Assessment" Display="None" ErrorMessage="Queston Set is a required field" />
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="SetAction">
                                    <Items>
                                        <insite:ComboBoxOption Text="Use Existing" Value="Existing" />
                                        <insite:ComboBoxOption Text="Create Manually" Value="New" />
                                    </Items>
                                </insite:ComboBox>
                                <div runat="server" id="SetSelectorContainer" style="margin-top: 15px;">
                                    <asp:CheckBoxList runat="server" ID="SetCheckBoxList" Visible="false"></asp:CheckBoxList>
                                    <asp:Literal runat="server" ID="SetMessage" Visible="false" />
                                </div>
                            </div>
                            <div class="form-text">
                                Each section on a form uses sets to determine the questions it contains and displays.
                            </div>
                        </div>

                        <div runat="server" id="QuestionLimitField" class="form-group mb-3">
                            <label class="form-label">
                                Question Item Limit
                                <insite:RequiredValidator runat="server" ControlToValidate="QuestionLimit" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="QuestionLimit" NumericMode="Integer" MinValue="0" />
                            </div>
                            <div class="form-text">
                                The maximum number of question items allowed on an exam form from this question set.
                            </div>
                        </div>

                        <div runat="server" id="AddQuestionsField" class="form-group mb-3">
                            <label class="form-label">
                                Add Questions
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="AddQuestions">
                                    <asp:ListItem Value="True" Text="Yes" />
                                    <asp:ListItem Value="False" Text="No" Selected="true" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text">
                                Do you want to select and add specific questions that match the criteria selected above?
                            </div>
                        </div>

                        <div runat="server" id="RandomlySelectField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Randomly Select
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="RandomlySelectSelector">
                                    <Items>
                                        <insite:ComboBoxOption Text="None" Value="None" />
                                        <insite:ComboBoxOption Text="Static" Value="Static" Enabled="false" />
                                        <insite:ComboBoxOption Text="Dynamic" Value="Dynamic" Enabled="false" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>

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

    <section class="mb-3" runat="server" id="PreviewSection" visible="false">
        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Preview Questions <asp:Literal ID="QuestionCount" runat="server"></asp:Literal></h3>
                        <div runat="server" id="NoQuestions" class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i><strong>Warning:</strong>
                            There are no questions that satisfy the criteria you selected.
                        </div>
                        <uc:QuestionList runat="server" ID="QuestionList" />
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row" runat="server" id="ButtonPanel">
        <div class="col-lg-12">
            <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Assessment" CausesValidation="true" Visible="false" />
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>
