<%@ Page Language="C#" CodeBehind="ChangeBranch.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.ChangeBranch" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="SkipPattern" />

    <section runat="server" ID="SurveyInformationSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <h2 runat="server" ID="CommonTitle" class="m-0 mb-2"></h2>
                <h3 runat="server" ID="QuestionTitle" class="m-0 mb-2"></h3>
                <h3 runat="server" ID="OptionListTitle" class="m-0 mb-2"></h3>

                <div class="form-text mb-3">
                    Select the question to which a form respondent will go after answering this question.
                </div>

                <div class="row p-2 ms-0 me-0 border-bottom fw-bold">
                    <div class="col-6">
                        If a respondent selects...
                    </div>
                    <div class="col-6">
                        ... then branch to this question
                    </div>
                </div>

                <asp:Repeater runat="server" ID="SkipPatternsRepeater">
                    <ItemTemplate>
                        <div class='<%# Container.ItemIndex % 2 == 0 ? "bg-secondary": "" %>'>
                            <div class="row p-2 ms-0 me-0 border-bottom">
                                <div class="col-6 fw-bold">
                                    <div class="row">
                                        <div class="col-2">
                                            <%# Eval("OptionSequence") %>.
                                        </div>
                                        <div class="col-10 text-end">
                                            <%# Eval("OptionText") %>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-6">
                                    <insite:FormQuestionComboBox runat="server" ID="QuestionID" DropDown-Size="6" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
