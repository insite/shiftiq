<%@ Page Language="C#" CodeBehind="DeleteOne.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Submissions.DeleteOne" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:ValidationSummary runat="server" ValidationGroup="Void" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">
                
                <insite:Container runat="server" ID="RespondentFields" Visible="false">
                    <h3>Respondent</h3>
                    <uc:PersonDetail runat="server" ID="PersonDetail" />
                </insite:Container>

                <dl class="row">
                    <dt class="col-sm-3">Form Name</dt>
                    <dd class="col-sm-9">
                        <a runat="server" id="SurveyLink">
                            <asp:Literal runat="server" ID="InternalName" />
                        </a>
                    </dd>

                    <dt class="col-sm-3">Started</dt>
				    <dd class="col-sm-9"><asp:Literal runat="server" ID="Started" /></dd>

                    <dt class="col-sm-3">Completed</dt>
				    <dd class="col-sm-9"><asp:Literal runat="server" ID="Completed" /></dd>
                </dl>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this submission session?
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
                The form submission session will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
