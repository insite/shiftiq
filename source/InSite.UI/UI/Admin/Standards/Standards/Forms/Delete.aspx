<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/StandardInfo.ascx" TagName="StandardDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
        <div class="alert alert-danger" role="alert">
             <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="DeletePanel" Visible="true">
        <div class="row settings">
            <div class="col-md-6">
                <h3>Standard</h3>

                <uc:StandardDetails Id="StandardDetails" runat="server" />

                <div class="alert alert-danger" role="alert" runat="server" ID="ConfirmText">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                        Are you sure you want to delete this standard?
                </div>

                <p>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </p>
            </div>

            <div class="col-md-6">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The standard will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Standards
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Contained Assets
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ContainedAssetsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Categories
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CategoriesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Upstream Relationships
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="UpstreamRelationshipsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Downstream Relationships
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="DownstreamRelationshipsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Upstream Connections
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="UpstreamConnectionsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Downstream Connections
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="DownstreamConnectionsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Validations
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ValidationsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Profile/Competency Settings
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ProfilesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Department/Profile/Person Settings
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="SettingsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Activity Competencies
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ActivityCompetenciesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Department Occupation Profiles
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="DepartmentStandardsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Organization Profiles
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="StandardOrganizationsCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Logged Competencies
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="ExperienceCompetenciesCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Logbook Competencies
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CompetencyRequirementsCount" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </asp:Panel>
</div>
</asp:Content>
