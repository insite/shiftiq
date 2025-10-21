<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Criteria.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CriterionInput" Src="../Controls/CriterionInput.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-filter"></i>
            Add Specification Criteria
        </h2>

        <div class="row mb-3">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Specification Name</label>
                            <div>
                                <asp:Literal runat="server" ID="SpecificationName" />
                            </div>
                            <div class="form-text">
                                The specification that contains the new criteria.
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Sets</h3>
                        <div runat="server" id="QuestionSetField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Question Set Names
                            <asp:CustomValidator runat="server" ID="QuestionSetRequired" ValidationGroup="Assessment" Display="None" ErrorMessage="Queston set is a required" />
                            </label>
                            <div>
                                <asp:CheckBoxList runat="server" ID="QuestionSets">
                                </asp:CheckBoxList>
                            </div>
                            <div class="form-text">
                                Select the question set to which this criteria applies.
                            Questions that match the criteria here will become available to include on forms.
                            </div>
                        </div>

                        <asp:Panel runat="server" ID="NoQuestionSetsMessage" CssClass="alert alert-danger" Style="margin-bottom: 0;" Visible="false">
                            This specification already contains criteria for every question set in this bank, so you cannot add more criteria to it.
                        You need to modify the existing criteria for this specification or add a new specifcation.
                        </asp:Panel>
                    </div>
                </div>

            </div>
        </div>

        <uc:CriterionInput runat="server" ID="CriterionInput" Visible="False" />

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" CausesValidation="true" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
