<%@ Page Language="C#" CodeBehind="Open.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.Open" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Logbook" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Logbook Template
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Logboook Template" />
            </div>
        </div>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div runat="server" id="NewPanel" class="row">

                    <div class="col-md-6">
                                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Logbook Template Title
                                <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" FieldName="Logbook Template Title" ValidationGroup="Logbook" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="TitleInput" MaxLength="400" />
                            </div>
                            <div class="form-text">A descriptive user-friendly title for the logbook.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Logbook Template Name
                                <insite:RequiredValidator runat="server" ControlToValidate="JournalSetupName" FieldName="Logbook Name" ValidationGroup="Logbook" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="JournalSetupName" MaxLength="400" />
                            </div>
                            <div class="form-text">The name that uniquely identifies the logbook for internal filing purposes.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Class
                            </label>
                            <div>
                                <insite:FindEvent runat="server" ID="EventIdentifier" ShowPrefix="false" />
                            </div>
                            <div class="form-text">The class that includes the list of registered students.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Achievement
                            </label>
                            <div>
                                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            </div>
                            <div class="form-text">The course content for this class.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Framework
                            </label>
                            <div>
                                <insite:FindStandard runat="server" ID="StandardIdentifier" TextType="Title" />
                            </div>
                            <div class="form-text">
                                Learning can be tracked against framework.
                                Desired competencies can be selected from an existing framework.
                            </div>
                        </div>

                    </div>
            
                </div>

            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Logbook" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
