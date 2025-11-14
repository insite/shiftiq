<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewExperienceSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ViewExperienceList" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 id="work-experience" class="card-title mb-3">Work Experience</h4>

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

                                        <div class="col-12">
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
