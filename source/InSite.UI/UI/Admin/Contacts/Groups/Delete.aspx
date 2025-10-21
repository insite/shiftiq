<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Delete" %>
<%@ Register Src="Controls/GroupInfo.ascx" TagName="GroupDetail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="alert alert-danger" role="alert" runat="server" ID="AdminErrorPanel" Visible="false"></div>

    <section class="pb-5 mb-md-2">

        <div class="row settings">

                <div class="col-md-6">
                    <div class="settings mb-3">
                        <h3>Summary</h3>
                        <uc:GroupDetail runat="server" ID="GroupDetail" />
                    </div>

                    <div runat="server" id="ConfirmPanel" class="alert alert-danger" role="alert">
                        <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                        Are you sure you want to delete this group?
                    </div>
    
                    <p style="padding-bottom:10px;">
                        <insite:DeleteButton runat="server" ID="DeleteButton" />
                        <insite:Button runat="server" ID="ArchiveButton" Text="Archive" Icon="fas fa-archive" ButtonStyle="Danger" />
                        <insite:CancelButton runat="server" ID="CancelButton" />
                    </p>
                </div>
                        
                <div class="col-md-6">
                    <h3>Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The group will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>

                    <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                        <tr>
                            <td>
                                Group
                            </td>
                            <td>
                                1
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Contained Contacts
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ContactsCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Upstream Relationships
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="UpstreamCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Downstream Relationships
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="DownstreamCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Permissions
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="PermissionsCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Venues
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="EventsCount" />
                            </td>
                        </tr>

                        <tr>
                            <td>
                                Billing Customers
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="CustomersCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Employers
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="EmployersCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Resource Permissions
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ResourcePermissionsCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Jobs
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="OpportunityCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Job Applications
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ApplicationCount" />
                            </td>
                        </tr>
                    </table>

                </div>

            </div>
    </section>

</asp:Content>