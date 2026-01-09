<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Survey" />

    <div class="row settings">
        <div class="col-md-6">
            <div class="settings">
                <h3>Form</h3>
                <uc:FormDetails runat="server" id="SurveyDetail" />
            </div>

            <div runat="server" id="DeleteWarning" class="alert alert-danger" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                <asp:Literal runat="server" ID="ConfirmLiteral" />
                <div runat="server" id="DeleteResponsesPanel">
                    <asp:CheckBox runat="server" ID="DeleteResponsesCheckBox" Text="Delete the form and all the submissions to it." />
                </div>
            </div>
            <p>	
                <insite:DeleteButton runat="server" ID="DeleteButton" ValidationGroup="Survey" />
                <insite:CancelButton runat="server" ID="CancelButton" />	
            </p>
        </div>

        <div class="col-lg-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The form will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Form
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Messages
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="MessageCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Courses
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="CourseCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Submissions
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ResponseCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Questions
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="QuestionCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</asp:Content>
