<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    
    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="mb-3">
        <insite:DeleteConfirmAlert runat="server" Name="Quiz" />
    </div>

    <div class="row">
        <div class="col-lg-6"> 

            <div class="card shadow h-100">
                <div class="card-body">

                    <h3>Quiz</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">Quiz Type</label>
                        <div runat="server" id="QuizType">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">Quiz Name</label>
                        <div runat="server" id="QuizName">
                        </div>
                    </div>

                </div>
            </div>

        </div>
        <div class="col-lg-6">
                
            <h3>Consequences</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The quiz will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>Quiz</td>
                    <td>1</td>
                </tr>
                <tr>
                    <td>Attempts</td>
                    <td runat="server" id="AttemptCount">
                    </td>
                </tr>
            </table>

        </div>
    </div>
   
    <div class="card shadow mt-4">
        <div class="card-body">
            <h5 class="card-title">Action Required</h5>
            <div class="card-text text-danger">

                <div>Yes, I understand the consequences:</div>
                <div class="mb-3 ms-1 mt-1">
                    <insite:CheckBox runat="server" ID="ConfirmCheckBox" Text="Delete this quiz and all its attempts" />
                </div>

                <insite:DeleteButton runat="server" ID="DeleteButton" CssClass="disabled" />
                <insite:CancelButton runat="server" ID="CancelButton" />

            </div>
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (() => {
                const confirmChk = document.getElementById('<%= ConfirmCheckBox.ClientID %>');
                const deleteBtn = document.getElementById('<%= DeleteButton.ClientID %>');

                confirmChk.addEventListener("change", validateDeleteButton);

                validateDeleteButton();

                function validateDeleteButton() {
                    if (!confirmChk.checked)
                        deleteBtn.classList.add("disabled");
                    else
                        deleteBtn.classList.remove("disabled");
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
