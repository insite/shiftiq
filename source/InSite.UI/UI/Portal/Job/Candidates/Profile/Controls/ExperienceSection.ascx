<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExperienceSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ExperienceList" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 id="work-experience" class="card-title mb-0">Work Experience</h4>

        <div class="form-text mb-3">
            Include any relevant volunteer work and memberships in relevant Professional or Trade Organizations
        </div>

        <div class="mb-3 clearfix">
            <insite:Button runat="server"
                Icon="fas fa-plus-circle"
                ButtonStyle="Default"
                Text="Add Work Experience"
                NavigateUrl="/ui/portal/job/candidates/profile/add-work-experience"
            />
            <div class="form-text mt-3 float-end">
                At least one entry is mandatory.
                <insite:CustomValidator runat="server" ID="ItemsRequiredValidator" ErrorMessage="At least one work experience entry must be added" Display="None" ValidationGroup="Profile" />
            </div>
        </div>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>

                <div runat="server" id="NoWorkExperienceItems" class="alert alert-warning" role="alert">
                    No Work Experience Added
                </div>

                <asp:Repeater runat="server" ID="WorkExperienceList">
                    <SeparatorTemplate><div class="mb-4"></div></SeparatorTemplate>
                    <ItemTemplate>
                        <div class="card mb-4">
                            <div class="card-body">

                                <h5 class="card-title mb-3">
                                    <%# GetYearString("ExperienceDateFrom") %> - <%# GetYearString("ExperienceDateTo") %>
                                </h5>

                                <div class="card-text mb-3">
                                    <div class="row" id='item<%# Eval("ExperienceIdentifier") %>'>

                                        <div class="col-10">
                                            <div class="row">
                                                <div class="col-md-5">
                                                    <div class="mb-2">
                                                        <%# GetJobTitle() %>
                                                    </div>
                                                    <div class="mb-2">
                                                        <%# HttpUtility.HtmlEncode((string)Eval("EmployerName")) %>
                                                    </div>
                                                    <div class="mb-2">
                                                        <%# EvalFormat("{0}, {1}", "ExperienceCity", "ExperienceCountry") %>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>

                                        <div class="col-2 text-end">
                                            <insite:IconLink runat="server"
                                                ID="EditButton"
                                                Name="pencil"
                                                ToolTip="Edit"
                                                NavigateUrl='<%# GetEditUrl() %>'
                                            />
                                            <insite:IconButton runat="server"
                                                Name="trash-alt"
                                                ToolTip="Delete"
                                                CommandName="Delete"
                                                CommandArgument='<%# Eval("ExperienceIdentifier") %>'
                                                ConfirmText="Are you sure you want to delete this experience?"
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
