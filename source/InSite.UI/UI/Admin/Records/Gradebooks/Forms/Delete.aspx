<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/GradebookInfo.ascx" TagName="GradebookDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div runat="server" id="NoVoid" class="alert alert-danger" role="alert">
         <i class="fas fa-stop-circle"></i> This gradebook contains Learner Enrollments and Learner Scores; deleting this gradebook will also permanently delete these.
    </div>
    
    <div class="row">

        <div class="col-lg-6">
                                    
            <div class="card">

                <div class="card-body">

                    <h4 class="card-title">Gradebook</h4>

                    <uc:GradebookDetail runat="server" ID="GradebookDetails" />

                </div>

            </div>

            <insite:DeleteConfirmAlert runat="server" Name="Gradebook" CssClass="mt-3" />

            <div class="mt-3 ms-3 text-danger">
                <asp:CheckBox runat="server" ID="DeleteCheckLearners" Text="Yes I wish to delete all learners and any learner scores, as well as this gradebook." />
            </div>
            <div class="mt-3 ms-3 text-danger">
                <asp:CheckBox runat="server" ID="DeleteCheck" Text="Yes, I understand the consequences, delete this gradebook." />
            </div>

            <div class="mt-3">	
                <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

        </div>

        <div class="col-lg-6">
            <div class="card">
                <div class="card-body">

                    <h3>Consequences</h3>

                    <insite:DeleteWarningAlert runat="server" Name="Appointment" />

                    <table class="table table-striped table-bordered table-metrics">
                        <tr>
                            <th>Data</th>
                            <th class="text-end" style="width:80px;">Items</th>
                        </tr>
                        <tr>
                            <td>Gradebook</td>
                            <td class="text-end">1</td>
                        </tr>
                        <tr>
                            <td>Grade Items</td>
                            <td class="text-end"><asp:Literal runat="server" ID="ScoreItemsCount" /></td>
                        </tr>
                        <tr>
                            <td>Learner Enrollments</td>
                            <td class="text-end"><asp:Literal runat="server" ID="StudentsCount" /></td>
                        </tr>
                        <tr>
                            <td>Learner Scores</td>
                            <td class="text-end"><asp:Literal runat="server" ID="ScoresCount" /></td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>

    </div>
    
</asp:Content>