<%@ Page Language="C#" CodeBehind="DeleteSessions.aspx.cs" Inherits="InSite.Admin.Surveys.Responses.DeleteSessions" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Surveys/Forms/Controls/SurveyFormInfo.ascx" TagName="SurveyDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Void" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">
                <h3>Survey</h3>
                <uc:SurveyDetails runat="server" id="SurveyDetail" />

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete all response sessions?
                </div>

                <insite:DeleteButton runat="server" ID="DeleteButton" ValidationGroup="Void" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">
            <div class="settings">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The all survey response sessions will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                    <tr>
                        <td>
                            Response Sessions
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ResponseSessionCount" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</div>
</asp:Content>
