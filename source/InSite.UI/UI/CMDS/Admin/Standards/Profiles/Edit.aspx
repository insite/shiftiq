<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/ProfileHierarchy.ascx" TagName="ProfileHierarchy" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ProfileCompetencyList.ascx" TagName="ProfileCompetencyList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profile" Icon="far fa-ruler-triangle" IconPosition="BeforeText">

            <section>
            
                <h2 class="h4 mt-4 mb-3">Profile</h2>   
    
                <div class="mb-3">
                    <insite:Button runat="server" ID="LockButton" ButtonStyle="Default" Text="Lock" Icon="far fa-lock"
                        ConfirmText="Are you sure you want to lock this profile?" />
                    <insite:Button runat="server" ID="UnlockButton" ButtonStyle="Default" Text="Unlock" Icon="far fa-lock-open"
                        ConfirmText="Are you sure you want to unlock this profile?" />
                    <insite:ButtonSpacer runat="server" ID="LockButtonSpacer" />
                    <insite:Button runat="server" ID="CopyButton" ButtonStyle="Default" Text="Copy" Icon="far fa-copy"
                        ConfirmText="Are you sure you want to copy this profile?" />
                    <insite:Button runat="server" ID="MoveButton" ButtonStyle="Default" Text="Move" Icon="fa-regular fa-briefcase-arrow-right" />
                    <insite:ButtonSpacer runat="server" ID="CopyButtonSpacer" />
                    <insite:Button runat="server" ID="EditCertificateButton" ButtonStyle="Default" Text="Edit College Certificate" Icon="far fa-pencil" />
                    <insite:Button runat="server" ID="ViewDifferencesButton" ButtonStyle="Default" Text="View Differences" Icon="far fa-chart-bar" NavigateTarget="_blank" />
                    <insite:Button runat="server" ID="PrintReportButton" ButtonStyle="Default" Text="Print Report" Icon="far fa-print" />
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-lg-6">

                                <h3>Profile Details</h3>

                                <div runat="server" id="NumberInputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Number
                                        <insite:RequiredValidator runat="server" FieldName="Number" ControlToValidate="NumberInput" ValidationGroup="Profile" Display="Dynamic" />
                                        <asp:CustomValidator ID="UniqueNumber" runat="server" ControlToValidate="NumberInput" ErrorMessage="Another profile with the same number alreadys exists" ValidationGroup="Profile" Display="Dynamic" />
                                    </label>
                                    <insite:TextBox ID="NumberInput" runat="server" MaxLength="40" />
                                </div>

                                <div runat="server" id="NumberOutputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Number
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="NumberOutput" />
                                    </div>
                                </div>

                                <div runat="server" id="TitleInputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Name
                                        <insite:RequiredValidator runat="server" FieldName="Name" ControlToValidate="TitleInput" ValidationGroup="Profile" />
                                    </label>
                                    <insite:TextBox ID="TitleInput" runat="server" MaxLength="256" />
                                </div>

                                <div runat="server" id="TitleOutputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Name
                                    </label>
                                    <div class="white-space:pre-wrap;"><asp:Literal runat="server" ID="TitleOutput" /></div>
                                </div>

                                <div runat="server" id="CertificationHoursPercentCoreInputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        % of Core Hours
                                    </label>
                                    <insite:NumericBox ID="CertificationHoursPercentCoreInput" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" />
                                </div>

                                <div runat="server" id="CertificationHoursPercentCoreOutputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        % of Core Hours
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="CertificationHoursPercentCoreOutput" />
                                    </div>
                                </div>

                                <div runat="server" id="CertificationHoursPercentNonCoreInputField" class="form-group mb-3">
                                    <label class="form-label">
                                        % of Non-Core Hours
                                    </label>
                                    <insite:NumericBox ID="CertificationHoursPercentNonCoreInput" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" />
                                </div>

                                <div runat="server" id="CertificationHoursPercentNonCoreOutputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        % of Non-Core Hours
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="CertificationHoursPercentNonCoreOutput" />
                                    </div>
                                </div>

                                <div runat="server" id="DescriptionInputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Description
                                    </label>
                                    <insite:TextBox ID="DescriptionInput" runat="server" TextMode="MultiLine" />
                                </div>

                                <div runat="server" id="DescriptionOutputField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Description
                                    </label>
                                    <div style="white-space:pre-wrap;"><asp:Literal runat="server" ID="DescriptionOutput" /></div>
                                </div>

                            </div>
                            <div class="col-lg-6">

                                <h3 runat="server" id="ProfileOwnershipHeading">Profile Visibility</h3>

                                <uc:ProfileHierarchy runat="server" ID="ProfileOwnership" />

                                <div runat="server" id="ProfileOwnershipConfirm" visible="false" class="alert alert-danger" role="alert">
                                    <p>Are you sure you want to move this profile to a different organization?</p>
                                    <insite:Button runat="server" ID="ConfirmMoveButton" 
                                        Text="Confirm" ButtonStyle="Danger" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

            </section>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ProfileCompetencyList ID="CompetencyList" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PeopleTab" Title="People" Icon="far fa-user" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    People
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div id="NoPersonPanel" runat="server">
                            This profile is not yet acquired by any workers or learners.
                        </div>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PeopleUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="PeopleUpdatePanel">
                            <ContentTemplate>
                                <uc:PersonGrid ID="PersonGrid" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Profile" />
        <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" 
            ConfirmText="Are you sure you want to delete this profile?" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>