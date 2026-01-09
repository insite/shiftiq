<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Records.Outcomes.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="GradebookPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ballot-check me-1"></i>
            Outcome
        </h2>

        <div class="row">
                        
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Gradebook</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Title
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="GradebookTitle" />
                            </div>
                            <div class="form-text">
                                Created: <asp:Literal runat="server" ID="GradebookCreated" />
                            </div>
                            <div runat="server" id="LockedField" style="color:red;">
                                Locked
                            </div>
                        </div>
            
                        <div runat="server" id="ClassField" class="form-group mb-3">
                            <label class="form-label">
                                Class
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="ClassTitle" />
                            </div>
                            <div class="form-text">
                                <asp:Literal runat="server" ID="ClassScheduled" />
                            </div>
                        </div>

                        <div runat="server" id="ClassInstructorsField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Class Instructors
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="ClassInstructors" />
                            </div>
                        </div>

                        <div class="form-group mb-3" id="AchievementField" runat="server">
                            <label class="form-label">
                                Achievement
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="AchievementTitle" />
                            </div>
                        </div>

                        <div runat="server" id="FrameworkField" class="form-group mb-3">
                            <label class="form-label">
                                Framework
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="FrameworkTitle" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h3>Student</h3>

                        <div class="form-group mb-3" runat="server">
                            <div>
                                <asp:Literal runat="server" ID="StudentFullName" />
                            </div>
                        </div>

                    </div>
                </div>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Competency</h3>

                        <div class="form-group mb-3" runat="server">
                            <div>
                                <asp:Literal runat="server" ID="CompetencyTitle" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server">
                            <label class="form-label">
                                Points
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="ValidationPoints" DecimalPlaces="2" MinValue="0" MaxValue="100000" Width="80" style="text-align:right;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Score" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</div>
</asp:Content>
