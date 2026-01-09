<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">

                <h3>Question</h3>

                <dl class="row">
                    <dt class="col-sm-3">Page #</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionPage" /></dd>

                    <dt class="col-sm-3">Question #</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionNumber" /></dd>

                    <dt class="col-sm-3">Question Text</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionText" /></dd>

                    <dt class="col-sm-3">Question Type</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionType" /></dd>

                    <dt class="col-sm-3">Form Name</dt>
                    <dd class="col-sm-9">
                        <a runat="server" id="SurveyLink">
                            <asp:Literal runat="server" ID="InternalName" />
                        </a>
                    </dd>
                </dl>

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

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The form question will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                    <tr>
                        <td>
                            Option Lists
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="OptionListCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Option Items
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="OptionItemCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Answers
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="AnswerCount" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
</asp:Content>
