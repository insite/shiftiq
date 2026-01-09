<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Forms.Dashboard" %>
<%@ Register Src="Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h2 class="h4 mb-3">Person</h2>
                
                <uc:PersonDetail runat="server" ID="PersonDetail" />

                <div runat="server" id="ConfirmMessage" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this person?	
                </div>	

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                    <insite:CloseButton runat="server" ID="CloseButton" Visible="false" />
                </div>
                
            </div>

            <div class="col-lg-6">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The person will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Person
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Group Assessments
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="RolesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Membership Reasons
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="MemberhipReasonCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Standard Validations/Permissions
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ValidationsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Department/Profile/Person Settings
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ProfilesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Person Comments
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CommentsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Experiences
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ExperiencesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Forms Submissions
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="SurveysResponsesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Attempts
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="AttemptsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Event Attendies
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="EventAttendiesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Event Registrations
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="EventRegistrationsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Message Subscribers
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="MessageSubscribers" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Case Responsibilities
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="IssueResponsibilities" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Gradebooks
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="GradebookStudentCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Credentials
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CredentialsCount" />
                        </td>
                    </tr>
                </table>

            </div>
            
        </div>

    </section>
    
    
</asp:Content>
