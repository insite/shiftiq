<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EducationSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.EducationSection" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 id="education" class="card-title mb-3">Education and Training</h4>

        <div class="form-text mb-3">
            Include any degrees, diplomas, certificates, professional training,
            trade designations or other relevant courses you’ve taken.
        </div>

        <div class="mb-3 clearfix">
            <insite:Button runat="server"
                Icon="fas fa-plus-circle"
                ButtonStyle="Default"
                Text="Add Education"
                NavigateUrl="/ui/portal/job/candidates/profile/add-education"
            />
            <div class="form-text mt-3 float-end">
                At least one entry is mandatory.
                <insite:CustomValidator runat="server" ID="ItemsRequiredValidator" ErrorMessage="At least one education entry must be added" Display="None" ValidationGroup="Profile" />
            </div>
        </div>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>

                <div runat="server" id="NoEducationItems" class="alert alert-warning" role="alert">
                    No Education Item Added
                </div>

                <asp:Repeater runat="server" ID="EducationList">
                    <SeparatorTemplate><div class="mb-4"></div></SeparatorTemplate>
                    <ItemTemplate>
                        <div class="card">
                            <div class="card-body">

                                <h5 class="card-title mb-3">
                                    <%# GetYearString((DateTime?)Eval("EducationDateFrom")) %> - <%# GetYearString((DateTime?)Eval("EducationDateTo")) %>
                                    <%# EvalEducationStatus() %>
                                </h5>

                                <div class="card-text mb-3">
                                    <div class="row" id='item<%# Eval("EducationIdentifier") %>'>

                                        <div class="col-10">
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationName") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationInstitution") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationQualification") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalAddress("EducationCity", "EducationCountry") %>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-2 text-end">
                                            <insite:IconLink runat="server"
                                                ID="EditButton"
                                                Name="pencil"
                                                ToolTip="Edit"
                                                NavigateUrl='<%# Eval("EducationIdentifier", "/ui/portal/job/candidates/profile/edit-education?id={0}") %>'
                                            />
                                            <insite:IconButton runat="server"
                                                Name="trash-alt"
                                                ToolTip="Delete"
                                                CommandName="Delete"
                                                CommandArgument='<%# Eval("EducationIdentifier") %>'
                                                ConfirmText="Are you sure you want to delete this education record?"
                                            />
                                        </div>

                                    </div>
                                </div>

                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </ContentTemplate>
        </insite:UpdatePanel>

    </div>
</div>
