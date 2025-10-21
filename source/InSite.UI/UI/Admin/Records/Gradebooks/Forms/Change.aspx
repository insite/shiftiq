<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Gradebook" />

    <section runat="server" ID="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            Change Gradebook Settings
        </h2>

        <div class="row">
            <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Achievement Setup
                            </label>
                            <div>
                                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            </div>
                            <div class="form-text">The achievement granted for successful completion of the items in this gradebook.</div>
                            <div runat="server" id="AchievementWarning" class="alert alert-warning" role="alert" style="margin-top:5px;">
                                You should not modify the achievement for a gradebook with learners who are now enrolled.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Class
                            </label>
                            <div>
                                <insite:FindEvent runat="server" ID="EventIdentifier" ShowPrefix="false" />
                            </div>
                            <div class="form-text">This class contains the registrations for learners tracked in this gradebook.</div>
                        </div>

                        <insite:Alert runat="server" ID="AlertClassStatus" />

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Period
                            </label>
                            <div>
                                <insite:FindPeriod runat="server" ID="PeriodIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>

            <div class="col-md-6">
                            
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Include
                                <insite:CustomValidator runat="server" ID="IncludeValidator" ControlToValidate="EventIdentifier" ValidationGroup="Gradebook" ErrorMessage="Please enable Scores and/or Standards for your new gradebook." />
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="Scores" Text="Scores" Checked="true" Enabled="false" />
                                <asp:CheckBox runat="server" ID="Standards" Text="Standards" />
                            </div>
                            <div class="form-text">Track scores, standards, or both.</div>
                        </div>

                        <div runat="server" id="StandardField" class="form-group mb-3" visible="false">
                            <label class="form-label">
                                Competency Framework
                                <insite:RequiredValidator runat="server" ControlToValidate="StandardIdentifier" FieldName="Framework" ValidationGroup="Gradebook" />
                            </label>
                            <div>
                                <insite:FindStandard runat="server" ID="StandardIdentifier" TextType="Title" />
                            </div>
                            <div class="form-text">The framework that contains the standards measured by this gradebook.</div>
                        </div>

                    </div>
                </div>

            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Gradebook" />
            <insite:CancelButton runat="server" ID="CancelButton"  />
        </div>
    </div>

</asp:Content>
