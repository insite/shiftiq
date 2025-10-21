<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDetails.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Controls.ViewDetails" %>

    <div class="row mb-3">

        <div class="col-12">

            <div class="card">

                <div class="card-body">

                    <h4 class="card-title mb-3"><asp:Literal runat="server" ID="JobTitle" /></h4>

                    <div class="row">
                        <!-- Left column -->
                        <div class="col-6">
                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">
                                    Type of Employment
                                </h5>
                                <div>
                                    <asp:Literal runat="server" ID="EmploymentType" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Location</h5>
                                <div>
                                    <asp:Literal runat="server" ID="Location" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Organization</h5>
                                <div>
                                    <asp:Literal runat="server" ID="Organization" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Position Type</h5>
                                <div>
                                    <asp:Literal runat="server" ID="PositionType" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Position Level</h5>
                                <div>
                                    <asp:Literal runat="server" ID="PositionLevel" />
                                </div>
                            </div>

                            <div runat="server" class="row mb-3" visible="false" id="StartDateLabel">
                                <h5 class="mt-2 mb-1">Planned Start Date</h5>
                                <div>
                                    <asp:Literal runat="server" ID="StartDate" />
                                </div>
                            </div>

                            <div runat="server" class="row mb-3" visible="false" id="ApplicationDeadlineLabel">
                                <h5 class="mt-2 mb-1">Application Deadline</h5>
                                <div>
                                    <asp:Literal runat="server" ID="ApplicationDeadline" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Salary/Wage range</h5>
                                <div>
                                    <asp:Literal runat="server" ID="Salary" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Resume required with applications?</h5>
                                <div>
                                    <asp:Literal runat="server" ID="IsResumeRequired" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Cover Letter required with applications?</h5>
                                <div>
                                    <asp:Literal runat="server" ID="IsCoverLetterRequired" />
                                </div>
                            </div>

                            <div runat="server" class="row mb-3" visible="false" id="AboutCompanyLabel">
                                <h5 class="mt-2 mb-1">About the Company</h5>
                                <div>
                                    <asp:Literal runat="server" ID="AboutCompany" />
                                </div>
                            </div>

                        </div>

                        <!-- Right column -->
                        <div class="col-6">

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Posted on</h5>
                                <div>
                                    <asp:Literal runat="server" ID="PostedOn" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Description</h5>
                                <div>
                                    <asp:Literal runat="server" ID="Description" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            
            </div>

        </div>

    </div>