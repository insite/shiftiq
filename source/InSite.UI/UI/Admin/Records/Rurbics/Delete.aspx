<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />

    <section class="pb-5 mb-md-2">

        <div class="row">

            <div class="col-md-6">
                <div class="mb-3">
                    <h3>Rubric</h3>
                    
                    <dl>
                        <dt>Rubric Title</dt>
                        <dd><asp:Literal runat="server" ID="RubricTitle" /></dd>
                        <dt>Rubric Description</dt>
                        <dd><asp:Literal runat="server" ID="RubricDescription" /></dd>
                        <dt>Total Rubric Points</dt>
                        <dd><asp:Literal runat="server" ID="RubricPoints" /></dd>
                    </dl>
                </div>

                <div runat="server" id="ConfirmAlert" class="alert alert-danger" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this rubric and related data?
                </div>

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>    
            </div>
                        
            <div class="col-md-6">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone.
                    The Rubric will be deleted from all questions, forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics">
                    <thead>
                        <tr>
                            <th>Type</th>
                            <th>Rows</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Number of Criteria</td>
                            <td><asp:Literal runat="server" ID="CriteriaCount" /></td>
                        </tr>
                        <tr>
                            <td>Attached Questions</td>
                            <td><asp:Literal runat="server" ID="QuestionsCount" /></td>
                        </tr>
                        <tr>
                            <td>Number of Attempts</td>
                            <td><asp:Literal runat="server" ID="AttemptsCount" /></td>
                        </tr>
                    </tbody>
                </table>

            </div>

        </div>
    </section>
</asp:Content>