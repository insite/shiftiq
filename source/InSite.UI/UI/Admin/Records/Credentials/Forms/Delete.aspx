<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CredentialDetails.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row settings">

        <div class="col-lg-6">
                                    
            <div class="settings">

                <h3>Credential</h3>

                <uc:Details runat="server" ID="CredentialDetails" />

                <div class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this credential?
                </div>

                <p>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </p>

            </div>

        </div>

        <div class="col-lg-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The credential will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

        </div>

    </div>

</div>

</asp:Content>
