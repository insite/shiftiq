<%@ Page Language="C#" CodeBehind="DeleteStudent.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.DeleteStudent" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
            
    <div class="row settings">

        <div class="col-lg-6">

            <h3>Learner</h3>

            <uc:PersonDetail runat="server" ID="PersonDetail" />

            <dl class="row">
                <dt class="col-sm-3">Gradebook</dt>
                <dd class="col-sm-9">
                    <a runat="server" Id="GradebookLink">
                        <asp:Literal runat="server" ID="GradebookTitle" />
                    </a>
                </dd>
            </dl>

            <div runat="server" id="AlertPanel" class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to remove this learner from the gradebook?	
            </div>	

            <p style="padding-bottom:10px;">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />	
            </p>	
        </div>

        <div class="col-lg-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The learner will be deleted from all forms, queries, and reports for this gradebook.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Learner
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Scores
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ScoresCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Credentials
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="CredentialCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>

</asp:Content>