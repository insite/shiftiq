<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Conditions.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">
                <h3>Condition</h3>
                <dl class="row">
                    <dt class="col-sm-3">Question</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="MaskingQuestionTitle" /></dd>
                </dl>

                <dl class="row" runat="server" id="MaskingListTitleField">
                    <dt class="col-sm-3">List</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="MaskingListTitle" /></dd>
                </dl>

                <dl class="row">
                    <dt class="col-sm-3">Option</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="MaskingOptionTitle" /></dd>

                    <dt class="col-sm-3">Hide</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="MaskedQuestionTitle" /></dd>

                    <dt class="col-sm-3">Form Name</dt>
                    <dd class="col-sm-9">
                        <a runat="server" id="SurveyLink">
                            <asp:Literal runat="server" ID="InternalName" />
                        </a>
                    </dd>
                </dl>

                <p>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </p>
            </div>
        </div>

        <div class="col-md-6">
            <div class="settings">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The form condition will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                    <tr>
                        <td>
                            Condition
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

</div>
</asp:Content>
