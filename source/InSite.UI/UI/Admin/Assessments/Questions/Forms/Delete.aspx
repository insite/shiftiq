<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Questions.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Sets/Controls/SetInfo.ascx" TagName="SetDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row">

        <div class="col-md-6">
            <div class="settings">
                <div class="card mb-3">
                    <div class="card-body">
                        <h3>Question</h3>

                        <dl class="row">
                            <dt class="col-sm-3">Question Text</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionText" /></dd>
                        </dl>

                        <dl class="row">
                            <dt class="col-sm-3">Question Number</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionAssetNumber" /></dd>
                        </dl>

                        <uc:SetDetails runat="server" id="SetDetails" />

                        <dl class="row">
                            <dt class="col-sm-3">Bank Name</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="BankName" /></dd>
                        </dl>
                    </div>
                </div>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this question?
                </div>

                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">
            <div class="settings">
                <div class="card">
                    <div class="card-body">

                        <h3>Impact</h3>

                        <div class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The question will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                        </div>

                        <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                            <tr>
                                <td>
                                    Type
                                </td>
                                <td>
                                    Rows
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Questions
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="QuestionCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Question Options
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="OptionCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Questions Fields
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="FieldCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Answers
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="AttemptQuestionCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Comments
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="AttemptQuestionCommentCount" />
                                </td>
                            </tr>
                        </table>

                        <div class="form-group mb-3">
                            <label class="form-label">Delete Question From...</label>
                            <div>
                                <asp:CheckBoxList runat="server" ID="QuestionContainerList" />
                            </div>
                            <div class="form-text">
                                Select the bank and/or form(s) from which you want to delete this question.
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            $("#<%= QuestionContainerList.ClientID %> input").on("click", removeQuestionFromListClicked);

            $("#<%= DeleteButton.ClientID %>").on("click", function () {
                if ($("#<%= QuestionContainerList.ClientID %> input:checked").length == 0) {
                    alert("Please check the box to indicate what you want to delete.");
                    return false;
                }
                return true;
            });

            function removeQuestionFromListClicked() {
                if ($(this).val() === "0") {
                    var checked = $(this).is(":checked");

                    $("#<%= QuestionContainerList.ClientID %> input").each(function () {
                        if ($(this).val() != "0") {
                            $(this).prop("checked", checked);
                            $(this).prop("disabled", checked);
                        }
                    });
                }

                calcImpact();
            }

            function calcImpact() {
                var totalCount = $("#<%= QuestionContainerList.ClientID %> input").length;
                var checkedCount = $("#<%= QuestionContainerList.ClientID %> input:checked").length;
                var questionCount = totalCount == checkedCount ? 1 : 0;
                var fieldCount = totalCount == checkedCount ? checkedCount - 1 : checkedCount;

                $("#<%= QuestionCount.ClientID %>").html(questionCount);
                $("#<%= FieldCount.ClientID %>").html(fieldCount);
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>
